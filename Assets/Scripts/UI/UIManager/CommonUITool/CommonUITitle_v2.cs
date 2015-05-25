using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class CommonUITitle_v2 : View
    {
        public Transform VIPIconPoint;
        public Transform HeroIconPoint;
        public UILabel Level_lable;
        public UILabel Force_lable;
        public UISlider Exp_sloder;
        public SpriteSwith VocationIcon_SpriteSwith;
        public SpriteSwith VocationName_SpriteSwith;
        private bool IsResidentUI;

        void Awake()
        {
            RegisterEventHandler();
            SetHeroData(); 
        }
       
        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        }

        private void UpdateViaNotify(INotifyArgs inotifyArgs)//设置各种属性
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                SetHeroData();  
            }
        }

        private void SetHeroData()
        {
           
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
                
                int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
                VocationIcon_SpriteSwith.ChangeSprite(vocationID);
                VocationName_SpriteSwith.ChangeSprite(vocationID);

                int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
                var resData= CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_Town.FirstOrDefault(P=>P.VocationID == vocationID&&P.FashionID == fashionID);
                if (resData == null) { TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"找不到角色头像,fashionID:" + fashionID); }
                HeroIconPoint.ClearChild();
                CreatObjectToNGUI.InstantiateObj(resData.IconPrefab,HeroIconPoint) ;   
              // HeroIconPoint.spriteName = resData.ResName;
                
                int newAtk = PlayerDataManager.Instance.GetHeroForce();
                Force_lable.SetText(newAtk);
                 Level_lable.SetText(m_HeroDataModel.UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL);
				 Exp_sloder.sliderValue=(float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_EXP/(float)m_HeroDataModel.PlayerValues.PLAYER_FIELD_NEXT_LEVEL_EXP;
                 VIPIconPoint.ClearChild();
                 CreatObjectToNGUI.InstantiateObj(PlayerDataManager.Instance.GetCurrentVipEmblemPrefab(),VIPIconPoint);
                //HeadIconSprite.ChangeSprite(m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION);

        }


    }
}