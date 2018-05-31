#include "HelloWorldScene.h"
#include "SimpleAudioEngine.h"
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
	dead.pushBack(frame0);
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

	//每一秒时间减少一
	schedule(schedule_selector(HelloWorld::update), 1.0f);
    return true;
}

void HelloWorld::moveCallback(Ref* pSender, string direction)
{
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
		}
		else if (direction == "S") {
			x = 0;
			y = -50;
		}
		else if (direction == "D") {
			x = 50;
			y = 0;
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
void HelloWorld::attackCallback(Ref * pSender)
{
	if (isAnimating == false) {
		isAnimating = true;
		auto sequence = Sequence::create(Animate::create(AnimationCache::getInstance()->getAnimation("attack")),
			CCCallFunc::create(([this]() {
			isAnimating = false;
		})), nullptr);
		player->runAction(sequence);

		float percentage = pT->getPercentage();
		if (percentage < 100) {
			auto to = ProgressFromTo::create(1.0f, percentage, percentage + 20);
			pT->runAction(to);
		}
	}

}

/*
死亡动作的实现
*/
void HelloWorld::deadCallback(Ref * pSender)
{
	if (isAnimating == false) {
		isAnimating = true;
		auto sequence = Sequence::create(Animate::create(AnimationCache::getInstance()->getAnimation("dead")),
			CCCallFunc::create(([this]() {
			isAnimating = false;
		})), nullptr);
		player->runAction(sequence);

		float percentage = pT->getPercentage();
		if (percentage > 0) {
			auto to = ProgressFromTo::create(2.0f, percentage, percentage - 20);
			pT->runAction(to);
		}
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