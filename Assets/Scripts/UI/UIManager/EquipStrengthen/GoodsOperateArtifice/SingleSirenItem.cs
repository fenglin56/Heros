using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class SingleSirenItem : MonoBehaviour
    {
        public Transform CreatIconTransform;
        public UILabel DesLabel;
        public UILabel NameLabel;
        public MaterielData M_Data { get; private set; }
        public ShopConfigData ShopData { get; private set; }

        private int m_guideID = 0;

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(this.gameObject, UIType.EquipStrengthen, SubType.EquipStrenOperateItem, out m_guideID);
        }

        void OnDestroy()
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideID);
        }

        public void Show()
        {
            int ShortcutItem_RefiningID = CommonDefineManager.Instance.CommonDefineFile._dataTable.ShortcutItem_Refining;
            ShopData = ShopDataManager.Instance.GetShopData(ShortcutItem_RefiningID);
            M_Data = ItemDataManager.Instance.GetItemData(ShopData.GoodsID) as MaterielData;// 获取妖女内丹
            CreatIconTransform.ClearChild();
            CreatObjectToNGUI.InstantiateObj(M_Data._picPrefab, CreatIconTransform);
            NameLabel.SetText(LanguageTextManager.GetString(M_Data._szGoodsName));
            DesLabel.SetText(LanguageTextManager.GetString(M_Data._szDesc));
            //List<ItemFielInfo> SirenItemList = ContainerInfomanager.Instance.itemFielArrayInfo.FindAll(P => P.LocalItemData._goodID == ShortcutItem_RefiningID);
            //int maxNum = 0;
            //SirenItemList.ApplyAllItem(P=>maxNum+=P.sSyncContainerGoods_SC.byNum);
            //NumberLabel.SetText(maxNum);
        }

        public void OnClick()
        {
            UIEventManager.Instance.TriggerUIEvent(UIEventType.QuickPurchase, ShopData._shopGoodsID);
        }

    }
}