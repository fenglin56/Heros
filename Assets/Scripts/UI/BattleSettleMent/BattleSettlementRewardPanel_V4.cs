using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.Battle{

	public class BattleSettlementRewardPanel_V4 : MonoBehaviour {


		public SingleButtonCallBack BackButton;
		public List<SingleRewardPanel_V4> m_RewardPanelList;

		public Transform FirstHarvestInfoPoint;
		public GameObject HarvestInfoPrefab_Long;
		public GameObject HarvestInfoPrefab_Short;

		public BattleSettlementPanel_V3 MyParent{get;private set;}
		
		void Awake()
		{
			GameDataManager.Instance.dataEvent.RegisterEvent(DataType.EctypeTreasureReward,ReceiveEctypeTreasureRewardMsg);
			m_RewardPanelList.ApplyAllItem(C=>C.gameObject.SetActive(false));
			BackButton.SetCallBackFuntion(OnBackBtnClick);
			BackButton.SetButtonColliderActive(false);
		}

		void OnDestroy()
		{
			GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.EctypeTreasureReward,ReceiveEctypeTreasureRewardMsg);
		}

		public void Show(BattleSettlementPanel_V3 myParent)
		{
			BackButton.SetButtonColliderActive(true);
            //List<int> localActorIDList = new List<int>(actorIDList);
			MyParent = myParent;
			DoForTime.DoFunForTime(1,CheckTreausreRewardData,null);
			var heroActorID = (int)PlayerManager.Instance.FindHeroDataModel().ActorID;
//            if(localActorIDList.Contains(heroActorID))
//			{
//                localActorIDList.Remove(heroActorID);
//			}
			m_RewardPanelList[0].gameObject.SetActive(true);
			m_RewardPanelList[0].Show(heroActorID,this);
//			for(int i = 1;i<m_RewardPanelList.Count;i++)
//			{
//                if(localActorIDList.Count>0)
//				{
//					m_RewardPanelList[i].gameObject.SetActive(true);
//                    m_RewardPanelList[i].Show(localActorIDList[0],this);
//                    localActorIDList.RemoveAt(0);
//				}else
//				{
//					m_RewardPanelList[i].gameObject.SetActive(false);
//				}
//			}

			//副本获得:
			List<SEquipReward> rewardList = new List<SEquipReward>();
			SEquipReward copper = myParent.ScorePanel.sMSGEctypeSettleAccounts.SEquipRewardList.SingleOrDefault(p=>p.dwEquipId == 3050001);
			if(copper.dwEquipId!=0)
			{
				rewardList.Add(copper);
			}
			SEquipReward Experience = myParent.ScorePanel.sMSGEctypeSettleAccounts.SEquipRewardList.SingleOrDefault(p=>p.dwEquipId == 3050004);
			if(Experience.dwEquipId!=0)
			{
				rewardList.Add(Experience);
			}
			var equipList =  myParent.ScorePanel.sMSGEctypeSettleAccounts.SEquipRewardList.Where(
				p=>p.dwEquipId != 3050001 && p.dwEquipId != 3050004).ToList();
			equipList.Sort(new EquipRewardComparerByColor());
			rewardList.AddRange(equipList);
			for(int i=0;i<rewardList.Count;i++)
			{
				GameObject harvestPrefab = i%2==0?HarvestInfoPrefab_Long:HarvestInfoPrefab_Short;
				GameObject harvestInfoItem = UI.CreatObjectToNGUI.InstantiateObj(harvestPrefab,FirstHarvestInfoPoint);
				Vector3 showPos = Vector3.down * i * 35;
				BattleSettlementHarvestInfo info = harvestInfoItem.GetComponent<BattleSettlementHarvestInfo>();
				info.SetInfo((int)rewardList[i].dwEquipId,rewardList[i].dwEquipNum, showPos, i*0.2f);
			}
			//StartCoroutine("ShowFreeTreasureRewardDelay");
		}
		IEnumerator ShowFreeTreasureRewardDelay()
		{
			yield return new WaitForSeconds(MyParent.EctypeData.BattleVictoryLotteryTime);
			if(!m_RewardPanelList[0].IsOpenNorMalBox())
			{
				EctypeTreasureRewardList ectypeTreasureRewardList = new EctypeTreasureRewardList();
				ectypeTreasureRewardList.TreasureList = new List<SMSGEctypeTreasureReward_SC>();
				ectypeTreasureRewardList.TreasureList.Add(new SMSGEctypeTreasureReward_SC(){
					dwUID = PlayerManager.Instance.FindHeroDataModel().UID,
					dwEquipId = MyParent.sMSGEctypeSettleAccounts.dwEquipId,
					dwEquipNum = MyParent.sMSGEctypeSettleAccounts.dwEquipNum,
					byClickType = 0,
				});
				m_RewardPanelList[0].ReceiveTreasureOpenMsg(ectypeTreasureRewardList);
			}
		}
		public void ShowFreeTreasureReward()
		{
			if(!m_RewardPanelList[0].IsOpenNorMalBox())
			{
				EctypeTreasureRewardList ectypeTreasureRewardList = new EctypeTreasureRewardList();
				ectypeTreasureRewardList.TreasureList = new List<SMSGEctypeTreasureReward_SC>();
				ectypeTreasureRewardList.TreasureList.Add(new SMSGEctypeTreasureReward_SC(){
					dwUID = PlayerManager.Instance.FindHeroDataModel().UID,
					dwEquipId = MyParent.sMSGEctypeSettleAccounts.dwEquipId,
					dwEquipNum = MyParent.sMSGEctypeSettleAccounts.dwEquipNum,
					byClickType = 0,
				});
				m_RewardPanelList[0].ReceiveTreasureOpenMsg(ectypeTreasureRewardList);
			}
		}

		void CheckTreausreRewardData(object obj)
		{
			ReceiveEctypeTreasureRewardMsg(null);
		}

		//收到翻宝箱结果
		void ReceiveEctypeTreasureRewardMsg(object obj)
		{
			var gameData = GameDataManager.Instance.GetData(DataType.EctypeTreasureReward);
			if(gameData == null)
				return;
			EctypeTreasureRewardList ectypeTreasureRewardList = (EctypeTreasureRewardList)gameData;
			m_RewardPanelList.ApplyAllItem(C=>C.ReceiveTreasureOpenMsg(ectypeTreasureRewardList));
		}

		void OnBackBtnClick(object obj)
		{
			SoundManager.Instance.PlaySoundEffect("Sound_Button_AppraisalBack");
			if(!IsOpenNorMalBox())
			{
				TraceUtil.Log(SystemModel.Jiang,"发送打开宝箱请求");
				//NetServiceManager.Instance.EctypeService.SendSMSGEctypeClickTreasure_CS(0);
				ShowFreeTreasureReward();
				DoForTime.DoFunForTime(3f,SendBackToTownSceneMsgToSever,null);
			}else
			{
				SendBackToTownSceneMsgToSever(null);
			}
		}

		void SendBackToTownSceneMsgToSever(object obj)
		{
			TraceUtil.Log(SystemModel.Jiang,"返回城镇");
			SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
			long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
			NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
		}
		
		//是否已经打开过宝箱了
		public bool IsOpenNorMalBox()
		{
			bool flag = m_RewardPanelList[0].IsOpenNorMalBox();
			return flag;
		}

		public class EquipRewardComparerByColor : IComparer<SEquipReward>
		{
			public int Compare(SEquipReward x, SEquipReward y)
			{
				int compareResult = 1;

				var itemData_x = ItemDataManager.Instance.GetItemData((int)x.dwEquipId);
				var itemData_y = ItemDataManager.Instance.GetItemData((int)y.dwEquipId);
//				if(itemData_x._GoodsClass == 1)
//				{
//					if(itemData_x._ColorLevel > itemData_y._ColorLevel)
//					{
//						compareResult = 0;
//					}
//				}	
//				else
//				{				
//					compareResult = -1;
//				}
				if(itemData_x._ColorLevel > itemData_y._ColorLevel)
				{
					compareResult = -1;
				}
				return compareResult;
			}
		}

	}
}