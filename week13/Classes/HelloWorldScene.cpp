#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h"
#include "Monster.h"

//宏定义用于本地数据持久化
#define database UserDefault::getInstance()

//因为string要用，或者std::string也行
using namespace std;
#pragma execution_character_set("utf-8")

USING_NS_CC;

Scene* HelloWorld::createScene()
{
    return HelloWorld::create();
}

// Print useful error message instead of segfaulting when files are not there.
static void problemLoading(const char* filename)
{
    printf("Error while loading: %s\n", filename);
    printf("Depending on how you compiled you might have to add 'Resources/' in front of filenames in HelloWorldScene.cpp\n");
}

// on "init" you need to initialize your instance
bool HelloWorld::init()
{
    //////////////////////////////
    // 1. super init first
    if ( !Scene::init() )
    {
        return false;
    }

    visibleSize = Director::getInstance()->getVisibleSize();
    origin = Director::getInstance()->getVisibleOrigin();

	//这里需要特别注意
	/***********************************************************************/
	/*这里差不多花了三个小时的时间
	主要解决，图片没有显示的问题，上网查了很多资料，没有一个有用的。
	有的说png图片不能用，有的说bmp图片不能用，我暂时用的jpg图片。
	据说新建层没有添加物体会奔溃，发现并没有，不知道网上的问题是怎么回事，全部都是错误答案。
	也可能是版本的问题，毕竟那个时候还是CC前缀还在的时候
	至于除去以上问题，画面还是没法显示，怎么解决？
	经过尝试，这种方法是可以的：
	一开始将所有要用到的资源放在Resources，不能直接把生成好的map.tmx放在Resources，
	否则没有显示，但是tmx ！= NULL。也不能将生成好的map.tmx先放在Resources下，再把用到
	的资源放在Resources，亲测不行。只能先把要用到的资源放在Resources下，我用到的资源是
	Test.jpg，然后打开TileMap，新建地图，保存地址也要保存到Resources，最好不要保存到其他位置，然后
	再拷贝到Resources，接下来导入图块，一定要用Resources下的图片，这样生成的图片才可以在Cocos中显示。*/
	/***********************************************************************/
	TMXTiledMap *tmx = TMXTiledMap::create("map.tmx");
	tmx->setPosition(visibleSize.width / 2, visibleSize.height / 2);
	CCLOG("%f,%f", visibleSize.width / 2, visibleSize.height / 2);
	tmx->setAnchorPoint(Vec2(0.5, 0.5));
	tmx->setScale(Director::getInstance()->getContentScaleFactor());
	addChild(tmx, 0);

	//创建一张贴图
	auto texture = Director::getInstance()->getTextureCache()->addImage("$lucia_2.png");
	//从贴图中以像素单位切割，创建关键帧
	auto frame0 = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(0, 0, 113, 113)));
	//使用第一帧创建精灵
	player = Sprite::createWithSpriteFrame(frame0);
	player->setPosition(Vec2(origin.x + visibleSize.width / 2,
		origin.y + visibleSize.height / 2));
	addChild(player, 3);

	//hp条
	Sprite* sp0 = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(0, 320, 420, 47)));
	Sprite* sp = Sprite::create("hp.png", CC_RECT_PIXELS_TO_POINTS(Rect(610, 362, 4, 16)));

	//使用hp条设置progressBar
	pT = ProgressTimer::create(sp);
	pT->setScaleX(90);
	pT->setAnchorPoint(Vec2(0, 0));
	pT->setType(ProgressTimerType::BAR);
	pT->setBarChangeRate(Point(1, 0));
	pT->setMidpoint(Point(0, 1));
	pT->setPercentage(100);
	pT->setPosition(Vec2(origin.x + 14 * pT->getContentSize().width, origin.y + visibleSize.height - 2 * pT->getContentSize().height));
	addChild(pT, 1);
	sp0->setAnchorPoint(Vec2(0, 0));
	sp0->setPosition(Vec2(origin.x + pT->getContentSize().width, origin.y + visibleSize.height - sp0->getContentSize().height));
	addChild(sp0, 0);

	// 静态动画
	idle.reserve(1);
	idle.pushBack(frame0);

	// 攻击动画
	attack.reserve(17);
	for (int i = 0; i < 17; i++) {
		auto frame = SpriteFrame::createWithTexture(texture, CC_RECT_PIXELS_TO_POINTS(Rect(113 * i, 0, 113, 113)));
		attack.pushBack(frame);
	}
	//这里又加入了frame0，原因是，动作结束之后要处理静止状态，而不是动作结束之后的状态
	attack.pushBack(frame0);
	auto attackAnimation = Animation::createWithSpriteFrames(attack, 0.1f);
	AnimationCache::getInstance()->addAnimation(attackAnimation, "attack");

	// 可以仿照攻击动画
	// 死亡动画(帧数：22帧，高：90，宽：79）
	auto texture2 = Director::getInstance()->getTextureCache()->addImage("$lucia_dead.png");
	// Todo
	dead.reserve(22);
	for (int i = 0; i < 22; i++) {
		auto frame = SpriteFrame::createWithTexture(texture2, CC_RECT_PIXELS_TO_POINTS(Rect(79 * i, 0, 79, 90)));
		dead.pushBack(frame);
	}
	//dead.pushBack(frame0);
	auto deadAnimation = Animation::createWithSpriteFrames(dead, 0.1f);
	AnimationCache::getInstance()->addAnimation(deadAnimation, "dead");

	// 运动动画(帧数：8帧，高：101，宽：68）
	auto texture3 = Director::getInstance()->getTextureCache()->addImage("$lucia_forward.png");
	// Todo
	run.reserve(8);
	for (int i = 0; i < 2; i++) {
		auto frame = SpriteFrame::createWithTexture(texture3, CC_RECT_PIXELS_TO_POINTS(Rect(68 * i, 0, 68, 101)));
		run.pushBack(frame);
	}
	run.pushBack(frame0);
	auto runAnimation = Animation::createWithSpriteFrames(run, 0.1f);
	AnimationCache::getInstance()->addAnimation(runAnimation, "run");

	auto menu = Menu::create();
	menu->setPosition(80, 50);
	addChild(menu);

	//参考以往代码，感觉这样写比较好
	auto createDirectionLabel = [this, &menu](string c) {
		int x = 0, y = 0;
		auto label = Label::create(c, "arial", 36);
		auto menuItem = MenuItemLabel::create(label, CC_CALLBACK_1(HelloWorld::moveCallback, this, c));
		if (c == "W") {
			y += 1.2 * label->getContentSize().height;
		}
		else if (c == "A") {
			x -= 1.5 * label->getContentSize().width;
		}
		else if (c == "D") {
			x += 1.5 * label->getContentSize().width;
		}
		menuItem->setPosition(x, y);
		menu->addChild(menuItem);
	};

	//创建方向键
	createDirectionLabel("W");
	createDirectionLabel("S");
	createDirectionLabel("A");
	createDirectionLabel("D");

	//X按钮
	auto labelX = Label::create("X", "fonts/arial.ttf", 36);
	auto menuItem = MenuItemLabel::create(labelX, CC_CALLBACK_1(HelloWorld::attackCallback, this));
	menuItem->setPosition(origin.x + visibleSize.width - 120, -15);
	menu->addChild(menuItem);

	//Y按钮
	auto labelY = Label::create("Y", "fonts/arial.ttf", 36);
	menuItem = MenuItemLabel::create(labelY, CC_CALLBACK_1(HelloWorld::deadCallback, this));
	menuItem->setPosition(origin.x + visibleSize.width - 100, 15);
	menu->addChild(menuItem);

	//倒计时
	time = Label::createWithTTF("180", "fonts/arial.ttf", 36);
	time->setPosition(origin.x + visibleSize.width / 2, origin.y + visibleSize.height -50);
	addChild(time);

	score = Label::createWithTTF("0", "fonts/arial.ttf", 36);
	score->setPosition(origin.x + visibleSize.width / 2, origin.y + visibleSize.height - 100);
	addChild(score);

	//每一秒时间减少一
	schedule(schedule_selector(HelloWorld::update), 1.0f);
	//每两秒生成一个怪物，并且移动
	schedule(schedule_selector(HelloWorld::createMonster), 2.0f);
	//每0.1秒检测有没有被怪物攻击
	schedule(schedule_selector(HelloWorld::hitByMonster), 0.1f);
	//每0.1秒检测主角有没有死
	//为什么要这么做，其实也是无奈的办法
	//因为如果死亡就要播放死亡动画，死亡动画结束后，取消所有动作，
	// 将死亡动画和取消所有动作放在一个函数中的时候，因为死亡动画是要时间的，函数会
	//直接执行，所以取消所有动作覆盖了死亡动画，导致没有播放，有人可能会依靠isDead来实现
	//先死亡再停止，但是本质没有改变，用一个序列来实现，序列结束的时候改变isDead的值，再用这个值
	//决定是否停止动画，同样，停止所有动画不会等isDead的值改变再执行。所有直接用调度器了。
	schedule(schedule_selector(HelloWorld::stop), 0.1f);
    return true;
}

