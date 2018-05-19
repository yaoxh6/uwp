#include "GameScene.h"

USING_NS_CC;

Scene* GameSence::createScene()
{
	//这个地方写Sence::create()不会报错,但是没法显示整个页面。
	return GameSence::create();
}

// on "init" you need to initialize your instance
bool GameSence::init()
{
	
	// 1. super init first
	if (!Scene::init())
	{
		return false;
	}

	//add touch listener
	EventListenerTouchOneByOne* listener = EventListenerTouchOneByOne::create();
	listener->setSwallowTouches(true);
	listener->onTouchBegan = CC_CALLBACK_2(GameSence::onTouchBegan, this);
	Director::getInstance()->getEventDispatcher()->addEventListenerWithSceneGraphPriority(listener, this);


	Size visibleSize = Director::getInstance()->getVisibleSize();
	Vec2 origin = Director::getInstance()->getVisibleOrigin();
	//设置背景图片
	auto levelBackGround = Sprite::create("level-background-0.jpg");
	levelBackGround->setPosition(Vec2(visibleSize.width / 2 + origin.x, visibleSize.height / 2 + origin.y));
	this->addChild(levelBackGround, 0);
	//设置石头层
	auto stoneLayer = Layer::create();
	stoneLayer->setPosition(0, 0);
	stoneLayer->setName("stoneLayer");
	this->addChild(stoneLayer, 1);
	//向石头层添加石头
	auto stone = Sprite::create("stone.png");
	stone->setPosition(600, 480);
	stone->setName("stone");
	stoneLayer->addChild(stone, 1);
	//设置老鼠层
	auto mouseLayer = Layer::create();
	mouseLayer->setPosition(0, visibleSize.height / 2);
	mouseLayer->setName("mouseLayer");
	this->addChild(mouseLayer, 1);
	//向老鼠层添加老鼠
	//这个地方巨坑，因为level-sheet.plist文件里面给了不止一个老鼠的动画
	//一开始看到老鼠的动画是老鼠和钻石在一起，所以想到先生成老鼠，再生成钻石
	//这样的话，发现老鼠是动的，钻石是不动的，不符合要求
	//所以把钻石作为老鼠的子类，这样钻石就可以和老鼠一起动
	//但是仔细看demo的示例，老鼠是抱着钻石的，而且钻石是覆盖掉肚子的，这是完全不可能的
	//因为老鼠是一个层面的，钻石是一个层面的，要么老鼠覆盖钻石，要么钻石覆盖老鼠，
	//所以事情的真相只有一个，老鼠抱着钻石本身就是一张图片。。。。。
	const std::string mouseFrameName = "gem-mouse-0.png";
	auto mouse = Sprite::createWithSpriteFrameName("gem-mouse-0.png");
	Animate* mouseAnimate = Animate::create(AnimationCache::getInstance()->getAnimation("mouseAnimation"));
	mouse->runAction(RepeatForever::create(mouseAnimate));
	mouse->setPosition(visibleSize.width / 2, 0);
	mouse->setName("mouse");
	mouseLayer->addChild(mouse, 1);

	//生成shoot文字
	const char *shootName = "shoot";
	auto shootText = Label::createWithTTF(shootName, "fonts/Marker Felt.ttf", 72);
	shootText->setPosition(Vect(origin.x + visibleSize.width - shootText->getContentSize().width, 480));
	shootText->setName("shootText");
	this->addChild(shootText, 1);

	return true;
}

bool GameSence::onTouchBegan(Touch *touch, Event *unused_event) {
	//得到鼠标点击的地方
	Size visibleSize = Director::getInstance()->getVisibleSize();
	auto location = touch->getLocation();
	//很好的控制台输出语句，比uwp方便多了
	CCLOG("location == %f, location == %f", location.x, location.y);
	//获得每个实例，有两种方法getChildByName,或者getChildByTag
	//个人觉得getChildByName更直观一些，需要在前面设置setName
	//Tag，是int型的，需要在addChild的时候设置
	auto stoneLayer = this->getChildByName("stoneLayer");
	auto stone = stoneLayer->getChildByName("stone");
	auto mouseLayer = this->getChildByName("mouseLayer");
	auto mouse = mouseLayer->getChildByName("mouse");
	auto shootText = this->getChildByName("shootText");

	//判断是否点击了shoot文字
	bool isInShootLabel = shootText->getBoundingBox().containsPoint(location);
	if (isInShootLabel) {
		//这里先生成一个stone，然后用生成的stone发射
		//stone移动到老鼠的世界坐标，然后消失，用的是动作序列，不要忘记addChild，否则没有任何反应
		auto stoneTempMoveTo = MoveTo::create(0.5, mouseLayer->convertToWorldSpace(mouse->getPosition()));
		auto stoneTemp = Sprite::create("stone.png");
		stoneTemp->setPosition(600, 480);
		stoneLayer->addChild(stoneTemp);
		//石头渐渐消失
		auto stoneTempFadeOut = FadeOut::create(0.5);
		//将生成的stone删除掉，节约资源
		auto callBackRemove = CallFunc::create([this, &stoneTemp]() {
			this->removeChild(stoneTemp);
		});
		auto stoneTempSequence = Sequence::create(stoneTempMoveTo, stoneTempFadeOut,callBackRemove,nullptr);
		stoneTemp->runAction(stoneTempSequence);

		//随机产生一个新位置，因为容易出界，所以各取了1/2
		auto mouseNewPosition = Vec2(CCRANDOM_0_1() * visibleSize.width*0.5, CCRANDOM_0_1() * visibleSize.height*0.5);
		auto mouseMoveTo = MoveTo::create(0.5, mouseNewPosition);
		mouse->runAction(mouseMoveTo);
		//被石头攻击，老鼠移动会留下钻石，留在老鼠的世界坐标系下
		auto diamond = Sprite::create("diamond.png");
		diamond->setPosition(mouseLayer->convertToWorldSpace(mouse->getPosition()));
		this->addChild(diamond);
	}
	else {
		//当点击的时候不是shoot，就会产生cheese，cheese位置是鼠标点击位置
		auto cheese = Sprite::create("cheese.png");
		cheese->setPosition(location);
		this->addChild(cheese);
		//老鼠移动到cheese位置，需要把cheese转成mouseLayer的相对坐标
		auto mouseMoveTo = MoveTo::create(0.5, mouseLayer->convertToNodeSpace(location));
		mouse->runAction(mouseMoveTo);
		//cheese渐渐消失
		auto cheeseFadeOut = FadeOut::create(0.5);
		//删除cheese节省资源
		auto callBackRemove = CallFunc::create([this, &cheese]() {
			this->removeChild(cheese);
		});
		auto cheeseSequence = Sequence::create(DelayTime::create(0.5), cheeseFadeOut, nullptr);
		cheese->runAction(cheeseSequence);
	}
	return true;
}
