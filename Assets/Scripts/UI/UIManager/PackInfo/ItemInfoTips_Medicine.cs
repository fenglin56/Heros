using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class ItemInfoTips_Medicine : BaseItemInfoTips {

		public UILabel TitleLabel;
		public UILabel LevelLabel;
		public UILabel NumLabel;
		public SpriteSwith QuilityBackground;
		public Transform IconPos;
		public UILabel DesLabel;
        public SingleButtonCallBack Btn_PathLink;
        public ItemPricePanel pricePanel;
        void  Awake()
        {
            Btn_PathLink.SetCallBackFuntion(OnPathLinkBtnClick);
        }
        void OnPathLinkBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
            ItemInfoTipsManager.Instance.BeigenShowPathLinkPanel();
        }
		public void Show(ItemFielInfo itemFileInfo)
		{
            Btn_PathLink.gameObject.SetActive(false);
            ISShowing=true;
			base.Init(itemFileInfo,true, itemFileInfo.LocalItemData._TradeFlag == 1);
			TitleLabel.SetText(GetItemName(itemFileInfo.LocalItemData));
			LevelLabel.SetText(GetItemLevel(itemFileInfo.LocalItemData));
			NumLabel.SetText(itemFileInfo.sSyncContainerGoods_SC.byNum>=1?itemFileInfo.sSyncContainerGoods_SC.byNum.ToString():"");	
			IconPos.ClearChild();
			CreatObjectToNGUI.InstantiateObj(itemFileInfo.LocalItemData._picPrefab,IconPos);
			DesLabel.SetText(LanguageTextManager.GetString(itemFileInfo.LocalItemData._szDesc));
            if(itemFileInfo.LocalItemData._TradeFlag==1)
            {
                pricePanel.SetPrice(itemFileInfo.LocalItemData._SaleCost+itemFileInfo.equipmentEntity.ITEM_FIELD_VISIBLE_COMM);
                pricePanel.gameObject.SetActive(true);
            }
            else
            {
                pricePanel.gameObject.SetActive(false);
            }
			TweenShow();
			QuilityBackground.ChangeSprite(itemFileInfo.LocalItemData._ColorLevel+1);
		}

        public void Show(ItemData medicamentData)
        {
            transform.localPosition=Vector3.zero;
             Btn_PathLink.gameObject.SetActive(true);
            ISShowing=true;
            base.Init(null,true,false);
            TitleLabel.SetText(GetItemName(medicamentData));
            LevelLabel.SetText(GetItemLevel(medicamentData));
            NumLabel.SetText(""); 
            IconPos.ClearChild();
            CreatObjectToNGUI.InstantiateObj(medicamentData._picPrefab,IconPos);
            DesLabel.SetText(LanguageTextManager.GetString(medicamentData._szDesc));
            pricePanel.gameObject.SetActive(false);
            //TweenShow();
            QuilityBackground.ChangeSprite(medicamentData._ColorLevel+1);
        }
		string GetItemLevel(ItemData itemFileInfo)
		{
			bool canEquipt = itemFileInfo._AllowLevel<=PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
			string str = NGUIColor.SetTxtColor(itemFileInfo._AllowLevel.ToString(),canEquipt?TextColor.white:TextColor.red);
			return str;
		}

		string GetItemName(ItemData itemFileInfo)
		{
			string getStr = "";
			TextColor labelColor = TextColor.white;
            switch (itemFileInfo._ColorLevel)
			{
				case 0:
					labelColor = TextColor.EquipmentGreen;
					break;
				case 1:
					labelColor = TextColor.EquipmentBlue;
					break;
				case 2:
					labelColor = TextColor.EquipmentMagenta;
					break;
				case 3:
					labelColor = TextColor.EquipmentYellow;
					break;
			}
			getStr = NGUIColor.SetTxtColor(LanguageTextManager.GetString(itemFileInfo._szGoodsName),labelColor);
			return getStr;
		}

	}
}