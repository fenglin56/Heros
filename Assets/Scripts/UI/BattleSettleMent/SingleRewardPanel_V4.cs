using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.Battle{

	public class SingleRewardPanel_V4 : MonoBehaviour {


		public Vector3 HidePos;
		public Vector3 ShowPos;
		public UILabel NameLabel;
		public UISprite HeardIcon;
		public SpriteSwith VocationSwith;
		public SingleTreasureChests_V4[] SingleTreasureChestsList;

		public bool IsHero{get;private set;}
		public string Name{get;private set;}
		public int FashionID{get;private set;}
		public int Vocation{get;private set;}
		public long RoleUID{get;private set;}
		public BattleSettlementRewardPanel_V4 MyParent{get;private set;}
		

		public void Show(long actorID,BattleSettlementRewardPanel_V4 myParent)
		{
			MyParent = myParent;
			TweenPosition.Begin(gameObject,0.3f,HidePos,ShowPos,ShowOver);

			//新增动画
			SingleTreasureChestsList[1].transform.localPosition = SingleTreasureChestsList[0].transform.localPosition+Vector3.back * 5;
			SingleTreasureChestsList[2].transform.localPosition = SingleTreasureChestsList[0].transform.localPosition+Vector3.back * 5;

			EntityModel entityModel = PlayerManager.Instance.FindPlayerByActorId((int)actorID);
			var playerBehaviour = (PlayerBehaviour)entityModel.Behaviour;
			IPlayerDataStruct data = (IPlayerDataStruct)entityModel.EntityDataStruct;
			IsHero = playerBehaviour.IsHero;
			if(IsHero)
			{
				var roleData = (SMsgPropCreateEntity_SC_MainPlayer)entityModel.EntityDataStruct;
				Name = roleData.Name;
				RoleUID = roleData.UID;
				FashionID = roleData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
				Vocation = roleData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			}else
			{
				var roleData = (SMsgPropCreateEntity_SC_OtherPlayer)entityModel.EntityDataStruct;
				Name = roleData.Name;
				RoleUID = roleData.UID;
				FashionID = roleData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
				Vocation = roleData.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			}
			
			var resData = CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_SettlementReward.FirstOrDefault(P => P.VocationID == Vocation && P.FashionID == FashionID);
			if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + FashionID); }
			HeardIcon.spriteName = resData.ResName;
			HeardIcon.enabled = true;
			NameLabel.SetText(Name);
			NameLabel.enabled = true;
			VocationSwith.ChangeSprite(Vocation);
			VocationSwith.enabled = true;
			SingleTreasureChestsList[0].Init(TreasureChestsType.Normal,this,IsHero);
			SingleTreasureChestsList[1].Init(TreasureChestsType.CostMoney,this,IsHero);
			SingleTreasureChestsList[2].Init(TreasureChestsType.VIP,this,IsHero);
		}

		void ShowOver(object obj)
		{
			Vector3 pos = SingleTreasureChestsList[0].transform.localPosition + Vector3.down * 110+Vector3.back * 5;
			TweenPosition.Begin(SingleTreasureChestsList[1].gameObject,0.3f,pos);
			TweenPosition.Begin(SingleTreasureChestsList[2].gameObject,0.3f,SingleTreasureChestsList[2].transform.localPosition, pos,MoveOver);
		}
		void MoveOver(object obj)
		{
			Vector3 pos = SingleTreasureChestsList[0].transform.localPosition + Vector3.down * 220+Vector3.back * 5;
			TweenPosition.Begin(SingleTreasureChestsList[2].gameObject,0.3f,pos);
		}

		public void ReceiveTreasureOpenMsg(EctypeTreasureRewardList rewardDataList)
		{
			if(!gameObject.activeSelf)
				return;
			var treasureRewardData = rewardDataList.TreasureList.FirstOrDefault(C=>C.dwUID == RoleUID);
			if(treasureRewardData.dwUID!=0)
			{
				SingleTreasureChestsList[treasureRewardData.byClickType].Open(treasureRewardData.dwEquipId,treasureRewardData.dwEquipNum);
			}
		}

		public bool IsOpenNorMalBox()
		{
			bool flag = SingleTreasureChestsList[0].IsTreasureChestsOpened;
			return flag;
		}

		[ContextMenu("InigPos")]
		void InitPos()
		{
			transform.localPosition = HidePos;
		}

	}
}