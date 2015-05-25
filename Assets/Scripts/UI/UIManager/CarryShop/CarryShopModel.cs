using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarryShopModel:ISingletonLifeCycle {
	private static CarryShopModel instance;
	public static CarryShopModel Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new CarryShopModel();
				SingletonManager.Instance.Add(instance);
			}
			return instance;
		}
	}
	//商店信息
	public bool isRequestedServerData = false;
	public SCarryShopOpenUI_SC sCarryShopOpenUI_SC;
	public int carryShopUpdatDownTime = 0;
	public float carryShopRealtimeSinceStartup = 0;
	//当前切换的界面index
	public int curShowIndexView;
	//当前选中的index下标号
	public int curSelectIndex;
	public int GetCarryShopData(int index)
	{
		for (int i = 0 ; i < sCarryShopOpenUI_SC.shopUintMap.Count ; i++) {
			if(sCarryShopOpenUI_SC.shopUintMap[i].byIndex == index)
			{
				return i;
			}
		}
		return -1;
	}

	#region 封装
	//排序
	public void RankCarryShopData()
	{
		List<DCarryShopUint> shopUintMap = new List<DCarryShopUint> ();
		int maxCount = sCarryShopOpenUI_SC.shopUintMap.Count;
		for (int i = 0; i < maxCount; i++) {
			int index = FindMinOfList(sCarryShopOpenUI_SC.shopUintMap);
			shopUintMap.Add(sCarryShopOpenUI_SC.shopUintMap[index]);
			sCarryShopOpenUI_SC.shopUintMap.RemoveAt(index);
		}
		sCarryShopOpenUI_SC.shopUintMap.Clear ();
		sCarryShopOpenUI_SC.shopUintMap.AddRange(shopUintMap);
	}
	//找出最小的出来//
	int FindMinOfList(List<DCarryShopUint> list)
	{
		uint index = list [0].byIndex;
		int minPos = 0;
		for (int i = 1; i < list.Count; i++) {
			if(index > list[i].byIndex)
			{
				index = list[i].byIndex;
				minPos = i;
			}
		}
		return minPos;
	}
	public bool isJudgeMoneyEnough(UI.EMessageCoinType coinType,int price)
	{
		if (coinType == UI.EMessageCoinType.ECuType) {
			return PlayerManager.Instance.IsMoneyEnough (price);			
		} else {
			return PlayerManager.Instance.IsBindPayEnough (price);
		}
	}
	//解锁开销
	public int GetUnLockPrice(int lockIndex)
	{
		switch(lockIndex)
		{
		case 5:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost5;
			break;
		case 6:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost6;
			break;
		case 7:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost7;
			break;
		case 8:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost8;
			break;
		case 9:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost9;
			break;
		case 10:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost10;
			break;
		case 11:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost11;
			break;
		case 12:
			return CommonDefineManager.Instance.CommonDefine.ShopSlotUnlockCost12;
			break;
		default:
			break;
		}
		return 0;
	}
	#endregion

	#region 处理服务器数据
	public void ReceiveBuyResult(SCarryShopBuy_SC sCarryShopBuy_SC)
	{
		if (sCarryShopBuy_SC.bySucess == 1) {
			int index = GetCarryShopData(sCarryShopBuy_SC.byIndex);
			DCarryShopUint shopUint = sCarryShopOpenUI_SC.shopUintMap[index];
			shopUint.byIsSale = 1;
			sCarryShopOpenUI_SC.shopUintMap.RemoveAt(index);
			sCarryShopOpenUI_SC.shopUintMap.Insert(index,shopUint);
		}
		UIEventManager.Instance.TriggerUIEvent(UIEventType.RcvCarryShopBuyEvent,sCarryShopBuy_SC.bySucess);
	}
	//解锁成功
	public void ReceiveUnLockResult(SCarryShopUnLock_SC sCarryShopUnLock_SC)
	{
		if (sCarryShopUnLock_SC.bySucess == 1) {
			sCarryShopOpenUI_SC.shopUintMap.Add(sCarryShopUnLock_SC.dCarryShopUint);
			RankCarryShopData();
		}
		UIEventManager.Instance.TriggerUIEvent(UIEventType.RcvCarryShopUnLockEvent,sCarryShopUnLock_SC.bySucess);
	}
	//每刷新一次货架消耗元宝= (向下取整 ((参数A×（已换货次数）^2+参数B×已换货次数+参数C)/参数D)×参数D)
	public int GetCarryShopCost(int costTimes)
	{
		int times = CommonDefineManager.Instance.CommonDefine.ShopChangeNum - costTimes;
		return Mathf.FloorToInt ((CommonDefineManager.Instance.CommonDefine.ShopChangeCost1*times*times+
		                          CommonDefineManager.Instance.CommonDefine.ShopChangeCost2*times+CommonDefineManager.Instance.CommonDefine.ShopChangeCost3)/
		                         CommonDefineManager.Instance.CommonDefine.ShopChangeCost4)*CommonDefineManager.Instance.CommonDefine.ShopChangeCost4;
	}
	#endregion

	public void Instantiate()
	{
		
	}

	public void LifeOver()
	{
		instance = null;
	}
}
