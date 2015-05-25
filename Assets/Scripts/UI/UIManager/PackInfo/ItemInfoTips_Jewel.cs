using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{
public class ItemInfoTips_Jewel : BaseItemInfoTips {

		public UILabel JewelNameLable;
		public SpriteSwith JewelType;
		public UILabel JewelPosition;
		public UILabel JewelLevel;
		public GameObject AttributeProfab;
		public Transform IconPos;
		public Transform Grid;
		public UISlider ProgressSlider;
		public UILabel ProgressText;
        public UILabel PanelTitleLable_des;
        public UILabel TypeLable_des;
        public UILabel PositionLable_des;
        public UILabel LevelLable_des;
        public SpriteSwith Background_spr;
        public SingleButtonCallBack Btn_PathLink;
		private int m_curGoodID;
		private ItemFielInfo m_curItemFielInfo;
		private Jewel jewel;
        GameObject Attribute_go;
        GameObject Suit_go;
		public static Dictionary<string,string> JewelPossionDic=new Dictionary<string, string>()
		{
			{"1","武器"},
			{"3","头饰"},
			{"4","衣服"},
			{"5","靴子"},
			{"6","饰品"}
		
		};
	   void Awake()
        {
            PanelTitleLable_des.text=LanguageTextManager.GetString("IDS_I9_5");
            TypeLable_des.text=(LanguageTextManager.GetString("IDS_I9_2"));
            PositionLable_des.text=(LanguageTextManager.GetString("IDS_I9_3"));
            LevelLable_des.text=(LanguageTextManager.GetString("IDS_I9_4"));
            Btn_PathLink.SetCallBackFuntion(OnPathLinkBtnClick);

        }
        void OnPathLinkBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
            ItemInfoTipsManager.Instance.BeigenShowPathLinkPanel();
        }
		/// <summary>
		/// Gets the jewel posion.
		/// </summary>
		/// <returns>The jewel posion.</returns>
		/// <param name="iarray">位置数组.</param>
		/// <param name="spaceCharacter">间隔符</param>
		public static string getJewelPosion(string[] iarray,string spaceCharacter)
		{
			string s="";
			for(int i=0;i<iarray.Length;i++)
			{
				s=s+JewelPossionDic[iarray[i]];
				if(i<iarray.Length-1)
				{

					s+=spaceCharacter;
				}

			}
			return s;
		}

		/// <summary>
		/// 初始装备信息栏，添加信息prefab
		/// </summary>
		/// <param name="itemFielInfo">Item fiel info.</param>

		public void Show(ItemFielInfo itemFielInfo,bool isEquiptItem,bool isLeftPos)
		{
            Btn_PathLink.gameObject.SetActive(false);
            ISShowing=true;
			this.m_curGoodID = itemFielInfo.LocalItemData._goodID;
			this.m_curItemFielInfo = itemFielInfo;
			jewel = ItemDataManager.Instance.GetItemData(m_curGoodID) as Jewel;

			SetTitleColor(jewel._ColorLevel,LanguageTextManager.GetString(itemFielInfo.LocalItemData._szGoodsName));
            Background_spr.ChangeSprite(jewel._ColorLevel+1);
			JewelType.ChangeSprite (jewel.StoneType+1);
			JewelLevel.SetText(itemFielInfo.materiel.ESTORE_FIELD_LEVEL);
			ItemPriceLabel.SetText (jewel._SaleCost);
			JewelPosition.SetText(ItemInfoTips_Jewel.getJewelPosion(jewel.StonePosition,"、"));
			if (itemFielInfo.materiel.ESTORE_FIELD_LEVEL == jewel.MaxLevel) 
			{
						
				ProgressText.SetText("[fe768b]满级[-]")	;	
				ProgressSlider.sliderValue=1;
			} 
			else 
			{
				ProgressText.SetText (itemFielInfo.materiel.ESTORE_FIELD_EXP + "/" + jewel.StoneExp [itemFielInfo.materiel.ESTORE_FIELD_LEVEL-1]);				
				ProgressSlider.sliderValue = (float)itemFielInfo.materiel.ESTORE_FIELD_EXP / (float)jewel.StoneExp [itemFielInfo.materiel.ESTORE_FIELD_LEVEL-1];
						
			}
			IconPos.ClearChild();
			CreatObjectToNGUI.InstantiateObj(itemFielInfo.LocalItemData._picPrefab,IconPos);
			CreatAttribute (itemFielInfo);
		    TweenShow();
		}

        public void Show(Jewel jewel,bool isEquiptItem,bool isLeftPos)
        {
            Btn_PathLink.gameObject.SetActive(true);
            ISShowing=true;
            this.m_curGoodID = jewel._goodID;
          
            SetTitleColor(jewel._ColorLevel,LanguageTextManager.GetString(jewel._szGoodsName));
            Background_spr.ChangeSprite(jewel._ColorLevel+1);
            JewelType.ChangeSprite (jewel.StoneType+1);
            JewelLevel.SetText(1);
            ItemPriceLabel.SetText (jewel._SaleCost);
            JewelPosition.SetText(ItemInfoTips_Jewel.getJewelPosion(jewel.StonePosition,"、"));
         
                ProgressText.SetText (jewel.StoneStartExp + "/" + jewel.StoneExp [0]);              
            ProgressSlider.sliderValue = (float)jewel.StoneStartExp / (float)jewel.StoneExp [0];
            IconPos.ClearChild();
            CreatObjectToNGUI.InstantiateObj(jewel._picPrefab,IconPos);
            CreatAttribute (jewel);
            //TweenShow();
            transform.localPosition=Vector3.zero;
        }
        /// <summary>
		///创建技能属性栏
		/// </summary>
		void CreatAttribute(ItemFielInfo itemfielInfo)
		{

            if(Attribute_go==null)
            {
                Attribute_go = Instantiate (AttributeProfab) as GameObject;
                AddObj (Attribute_go);
            }
			Attribute_go.GetComponent<JewelAttriture> ().Init (itemfielInfo,false);
			
            var ItemId = itemfielInfo.LocalItemData._goodID;
            var jewel = ItemDataManager.Instance.GetItemData(ItemId) as Jewel;
            if(jewel.StoneGrop!=0)
            {
                if(Suit_go==null)
                {
                    Suit_go = Instantiate (AttributeProfab) as GameObject;
                    AddObj (Suit_go);
                }
			Suit_go.GetComponent<JewelAttriture> ().Init (itemfielInfo,true);
			
            }

		}

        /// <summary>
        ///创建技能属性栏
        /// </summary>
        void CreatAttribute(Jewel jewel)
        {
            if(Attribute_go==null)
            {
            Attribute_go = Instantiate (AttributeProfab) as GameObject;
                AddObj (Attribute_go);
            }
            Attribute_go.GetComponent<JewelAttriture> ().Init (jewel,false);
         
            var ItemId = jewel._goodID;

            if(jewel.StoneGrop!=0)
            {
                if(Suit_go==null)
                {
                    Suit_go = Instantiate (AttributeProfab) as GameObject;
                    AddObj (Suit_go);

                }
                Suit_go.GetComponent<JewelAttriture> ().Init (jewel,true);
               
            }
            
        }
		void SetTitleColor(int type,string text)
		{

            JewelNameLable.SetText(NGUIColor.SetTxtColor(text,(TextColor)type));

		}
}
}
