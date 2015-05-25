using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    /// <summary>
    /// new
    /// </summary>
    public class BattleMagicAndHealthButtons : View
    {
        public enum BtnType {Magic,Health }

        public GameObject ButtonPrefab;

        HealthAndMagicButton MagicButton;
        HealthAndMagicButton HealthButton;

        UI.MainUI.ItemFielInfo MagicButtonItemFielInfo;
        UI.MainUI.ItemFielInfo HealthButtonItemFielInfo;

        UI.MainUI.ItemFielInfo OnUseritemFilelInfo;

        void Start()
        {
            RegisterEventHandler();
            SetMyButtons(null);
        }

        void Awake()
        { 
            for (int i = 0; i < 2; i++)
            {
                GameObject creatBtn = CreatObjectToNGUI.InstantiateObj(ButtonPrefab, transform);
                creatBtn.transform.localPosition = new Vector3(-60- 100 * i, 50 , 0);
                if (i == 0) 
                {
                    HealthButton = creatBtn.GetComponent<HealthAndMagicButton>(); 
                }
                else 
                {
                    MagicButton = creatBtn.GetComponent<HealthAndMagicButton>(); 
                }
            }
        }

        protected override void RegisterEventHandler()
        {
            AddEventHandler(EventTypeEnum.ColdWork.ToString(),ResetBtnStatus);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.UseMedicamentResult, this.UseMedicamentResult);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, SetMyButtons);
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.ColdWork.ToString(), ResetBtnStatus);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UseMedicamentResult, this.UseMedicamentResult);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, SetMyButtons);
        }


        public void SetMyButtons(object obj)
        {
            if (MagicButtonItemFielInfo != null || HealthButtonItemFielInfo != null) { return; }
            MagicButtonItemFielInfo = GetBeLinkedItem(BtnType.Magic);
            HealthButtonItemFielInfo = GetBeLinkedItem(BtnType.Health);
            SetButtonStatus(MagicButtonItemFielInfo, MagicButton);
            SetButtonStatus(HealthButtonItemFielInfo, HealthButton);
            //TraceUtil.Log("设置药品按钮：" + MagicButtonItemFielInfo.LocalItemData._goodID);
        }

        void SetButtonStatus(UI.MainUI.ItemFielInfo ItemFileInfo, BattleButton Button)
        {
            if (Button == null) return;
            Button.RecoveSprite.fillAmount = 0;
            if (ItemFileInfo == null)
            {
                Button.SetCallBackFuntion(null,null);
                Button.SetButtonIcon(null);
                Button.SetButtonText("");
            }else
            {
                Button.SetCallBackFuntion(OnButtonClick, ItemFileInfo);
                Button.SetButtonIcon(ItemFileInfo.LocalItemData._picPrefab);
                int ItemNumber = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(ItemFileInfo.LocalItemData._goodID);
                if (ItemNumber > 99) { ItemNumber = 99; }
                Button.SetButtonText(ItemNumber.ToString());
            }
        }

        void OnButtonClick(object obj)
        {
            //print("使用药品");
            OnUseritemFilelInfo = (UI.MainUI.ItemFielInfo)obj;
            //itemFilelInfo.UseButtonCallBack(null);
            UseItem(OnUseritemFilelInfo);
        }

        void UseMedicamentResult(object obj)
        {
            if (OnUseritemFilelInfo == null) return;
            var sMsgActionUseMedicamentResult_SC = (SMsgActionUseMedicamentResult_SC)obj;
            if (sMsgActionUseMedicamentResult_SC.byResult == 0) 
            { 
                return; 
            }
            switch (OnUseritemFilelInfo.LocalItemData._GoodsSubClass)
            {
                case 3://恢复生命类
                    if(HealthButton!=null)
                    HealthButton.SetMyButtonActive(false);
                    break;
                case 4://恢复真气类
                    if (MagicButton != null)
                    MagicButton.SetMyButtonActive(false);
                    break;
                default:
                    break;
            }
            this.OnUseritemFilelInfo = null;
        }

        void ResetBtnStatus(INotifyArgs inotifyArgs)
        {
            SmsgActionColdWork smsgActionColdWork = (SmsgActionColdWork)inotifyArgs;
            if (smsgActionColdWork.sMsgActionColdWorkHead_SC.lMasterID == PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity)
            {
                foreach (SMsgActionColdWork_SC child in smsgActionColdWork.sMsgActionColdWork_SCs)
                {
                    //Debug.LogWarning("收到冷却消息：" + child.byClassID);
                    if (child.byClassID == 1)
                    {
                        ColdItem((int)child.dwColdID,(int)child.dwColdTime/1000);
                    }
                }
            }
        }

        void ColdItem(int ItemID,int ColdTime)
        {
            //print("收到药品使用冷却应答");
            if (MagicButtonItemFielInfo!=null&&ItemID == MagicButtonItemFielInfo.LocalItemData._goodID)
            {
                int ItemNumber = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(MagicButtonItemFielInfo.LocalItemData._goodID);
                if (ItemNumber > 99) { ItemNumber = 99; }
                if (ItemNumber < 1)
                {
                    SetButtonStatus(null,MagicButton);
                    MagicButton = null;
                    return;
                }
                MagicButton.SetButtonText(ItemNumber.ToString());
                MagicButton.RecoverMyself(ColdTime);
            }
            else if (HealthButtonItemFielInfo!=null&&ItemID == HealthButtonItemFielInfo.LocalItemData._goodID)
            {
                int ItemNumber = UI.MainUI.ContainerInfomanager.Instance.GetItemNumber(HealthButtonItemFielInfo.LocalItemData._goodID);
                if (ItemNumber > 99) { ItemNumber = 99; }
                if (ItemNumber < 1)
                {
                    SetButtonStatus(null, HealthButton);
                    HealthButton = null;
                    return;
                }
                HealthButton.SetButtonText(ItemNumber.ToString());
                HealthButton.RecoverMyself(ColdTime);
            }
            //SetMyButtons();
        }

        UI.MainUI.ItemFielInfo GetBeLinkedItem(BtnType btnType)
        {
            foreach (UI.MainUI.ItemFielInfo itemFielInfo in UI.MainUI.ContainerInfomanager.Instance.itemFielArrayInfo)
            {
                if (itemFielInfo.LocalItemData._GoodsClass == 2 && itemFielInfo.medicamentEntity.MEDICAMENT_FIELD_BELINKED == 1)
                {
                    switch (itemFielInfo.LocalItemData._GoodsSubClass)
                    {
                        case 3://恢复生命类
                            if (btnType == BtnType.Health) { return itemFielInfo; }
                            break;
                        case 4://恢复真气类
                            if (btnType == BtnType.Magic) { return itemFielInfo; }
                            break;
                        default:
                            break;
                    }
                }
            }
            return null;
        }

        void UseItem(UI.MainUI.ItemFielInfo itemFielInfo)//使用物品,目前暂定目标都为主角
        {   
            SMsgContainerUse_CS dataStruct = new SMsgContainerUse_CS();
            dataStruct.dwContainerID1 = dataStruct.dwContainerID2 = itemFielInfo.sSyncContainerGoods_SC.SMsgContainerCSCHead.dwContainerID;
            dataStruct.byPlace = (byte)itemFielInfo.sSyncContainerGoods_SC.nPlace;
            dataStruct.uidTarget = PlayerManager.Instance.FindHeroEntityModel().EntityDataStruct.SMsg_Header.uidEntity;
            NetServiceManager.Instance.ContainerService.SendContainerUse(dataStruct);
        }


        public void SetMyButtonsColliderActive(bool flag)
        {
            if (MagicButton != null)
                MagicButton.Active = flag;
            if (HealthButton != null)
                HealthButton.Active = flag;
        }

    }
}
