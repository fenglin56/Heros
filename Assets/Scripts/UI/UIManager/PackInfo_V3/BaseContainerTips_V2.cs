using UnityEngine;
using System.Collections;


namespace UI.MainUI
{

    public class BaseContainerTips_V2 : MonoBehaviour
    {

        public GameObject SingelContainerBoxPrefab;

        public Transform CreatContainerBoxPoint;

        public int[] m_guideBtnID;

        private SingleContainerBox singleContainerBox;

        private ItemFielInfo itemFielInfo;

        public virtual void Show(ItemFielInfo itemFielInfo)
        {
            this.itemFielInfo = itemFielInfo;
            if (singleContainerBox == null)
            {
                singleContainerBox = CreatObjectToNGUI.InstantiateObj(SingelContainerBoxPrefab, CreatContainerBoxPoint).GetComponent<SingleContainerBox>();
                singleContainerBox.collider.enabled = false;
            }
            singleContainerBox.Init(itemFielInfo,SingleContainerBoxType.HeroEquip);
            transform.localPosition = Vector3.zero;
        }
        /// <summary>
        /// 注册出售和取消两个按钮，因为这个两个按钮在物品，装备，药品上都是相同的。为方便策划配表，固定两个ID为{ 181900, 181901 }; 
        /// </summary>
        /// <param name="sellBtn"></param>
        /// <param name="cancelBtn"></param>
        public void RegBtn(GameObject sellBtn,GameObject cancelBtn)
        {
            m_guideBtnID = new int[] { 181900, 181901 };  
            //TODO GuideBtnManager.Instance.RegGuideButton(sellBtn, m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(cancelBtn, m_guideBtnID[1]);
        }
        public void UnRegBtn()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(181900);  //出售
            //TODO GuideBtnManager.Instance.DelGuideButton(181901);  //取消
        }
        public void InitItemFileInfo(ItemFielInfo itemFielInfo)
        {
            this.itemFielInfo = itemFielInfo;
        }

        public virtual void Close()
        {
            //TraceUtil.Log("CloseTips");
            transform.localPosition = new Vector3(0, 0, -1000);
        }


        public void DiscardItems() //丢弃物品
        {
            //string Msg = string.Format(LanguageTextManager.GetString("IDS_H1_5"),LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName), itemFielInfo.sSyncContainerGoods_SC.byNum);
            ////"丢弃后会失去\n" + itemFielInfo.LocalItemData._szGoodsName + "x" + itemFielInfo.sSyncContainerGoods_SC.byNum;
            //MessageBox.Instance.Show(5, "", Msg, LanguageTextManager.GetString("IDS_H2_55"), LanguageTextManager.GetString("IDS_H2_28"), SendContainerDiscard, null);
            int money = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;
            if (money + itemFielInfo.LocalItemData._SaleCost * itemFielInfo.sSyncContainerGoods_SC.byNum > 999999999)//携带铜币是否达到上限
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_211"), LanguageTextManager.GetString("IDS_H2_55"), null);
                return;
            }
            if (itemFielInfo.LocalItemData._ColorLevel > 1)//品质是否普通
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_212"), LanguageTextManager.GetString("IDS_H2_61"),
                    LanguageTextManager.GetString("IDS_H2_28"), SendContainerDiscard, null);
                return;
            }
            if (itemFielInfo.equipmentEntity.EQUIP_FIELD_START_LEVEL != 0 || itemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL != 0)
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_213"), LanguageTextManager.GetString("IDS_H2_61"),
                    LanguageTextManager.GetString("IDS_H2_28"), SendContainerDiscard, null);
                return;
            }
            SendContainerDiscard();
        }


        public virtual void SendContainerDiscard()//向服务器发送丢弃物品
        {
            //ContainerTipsManager.Instance.CloaseMyLastPanel();
//            SMsgContainerDoff_CS dataStruct = new SMsgContainerDoff_CS();
//            dataStruct.dwSrcContainerID1 = dataStruct.dwSrcContainerID2 = itemFielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
//            dataStruct.bySrcPlace = (byte)itemFielInfo.sSyncContainerGoods_SC.nPlace;
//            NetServiceManager.Instance.ContainerService.SendContainerDoff(dataStruct);
            TraceUtil.Log("向服务器发送出售物品");
            Close();
        }


        protected void UseItems(object obj)//使用物品
        {
            EquipButtonType equipButtonType = GetEquipItemButtonStatus();
            switch (equipButtonType)
            {
                case EquipButtonType.CanEquip:
                    Close();
                    SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
                    dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
                    dataStruct.byPlace = (byte)itemFielInfo.sSyncContainerGoods_SC.nPlace;
                    dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
                    NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
                    break;
                case EquipButtonType.ProfesionNotEnough:
                    MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_48"), LanguageTextManager.GetString("IDS_H2_55"));
                    //SetButtonGray(CancelButtonScript, LanguageTextManager.GetString("IDS_H2_48"));
                    return;
                case EquipButtonType.LVNotEnough:
                    MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H2_6"), LanguageTextManager.GetString("IDS_H2_55"));
                    //SetButtonGray(CancelButtonScript, LanguageTextManager.GetString("IDS_H2_6"));
                    return;
                default:
                    break;
            }
        }

        public EquipButtonType GetEquipItemButtonStatus()
        {
            //print("EquipItemChild");
            int ItemEquipLevel = itemFielInfo.LocalItemData._AllowLevel;
            int HeroLevel = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
            string[] ItemVocation = itemFielInfo.LocalItemData._AllowProfession.Split('+');
            int HeroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            if (HeroLevel >= ItemEquipLevel)
            {
                foreach (string child in ItemVocation)
                {
                    int Vocation = int.Parse(child);
                    if (Vocation == 5 || HeroVocation == Vocation)
                    {//装备
                        return EquipButtonType.CanEquip;
                    }
                }
                //职业不符
                return EquipButtonType.ProfesionNotEnough;
            }
            else
            {
                //等级不符
                return EquipButtonType.LVNotEnough;
            }
        }

    }
}