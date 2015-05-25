using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace UI.MainUI
{

    public class RoleAttributePanel : MonoBehaviour
    {
        public Camera MyUICamera;

        public SingleRoleAtrribute[] SingleRoleAttributeList;

        //private Vector3 ShowPosition = Vector3.zero;
        //private Vector3 HidePosition = new Vector3(-280,0,0);
        private bool IsShow = false;

        private Dictionary<RoleAttributeType, SingleRoleAtrribute> RoleAtbList = new Dictionary<RoleAttributeType, SingleRoleAtrribute>();

        private Action mCallBackAction;

        void Awake()
        {
            foreach (var child in SingleRoleAttributeList)
            {
                RoleAtbList.Add(child.roleAttributeType, child);
            }
            //transform.localPosition = HidePosition;

            #region edit by lee
            mCallBackAction = InitialShowAttributePanelInfoFuntion;
            #endregion 

            ShowAttributePanelInfo();        
        }

        public bool ChangePanelActive()
        {
            IsShow = !IsShow;
            //TweenPosition.Begin(gameObject,0.5f,IsShow?ShowPosition:HidePosition);
            return IsShow;
        }

        public void ClosePanel()
        {
            if (IsShow)
            {
                IsShow = !IsShow;
                //transform.localPosition = HidePosition;
            }
        }

        public void ShowAttributePanelInfo()
        {
            this.mCallBackAction();            
        }

        public void SetPanelPosition(Vector3 CameraPosition)
        {
            transform.parent.position = MyUICamera.ScreenToWorldPoint(CameraPosition);
        }

        #region edit by lee
        void InitialShowAttributePanelInfoFuntion()
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            //RoleAtbList[RoleAttributeType.MaxHP].ResetInfo(GetRoleValue(25, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP));
            //RoleAtbList[RoleAttributeType.MaxMP].ResetInfo(GetRoleValue(26, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP));
            //RoleAtbList[RoleAttributeType.ATK].ResetInfo(GetRoleValue(30, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_ATTACK));
            //RoleAtbList[RoleAttributeType.DEF].ResetInfo(GetRoleValue(31, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_DEFEND));
            //RoleAtbList[RoleAttributeType.HIT].ResetInfo(GetRoleValue(32, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_NICETY));
            //RoleAtbList[RoleAttributeType.EVA].ResetInfo(GetRoleValue(29, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_JOOK));
            //RoleAtbList[RoleAttributeType.Crit].ResetInfo(GetRoleValue(27, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_BURST));
            //RoleAtbList[RoleAttributeType.ResCrit].ResetInfo(GetRoleValue(28, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_UNBURST));
            RoleAtbList[RoleAttributeType.MaxHP].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_MaxHp,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXHP).ToString());
            RoleAtbList[RoleAttributeType.MaxMP].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_MaxMP,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_MAXMP).ToString());
            RoleAtbList[RoleAttributeType.ATK].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_ATK,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_ATTACK).ToString());
            RoleAtbList[RoleAttributeType.DEF].ResetInfo( HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_DEF,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_DEFEND).ToString());
            RoleAtbList[RoleAttributeType.HIT].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_HIT,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_NICETY).ToString());
            RoleAtbList[RoleAttributeType.EVA].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_EVA, m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_JOOK).ToString());
            RoleAtbList[RoleAttributeType.Crit].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Crit,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_BURST).ToString());
            RoleAtbList[RoleAttributeType.ResCrit].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_ResCrit,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitInvisibleValue.UNIT_FIELD_UNBURST).ToString());
            //RoleAtbList[RoleAttributeType.Force].ResetInfo(HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat,m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING).ToString());
        }
        public Dictionary<RoleAttributeType, SingleRoleAtrribute> GetListAndResetCallBack(Action callBack)
        {
            this.mCallBackAction = callBack;            
            return RoleAtbList;
        }
        #endregion

        //string GetRoleValue(int BasePropID, int CurrentValue)
        //{
        //    var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
        //    int vocation = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        //    int Level = m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
        //    int baseValuer = GetRoleBaseValue(vocation, Level, BasePropID);
        //    string AddStr = baseValuer == CurrentValue ? baseValuer.ToString() : string.Format("{0} {1}", baseValuer, NGUIColor.SetTxtColor(string.Format("(+{0})", CurrentValue - baseValuer), TextColor.green));
        //    return AddStr;
        //}

//        int GetRoleBaseValue(int Vocation, int Level, int BasePropID)
//        {
//            var PlayerData = PlayerDataManager.Instance.PlayerBasePropConfigData.playerBasePropDatalist.SingleOrDefault(P => P.PlayerKind == Vocation && P.BasePropID == BasePropID);
//            if (PlayerData == null)
//            {
//                return 0;
//            }
//            else
//            {
//                int m_value = (int)(Mathf.FloorToInt((PlayerData.ParamA * Mathf.Pow(Level, 2) + PlayerData.ParamB * Level + PlayerData.ParamC) / PlayerData.ParamD) * PlayerData.ParamD);
//                //TraceUtil.Log("获取基础值=>" + PlayerData.BaseProp + ":" + string.Format("A:{0},B:{1},C:{2},D:{3},Value:{4}", PlayerData.ParamA, PlayerData.ParamB, PlayerData.ParamC, PlayerData.ParamD, m_value));
//                return m_value;
//            }
//        }

		[ContextMenu("InitChildItemList")]
		void InitChildItemList()
		{
			SingleRoleAttributeList = transform.GetComponentsInChildren<SingleRoleAtrribute>();
		}
    }
}