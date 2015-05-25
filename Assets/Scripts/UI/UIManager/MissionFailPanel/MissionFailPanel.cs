using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class MissionFailPanel : MonoBehaviour
    {

        public UILabel LevelLabel;
        public UILabel ForceLabel;
        public UILabel MoneyLabel;
        public UILabel EnergyLabel;
        public SingleButtonCallBack CloseBtn;
        public SingleMissionfailButton[] MissionFailBtnList;
        public MissionFailBtnDataBase MissionFailData;

		bool isShow = false;

        void Awake()
        {
            CloseBtn.SetCallBackFuntion(OnCloseBtnClick);
            MissionFailBtnList.ApplyAllItem(P=>P.Init());
        }

        public void Show()
        {
			isShow = true;
            gameObject.SetActive(true);
            UIType[] enabelUIType = UIDataManager.Instance.InitMainButtonList.First(P => P._EnableIndex == GameManager.Instance.MainButtonIndex)._MainButtonList;
            for (int i = 0; i < MissionFailData.MissionFailDataTable.Length; i++)
            {
                bool isActive = enabelUIType.FirstOrDefault(P=>P == MissionFailData.MissionFailDataTable[i].BtnType)!=UIType.Empty;
                MissionFailBtnList[i].SetBtnActive(MissionFailData.MissionFailDataTable[i],this,isActive);
            }
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            LevelLabel.SetText(m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL);
            int NewAtk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
            ForceLabel.SetText(NewAtk);
            MoneyLabel.SetText(m_HeroDataModel.PlayerValues.PLAYER_FIELD_HOLDMONEY);
            EnergyLabel.SetText(m_HeroDataModel.PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE);
        }

        public void OnSingleBtnClick(MissionFailData selectData)
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI,selectData.BtnType);
            OnCloseBtnClick(null);
        }

        void OnCloseBtnClick(object obj)
        {
			isShow = false;
            gameObject.SetActive(false);
        }

        [ContextMenu("InitBtnList")]
        void InitBtnList()
        {
            MissionFailBtnList = transform.GetComponentsInChildren<SingleMissionfailButton>();
        }

		void Update()
		{
			if(isShow&&TaskModel.Instance.TaskGuideType==TaskGuideType.Enforce)// NewbieGuideManager_V2.Instance.IsConstraintGuide == true)//如果有强引导，则关闭
			{
				OnCloseBtnClick(null);
			}
		}

    }
}