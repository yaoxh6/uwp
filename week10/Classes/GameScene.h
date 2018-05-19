#pragma once
#include <stdio.h>
#include "cocos2d.h"
USING_NS_CC;

class GameSence : public cocos2d::Scene
{
public:
	static cocos2d::Scene* createScene();

	virtual bool init();

	virtual bool onTouchBegan(Touch *touch, Event *unused_event);

	//virtual void shootMenuCallback(Ref* pSender);

	CREATE_FUNC(GameSence);

private:
	Sprite * mouse;

	Sprite* stone;

	Layer* mouseLayer;

	Layer* stoneLayer;
	//感觉私有变量没什么作用啊，init之后，虽然初始化了
	//但是在其他函数，还是要通过tag或者name获取实例
	Label* shootText;
};