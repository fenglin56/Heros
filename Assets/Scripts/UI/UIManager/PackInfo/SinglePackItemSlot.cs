using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class SinglePackItemSlot : MonoBehaviour {

		public enum ItemStatus{PackItem,Sell}

	    public UISprite BestIcon;
	    public UISprite SelectStatus;
	    public SpriteSwith QualityBackground;
		//public SpriteSwith EquiptStartSprite;
	    public SingleButtonCallBack ItemNumLabel;
	    public SingleButtonCallBack NeedLabel;//条件不足提示
	  //  public SingleButtonCallBack StrengthenLevelLabel;//强化等级
		public Transform CreatItemIconPoint;
        public GameObject BesetJewelEff_prefab;
		private GameObject BesetJewelEff;
		public ItemFielInfo MyItemFileInfo{get;private set;}
		public ItemStatus MyItemStatus{get;private set;}
		public bool IsEmpty{get{return MyItemFileInfo == null;}}
		public bool IsSelect{get{return SelectStatus.gameObject.activeSelf;}}
		ButtonCallBack ClickCallBack;

        void Awake()
        {
            BesetJewelEff=CreatObjectToNGUI.InstantiateObj(BesetJewelEff_prefab,transform);
            BesetJewelEff.SetActive(false);

            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            //ItemNumLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Equip_AmountConfirm);
            //NeedLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Equip_AmountCancel);
            //StrengthenLevelLabel.gameObject.RegisterBtnMappingId(UIType.Package, BtnMapId_Sub.Package_Equip_AmountCancel);            
        }
		public void Init(ItemFielInfo myItemFileInfo,bool isBestItem,ItemStatus itemStatus,ButtonCallBack clickCallBack)
		{
			this.MyItemFileInfo = myItemFileInfo;
			this.MyItemStatus = itemStatus;
			this.ClickCallBack = clickCallBack;
			EmptyMySelf();
			if(myItemFileInfo != null)
			{
				BestIcon.gameObject.SetActive(itemStatus == ItemStatus.PackItem&&isBestItem);
				UpdateStatu();                
			}
		}

		void EmptyMySelf()
		{
			BestIcon.gameObject.SetActive(false);
			SelectStatus.gameObject.SetActive(false);
			QualityBackground.ChangeSprite(1);
			ItemNumLabel.gameObject.SetActive(false);
			NeedLabel.gameObject.SetActive(false);
//			StrengthenLevelLabel.gameObject.SetActive(false);
//			EquiptStartSprite.ChangeSprite(0);
		}

		void UpdateStatu()
		{
			CreatObjectToNGUI.InstantiateObj(MyItemFileInfo.LocalItemData._picPrefab,CreatItemIconPoint);
			QualityBackground.gameObject.SetActive(true);
			QualityBackground.ChangeSprite(MyItemFileInfo.LocalItemData._ColorLevel+2);

			if(MyItemFileInfo.LocalItemData._GoodsClass ==1)
            {
				switch (GetBestItem.GetEquipItemStatus(MyItemFileInfo))
				{
				case EquipButtonType.CanEquip:
//					var strengLevel = EquipItem.GetItemInfoDetail(MyItemFileInfo,EquipInfoType.EquipStrenLevel);
//					if(strengLevel!="+0")
//					{
//						StrengthenLevelLabel.gameObject.SetActive(true);
//						StrengthenLevelLabel.SetButtonText(strengLevel);
//						
//					}
//                        var StartLevel= EquipItem.GetItemInfoDetail(MyItemFileInfo,EquipInfoType.EquipStarLevel);
//                        if(StartLevel!="0")
//                        {
//
//                            EquiptStartSprite.ChangeSprite(int.Parse(StartLevel));
//                        }
					break;
                    case EquipButtonType.LVNotEnough:

                        NeedLabel.gameObject.SetActive(true);
                        NeedLabel.SetButtonText(string.Format(LanguageTextManager.GetString("IDS_I1_22"),MyItemFileInfo.LocalItemData._AllowLevel));
                        break;
                    case EquipButtonType.ProfesionNotEnough:
                        NeedLabel.gameObject.SetActive(true);
                        NeedLabel.SetButtonText(GetVocation());
                        break;
				}
			}

			if(MyItemFileInfo.sSyncContainerGoods_SC.byNum>1)
			{
				ItemNumLabel.gameObject.SetActive(true);
				ItemNumLabel.SetButtonText(MyItemFileInfo.sSyncContainerGoods_SC.byNum.ToString());
			}
//            //第一个判断是“是装备”，第二个判断是“两个孔都有器魂”，第三个判断是“第一个器魂与第二个器魂不是相同id的”，第四/五个判断是“器魂套装id不等于0且第一个套装id等于第二个套装id”，
//            if(MyItemFileInfo.LocalItemData._GoodsClass==1&&(MyItemFileInfo.GetIfBesetJewel(1)&&MyItemFileInfo.GetIfBesetJewel(2))&&MyItemFileInfo.GetJewelIndex(1)!=MyItemFileInfo.GetJewelIndex(2)&& MyItemFileInfo.GetJewel(1).StoneGrop!=0&&MyItemFileInfo.GetJewel(1).StoneGrop==MyItemFileInfo.GetJewel(2).StoneGrop)
//			{
//				ShowBesetjewelEff();
//			}
//			else
//			{
//				HideBesetjewelEff();
//			}
			if(MyItemFileInfo.LocalItemData._GoodsClass==3&&MyItemFileInfo.LocalItemData._GoodsSubClass==3)//器魂
			{
				NeedLabel.gameObject.SetActive(true);
				NeedLabel.SetButtonText("Lv."+MyItemFileInfo.materiel.ESTORE_FIELD_LEVEL);//临时
			}
		}
		void HideBesetjewelEff()
		{
			BesetJewelEff.SetActive(false);
		}
		void ShowBesetjewelEff()
		{
			BesetJewelEff.SetActive(true);
		}
		void OnClick()
		{
			if(!IsEmpty&&ClickCallBack!=null)
			{
				ClickCallBack(this.MyItemFileInfo);
				SoundManager.Instance.PlaySoundEffect("Sound_Button_PackageItemChoice");
			}
		}

		public void SetSelectStatus(ItemFielInfo itemFielInfo)
		{
			SetSelectStatus(MyItemFileInfo == itemFielInfo);
		}
		public void SetSelectStatus(bool flag)
		{
			SelectStatus.gameObject.SetActive(flag);
		}

		string GetVocation()
		{
			string getVocation = "";
			string[] vocation =  MyItemFileInfo.LocalItemData._AllowProfession.Split('+');
			switch(int.Parse(vocation[0]))
			{
			case 1://剑客
				getVocation = LanguageTextManager.GetString("IDS_I1_20");
				break;
			case 4://刺客
				getVocation = LanguageTextManager.GetString("IDS_I1_21");
				break;
			}
			return getVocation;
		}

	}
}