void HelloWorld::moveCallback(Ref* pSender, string direction)
{
	if (isDead == true) {
		return;
	}
	auto position = player->getPosition();
	//事实证明，在区间(0,visibleSize.width)之间还是会出界，所以再缩小一点
	if (isAnimating == false && ((position.x > 50 && direction == "A") || (position.x < visibleSize.width-50 && direction == "D") || (position.y > 50 && direction == "S") || (position.y < visibleSize.height-50 && direction == "W"))) {
		isAnimating = true;
		int x, y;
		if (direction == "W") {
			x = 0;
			y = 50;
		}
		else if (direction == "A") {
			x = -50;
			y = 0;
			player->setFlippedX(true);
		}
		else if (direction == "S") {
			x = 0;
			y = -50;
		}
		else if (direction == "D") {
			x = 50;
			y = 0;
			player->setFlippedX(false);
		}
		//移动和动画是同时执行的
		auto spawn = Spawn::createWithTwoActions(Animate::create(AnimationCache::getInstance()->getAnimation("run")), MoveBy::create(0.5f, Vec2(x, y)));
		//执行完之后要将isAnimating置为false
		auto sequence = Sequence::create(spawn, CCCallFunc::create(([this]() { isAnimating = false; })), nullptr);
		player->runAction(sequence);
	}
	return;
}

