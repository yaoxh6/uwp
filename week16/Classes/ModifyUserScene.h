#pragma once

#ifndef __MODIFY_USER_SCENE_H__
#define __MODIFY_USER_SCENE_H__

#include "cocos2d.h"
#include "ui\CocosGUI.h"
//必须要加这个库，以及引用名字空间，否则虽然没有红线，但是无法运行
#include "network\HttpClient.h"
using namespace cocos2d::network;
using namespace cocos2d::ui;
USING_NS_CC;

class ModifyUserScene : public cocos2d::Scene {
public:
  static cocos2d::Scene* createScene();

  virtual bool init();

  void putDeckButtonCallback(Ref *pSender);
  //为了点击回调
  void onHttpRequestCompleted(HttpClient *sender, HttpResponse *response);
  // implement the "static create()" method manually
  CREATE_FUNC(ModifyUserScene);

  Label *messageBox;
  TextField *deckInput;
};

#endif
