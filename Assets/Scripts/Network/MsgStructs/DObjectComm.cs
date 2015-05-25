using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

	/////////////////////////////对象类型///////////////////////////////
public enum TypeID
{
	TYPEID_INVALID		 = -1,
	TYPEID_OBJECT        = 0,	// 对象
	TYPEID_ITEM          = 1,	// 道具
	TYPEID_CONTAINER     = 2,	// 背包
	TYPEID_NPC		     = 3,	// NPC
	TYPEID_MONSTER		 = 4,	// 怪物
	TYPEID_PLAYER        = 5,	// 玩家
	TYPEID_GAMEOBJECT    = 6,	// 场景物件
	TYPEID_BOX			 = 7,	// 盒子物件
	TYPEID_CORPSE        = 8,	// 尸体
	TYPEID_PET           = 9,	// 宠物
	TYPEID_CHUNNEL		 = 10,  // 传送门
    TYPEID_TRAP          = -11, // 陷阱
    TYPEID_BULLET        = 11,  //子弹
	TYPEID_MAX			 = 12,	// 最大	
};
