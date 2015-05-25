using UnityEngine;
using System.Collections;
using System.Linq;
using UI.MainUI;

public class SirenButtonManager : MonoBehaviour 
{

	void Start()
	{
		Judge(null);
		UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods,Judge);
	}

	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods,Judge);
	}

	private void Judge(object obj)
	{
//		var sirenConfig = SirenDataManager.Instance.GetPlayerSirenList();
//		for(int i=0;i<sirenConfig.Count;i++)
//		{
//			var sirenInfo = SirenManager.Instance.GetYaoNvList().SingleOrDefault(p => p.byYaoNvID == sirenConfig[i]._sirenID);
//			var sirenThisLvData = sirenConfig[i]._sirenConfigDataList.SingleOrDefault(p=>p._growthLevels == sirenInfo.byLevel);
//			//是否突破
//			if(sirenInfo.byLevel >= sirenThisLvData.BreakStageMaxLevel)
//			{
//				bool isEnough = true;
//				for(int j=0;j< sirenThisLvData.BreakCondition.Length;j++)
//				{
//					int hadNum = ContainerInfomanager.Instance.GetItemNumber(sirenThisLvData.BreakCondition[j].ItemID);
//					if(hadNum < sirenThisLvData.BreakCondition[j].ItemNum)
//					{
//						isEnough = false;
//					}
//				}
//				if(isEnough)//够材料突破
//				{
//					UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Siren);
//					break;
//				}
//			}
//			else
//			{
//				var playerData = PlayerManager.Instance.FindHeroDataModel();
//				if(sirenInfo.lExperience + playerData.PlayerValues.PLAYER_FIELD_PRACTICE_NUM>=sirenThisLvData._growthCost)//够修为升级
//				{
//					UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Siren);
//					break;
//				}
//			}
//		}
		if(SirenManager.Instance.IsHasSirenSatisfyIncrease())
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.PlayMainBtnAnim, UIType.Siren);
		}	
		else
		{
			UIEventManager.Instance.TriggerUIEvent(UIEventType.StopMainBtnAnim, UIType.Siren);
		}

	}

}