/*
实现攻击动作，攻击工作结束将isAnimating置为false
加分项是实现血条的变化
*/
/*
这次作业将血条和攻击分离了
*/
void HelloWorld::attackCallback(Ref * pSender)
{
	if (isDead == true) {
		return;
	}
	if (isAnimating == false) {
		isAnimating = true;
		isAttack = true;
		auto sequence = Sequence::create(Animate::create(AnimationCache::getInstance()->getAnimation("attack")),
			CCCallFunc::create(([this]() {
			isAnimating = false;
			isAttack = false;
		})), nullptr);
		player->runAction(sequence);
	}
}

/*
死亡动作的实现
*/
/*
死亡之后，处理调度器
*/
void HelloWorld::deadCallback(Ref * pSender)
{
	if (isDead == false) {
		isAnimating = true;
		auto sequence = Sequence::create(Animate::create(AnimationCache::getInstance()->getAnimation("dead")),
			CCCallFunc::create(([this]() {
			isAnimating = false;
			isDead = true;
		})), nullptr);
		player->runAction(sequence);
		unschedule(schedule_selector(HelloWorld::createMonster));
		unschedule(schedule_selector(HelloWorld::hitByMonster));
		unschedule(schedule_selector(HelloWorld::update));
	}
}


/*
update实现倒计时的功能，其中开始的值是180
需要每秒减一，所以需要拿到当前的值，并且减一再赋值回去
其中需要int和string的相互转换
由于是cocos2d，还有一个新的类型是CCString
这三者之间的转换参考下面链接
https://www.cnblogs.com/leehongee/p/3642308.html
*/

void HelloWorld::update(float dt)
{
	string str = time->getString();
	int timeLength = atoi(str.c_str());
	if (timeLength > 0) {
		timeLength--;
		CCString* ns = CCString::createWithFormat("%d", timeLength);
		string s = ns->_string;
		time->setString(s);
	}
	else {
		unschedule(schedule_selector(HelloWorld::update));
	}
}

//参考PPT
void HelloWorld::createMonster(float dt) 
{
	auto fac = Factory::getInstance();
	auto m = fac->createMonster();
	float x = random(origin.x, visibleSize.width);
	float y = random(origin.y, visibleSize.height);
	m->setPosition(x, y);
	addChild(m, 3);
	fac->moveMonster(player->getPosition(), 0.5f);
}

//参考PPT
void HelloWorld::hitByMonster(float dt) 
{
	auto fac = Factory::getInstance();
	Rect playerRect = player->getBoundingBox();
	Rect attackRect = Rect(playerRect.getMinX() - 40, playerRect.getMinY(), playerRect.getMaxX() - playerRect.getMinX() + 80, playerRect.getMaxY() - playerRect.getMinY());
	Sprite *attackCollision = fac->collider(attackRect);
	//这边注意判断的时候，是嵌套关系，不是并列关系
	//就是说不能将attackCollison和playerCollision放在并列关系判断，否则会出错
	if (attackCollision != NULL) {
		if (isAttack) {
			fac->removeMonster(attackCollision);
			float percentage = pT->getPercentage();
			if (percentage < 100) {
				auto to = ProgressFromTo::create(1.0f, percentage, percentage + 20);
				pT->runAction(to);
			}
			string str = score->getString();
			int scoreLength = atoi(str.c_str());
			scoreLength++;
			CCString* ns = CCString::createWithFormat("%d", scoreLength);
			string s = ns->_string;
			score->setString(s);
			//放在本地文件UserDefault.xml中
			database->setIntegerForKey("score",scoreLength);
			//这里又要吐槽一下网上的解答，说UserDefault.xml文件在Debug文件夹里面之类的
			//直接用下面的输出语句就可以看到地址，实现本地数据持久化
			CCLOG("%s", FileUtils::getInstance()->getWritablePath().c_str());
		}
		else {
			Sprite *playerCollision = fac->collider(playerRect);
			if (playerCollision != NULL) {
				fac->removeMonster(playerCollision);
				float percentage = pT->getPercentage();
				if (percentage > 0) {
					auto to = ProgressFromTo::create(1.0f, percentage, percentage - 20);
					pT->runAction(to);
				}
				else {
					deadCallback(NULL);
				}
			}
		}
	}
}

//停止所有动作
void HelloWorld::stop(float dt) 
{
	if (isDead == true) {
		player->stopAllActions();
	}
}