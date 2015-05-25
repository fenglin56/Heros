using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class SingleContainerBox : DragComponent
    {

        public UILabel NameLabel;
        public UILabel OverlayLabel;//叠加数量
        public SingleButtonCallBack StrengthLabel;
        //public SingleButtonCallBack ViewAtbButton;
        public SingleContainerEffect ContainerEffect01;
        public SingleContainerEffect ContainerEffect02;
        public Transform CreatItemIconPoint;

        public SpriteSwith Background;

        public SingleContainerBoxType singleContainerBoxType { get; private set; }

        public ItemFielInfo itemFielInfo { get; private set; }

        public int[] m_guideBtnID ;//{ get; private set; }
        public int GuideID { get { return m_guideBtnID[0]; } }

        void Awake()
        {
            m_guideBtnID = new int[2];
            //TODO GuideBtnManager.Instance.RegGuideButton(this.gameObject, UIType.Package, SubType.PackageEquipItem, out m_guideBtnID[0]);
            ////TODO GuideBtnManager.Instance.RegGuideButton(ViewAtbButton.gameObject, UIType.PackInfo, SubType.PackageViewSkill, out m_guideBtnID[1]);
        }

        public override void Start()
        {
            base.Start();
            //ViewAtbButton.SetCallBackFuntion(OnViewAtbButtonClick);           
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            OnCustomerDestroy();
        }
        public void OnCustomerDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }

        }
        public void Init(ItemFielInfo itemFielInfo, SingleContainerBoxType singleContainerBoxType)
        {
            this.itemFielInfo = itemFielInfo;
            this.singleContainerBoxType = singleContainerBoxType;
            //ViewAtbButton.gameObject.SetActive(singleContainerBoxType == SingleContainerBoxType.Container?true:false);
            //ViewAtbButton.gameObject.SetActive(false);
            ResetBox();
            SetPanelAttribute();
        }

        void ResetBox()
        {
            this.NameLabel.SetText("");
            this.OverlayLabel.SetText("");
            this.StrengthLabel.gameObject.SetActive(false);
            this.ContainerEffect01.gameObject.SetActive(false);
            this.ContainerEffect02.gameObject.SetActive(false);
            this.CreatItemIconPoint.ClearChild();
        }
        /// <summary>
        /// 显示面板各项属性
        /// </summary>
        void SetPanelAttribute()
        {
            //int StrengLevel = itemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
            StrengthLabel.gameObject.SetActive(true);
            StrengthLabel.SetButtonText(itemFielInfo.LocalItemData._AllowLevel.ToString());
            string ItemName = LanguageTextManager.GetString(this.itemFielInfo.LocalItemData._szGoodsName);
            this.NameLabel.SetText(NGUIColor.SetTxtColor( ItemName,GetNameLabelColor(itemFielInfo)));
            
            CreatObjectToNGUI.InstantiateObj(this.itemFielInfo.LocalItemData._picPrefab,CreatItemIconPoint);
            switch ((BoxItemType)itemFielInfo.LocalItemData._GoodsClass)
            {
                case BoxItemType.Equipment:
                    if (itemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE0 != 0)//主属性1
                    {
                        ContainerEffect01.gameObject.SetActive(true);
                        ContainerEffect01.ShowEffect(itemFielInfo, 0);
                    }
                    if (itemFielInfo.equipmentEntity.EQUIP_FIELD_EFFECTBASE1 != 0)//主属性2
                    {
                        ContainerEffect02.gameObject.SetActive(true);
                        ContainerEffect02.ShowEffect(itemFielInfo, 1);
                    }
                    int StrengLevel = itemFielInfo.equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL;
                    this.NameLabel.SetText(NGUIColor.SetTxtColor( StrengLevel>0?string.Format("{0}+{1}",ItemName,StrengLevel):ItemName,GetNameLabelColor(itemFielInfo)));
                    break;
                case BoxItemType.Medicament:
                    OverlayLabel.SetText(string.Format("x{0}" ,itemFielInfo.sSyncContainerGoods_SC.byNum));
                    break;
                case BoxItemType.Materien:
                    OverlayLabel.SetText(string.Format("x{0}", itemFielInfo.sSyncContainerGoods_SC.byNum));
                    break;
            }
        }

        public void UpdatePos()
        {
            transform.localPosition = Vector3.zero;
        }

        TextColor GetNameLabelColor(ItemFielInfo itemFielInfo)
        {
            //TraceUtil.Log(string.Format("显示物品：{0}，品质：{1}",itemFielInfo.LocalItemData._goodID,itemFielInfo.LocalItemData._ColorLevel));
            TextColor labelColor = TextColor.white;
            switch (itemFielInfo.LocalItemData._ColorLevel)
            { 
                case 0:
                    //labelColor = Color.green;
                    //labelColor = new Color(47,119,25);
                    labelColor = TextColor.EquipmentGreen;
                    break;
                case 1:
                    //labelColor = Color.blue;
                    //labelColor = new Color(0,162,255);
                    labelColor = TextColor.EquipmentBlue;
                    break;
                case 2:
                    //labelColor = Color.magenta;
                    //labelColor = new Color(215,75,255);
                    labelColor = TextColor.EquipmentMagenta;
                    break;
                case 3:
                    //labelColor = Color.yellow;
                    //labelColor = new Color(255,198,0);
                    labelColor = TextColor.EquipmentYellow;
                    break;
            }
            return labelColor;
        }

        new void OnPress(bool isPressed)
        {
            if (!IsCanDrag)
                return;
            base.OnPress(isPressed);
            if (dragTool.CanDrag)
            {
                //Background.ChangeSprite(isPressed?2:1);
                //SetSelectStatus(true);
                if (isPressed)
				{
					SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
                    PackInfo_V3 packInfo = MainUIController.Instance.GetPanel(UIType.Package) as PackInfo_V3;
                    packInfo.containerPackList.UpdateSlotSelectStatus(this);
                    packInfo.heroEquiptItemList.UpdateSelectStatus(this);
                    OnViewAtbButtonClick(null);
                }
            }
        }

        public void SetDragComponentEnabel(bool flag)
        {
            base.IsCanDrag = flag;
        }

        public void SetSelectStatus(bool flag)
        {
            Background.ChangeSprite(flag ? 2 : 1);
        }

        public override void MoveBackComplete()
        {
            PackInfo_V3.Instance.CloseContainerTips();
            Background.ChangeSprite(1);
            base.MoveBackComplete();
        }

        public override void MoveToNewPointComplete()
        {
            PackInfo_V3.Instance.CloseContainerTips();
            Background.ChangeSprite(1);
        }

        void OnViewAtbButtonClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            PackInfo_V3.Instance.ContainerTipsManager.Show(this.itemFielInfo);
        }
        public override void OnDragComponetClick()
        {
            base.OnDragComponetClick();
        }
    }

    public enum BoxItemType
    {
        Equipment = 1,//装备类
        Medicament,//可使用药品类
        Materien,//不可使用材料类
    }

    public enum SingleContainerBoxType
    {
        Container,
        HeroEquip,
    }
}