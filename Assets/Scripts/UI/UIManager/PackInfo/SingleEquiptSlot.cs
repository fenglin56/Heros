using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UI.MainUI
{

    public class SingleEquiptSlot : MonoBehaviour
    {
        public EquiptSlotType m_EquiptSlotType;
        public UISprite SelectStatus;
        public SpriteSwith QualityBackground;
        public Transform IconPoint;
		public GameObject SelectStatusObj;
        public GameObject Background;

        public SpriteSwith Sps_star;
        public SingleButtonCallBack Go_Strength;

        public ItemFielInfo MyItem { get; private set; }
		public bool IsEmpty { get { return MyItem==null; } }
        public HeroEquiptPanel MyParent { get; private set; }

        public void Init(List<ItemFielInfo> goodsList,HeroEquiptPanel myParent)
        {
			this.MyParent = myParent;
			if(m_EquiptSlotType == EquiptSlotType.Medicine)//如果为药品槽
			{
				MyItem = ContainerInfomanager.Instance.GetMedicineItemFileInfo();
			}
            else{
				MyItem = goodsList.FirstOrDefault(P => P.sSyncContainerGoods_SC.nPlace == (int)m_EquiptSlotType);
			}
			if (this.MyItem != null)
            {
                Background.SetActive(false);
                InitItem();
            }
            else
            {
                Sps_star.gameObject.SetActive(false);
                Go_Strength.gameObject.SetActive(false);
                Background.SetActive(true);
                ClearUpSlot();
            }

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            switch (m_EquiptSlotType)
            {
                case EquiptSlotType.Heard:
                    gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_Heard);
                    break;
                case EquiptSlotType.Weapon:
                    gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_Weapon);
                    break;
                case EquiptSlotType.Shoes:
                    gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_Shoes);
                    break;
                case EquiptSlotType.Body:
                    gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_Body);
                    break;
                case EquiptSlotType.Accessories:
                    gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_Accessories);
                    break;
                case EquiptSlotType.Medicine:
                    gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_HeroEquiptPanel_Medicine);
                    break;
            }
        }
        void InitItem()
        {
            IconPoint.ClearChild();
            CreatObjectToNGUI.InstantiateObj(MyItem.LocalItemData._picPrefab,IconPoint);
            QualityBackground.ChangeSprite(MyItem.LocalItemData._ColorLevel+2);
            int StrengthLevel=PlayerDataManager.Instance.GetEquipmentStrengthLevel((EquiptSlotType)MyItem.sSyncContainerGoods_SC.nPlace);
            int StarLevel=PlayerDataManager.Instance.GetEquipmentStarLevel((EquiptSlotType)MyItem.sSyncContainerGoods_SC.nPlace);
            if(StarLevel>0)
            {
                int par= (StarLevel-1)/10+1;
                Sps_star.ChangeSprite(par);
                Sps_star.gameObject.SetActive(true);
            }
            else
            {
                Sps_star.gameObject.SetActive(false);
            }

            if(StrengthLevel>0)
            {
                Go_Strength.SetButtonText("+"+StrengthLevel);
                Go_Strength.gameObject.SetActive(true);
            }
            else
            {
                Go_Strength.gameObject.SetActive(false);
            }
        }

        void ClearUpSlot()
        {
            IconPoint.ClearChild();
            MyItem = null;
            QualityBackground.ChangeSprite(1);
        }

		public void SetSelectStatus(SingleEquiptSlot selectSlot)
		{
			SelectStatusObj.gameObject.SetActive(selectSlot!=null&&selectSlot==this);
		}

        void OnClick()
        {
            if (IsEmpty)
                return;
            MyParent.OnItemClick(this);
        }

    }
}