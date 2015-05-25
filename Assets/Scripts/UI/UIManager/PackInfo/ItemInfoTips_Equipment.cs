using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;


namespace UI.MainUI{

	public class ItemInfoTips_Equipment : BaseItemInfoTips {

		public UILabel TitleLabel;
		public GameObject MainPropertyPrefab;
		public GameObject MainAttribute_NormalPrefab;
		public GameObject MainAttribute_CompairPrefab;
		public GameObject MainAttribute_jewelPrefab;
		public GameObject DesPanelPrefab;
        public GameObject PricePanelPrefab;
		public PassiveSkillDataBase _PassiveSkillDataBase;
        public SingleButtonCallBack Btn_PathLink;

		//public Transform Grid;

		bool IsShowEquiptItem = false;
		Jewel jewel1;//第一个孔的宝石
		Jewel jewel2;//第二个孔的宝石;
		PassiveSkillData skill;//被动技能
        void  Awake()
        {
            Btn_PathLink.SetCallBackFuntion(OnPathLinkBtnClick);
        }

        void OnPathLinkBtnClick(object obj)
        {
          
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default_Open");
            ItemInfoTipsManager.Instance.BeigenShowPathLinkPanel();
        }
		/// <summary>
		/// 初始装备信息栏，添加信息prefab
		/// </summary>
		/// <param name="itemFielInfo">Item fiel info.</param>
		public override void Init (ItemFielInfo itemFielInfo,bool isLeftPos,bool Cansale)
		{
            List<JewelInfo> jewelInfos=PlayerDataManager.Instance.GetJewelInfo((EquiptSlotType)int.Parse((itemFielInfo.LocalItemData as EquipmentData)._vectEquipLoc));
			m_AutoAddDragTool.ClearAll();
			EquipItemMainProperty mainPropertyObj = (Instantiate(MainPropertyPrefab) as GameObject).GetComponent<EquipItemMainProperty>();
			mainPropertyObj.gameObject.AddComponent<BoxCollider>().size = new Vector3(300,200,1);
			//EquipItemMainProperty mainPropertyObj = CreatObjectToNGUI.InstantiateObj(MainPropertyPrefab,Grid).GetComponent<EquipItemMainProperty>();
			//mainPropertyObj.transform.localPosition = Vector3.zero;
			mainPropertyObj.Init(itemFielInfo);
			AddObj(mainPropertyObj.gameObject);

			SingleItemTipsEffect mainAtbObj = (Instantiate(IsShowEquiptItem?MainAttribute_NormalPrefab:MainAttribute_CompairPrefab) as GameObject).GetComponent<SingleItemTipsEffect>();
			//SingleItemTipsEffect mainAtbObj = CreatObjectToNGUI.InstantiateObj(IsShowEquiptItem?MainAttribute_CompairPrefab:MainAttribute_NormalPrefab ,Grid).GetComponent<SingleItemTipsEffect>();
			mainAtbObj.Init(itemFielInfo,IsShowEquiptItem?SingleItemTipsEffect.EffectType.MainAttribute:SingleItemTipsEffect.EffectType.MainAttributeCompair);
			AddObj(mainAtbObj.gameObject);

			if(EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1MainAdd)!="+0")
			{
				SingleItemTipsEffect normalAdd = (Instantiate(MainAttribute_NormalPrefab) as GameObject).GetComponent<SingleItemTipsEffect>();
				//SingleItemTipsEffect normalAdd = CreatObjectToNGUI.InstantiateObj(MainAttribute_NormalPrefab ,Grid).GetComponent<SingleItemTipsEffect>();
                normalAdd.Init(itemFielInfo, SingleItemTipsEffect.EffectType.MainProAdd);
                AddObj(normalAdd.gameObject);
			}
			
			if(EquipItem.GetItemInfoDetail(itemFielInfo,EquipInfoType.Prop1StarAdd)!="+0")
			{
				SingleItemTipsEffect normalAdd = (Instantiate(MainAttribute_NormalPrefab) as GameObject).GetComponent<SingleItemTipsEffect>();
				//SingleItemTipsEffect normalAdd = CreatObjectToNGUI.InstantiateObj(MainAttribute_NormalPrefab ,Grid).GetComponent<SingleItemTipsEffect>();
                normalAdd.Init(itemFielInfo, SingleItemTipsEffect.EffectType.MainProAddForStar);
                AddObj(normalAdd.gameObject);
			}
            if(Cansale)
            {
                if (jewelInfos[0].JewelID!=0) //1号孔已镶嵌
    			{
                    jewel1=ItemDataManager.Instance.GetItemData(jewelInfos[0].JewelID) as Jewel;
                    skill=_PassiveSkillDataBase._dataTable.First(c=>c.SkillID==jewel1.PassiveSkill&&c.SkillLevel==jewelInfos[0].JewelLevel);
    				EquipmentAtt_Jewel att=(Instantiate(MainAttribute_jewelPrefab) as GameObject).GetComponent<EquipmentAtt_Jewel>();
    				att.init(jewel1,skill,false,jewel1._ColorLevel);
    				AddObj(att.gameObject);
    			}
                if (jewelInfos[1].JewelID!=0) //2号孔已镶嵌
    			{
                    jewel2 =ItemDataManager.Instance.GetItemData(jewelInfos[1].JewelID) as Jewel;
                    skill=_PassiveSkillDataBase._dataTable.First(c=>c.SkillID==jewel2.PassiveSkill&&c.SkillLevel==jewelInfos[1].JewelLevel);
    				EquipmentAtt_Jewel att=(Instantiate(MainAttribute_jewelPrefab) as GameObject).GetComponent<EquipmentAtt_Jewel>();
                    att.init(jewel2,skill,false,jewel2._ColorLevel);
    				AddObj(att.gameObject);
    			}
                if (jewelInfos[0].JewelID !=0 && jewelInfos[1].JewelID!=0 && jewelInfos[0].JewelID != jewelInfos[1].JewelID)
    			{
                    if(jewel1.StoneGrop!=0&&jewel1.StoneGrop==jewel2.StoneGrop)
    				{
    					 skill=_PassiveSkillDataBase._dataTable.First(c=>c.SkillID==jewel1._activePassiveSkill.skillID&&c.SkillLevel==jewel1._activePassiveSkill.skillLevel);
    					EquipmentAtt_Jewel att=(Instantiate(MainAttribute_jewelPrefab) as GameObject).GetComponent<EquipmentAtt_Jewel>();
                        att.init(jewel2,skill,true,jewel2._ColorLevel);
                        AddObj(att.gameObject);
    				}
    			}
            }
			GameObject desLabel = (Instantiate(DesPanelPrefab)as GameObject);
			desLabel.GetComponent<SingleButtonCallBack>().SetButtonText(LanguageTextManager.GetString(itemFielInfo.LocalItemData._szDesc));
			AddObj(desLabel);
            if(Cansale)
            {
            GameObject PriceLabel = (Instantiate(PricePanelPrefab)as GameObject);
            PriceLabel.GetComponent<ItemPricePanel>().SetPrice(itemFielInfo.LocalItemData._SaleCost+itemFielInfo.equipmentEntity.ITEM_FIELD_VISIBLE_COMM);
            AddObj(PriceLabel);
            }
			base.Init (itemFielInfo,isLeftPos,Cansale);
		}


		public void Show(ItemFielInfo itemFielInfo,bool isEquiptItem,bool isLeftPos,bool ShowPathLinkBtn)
		{
            ISShowing=true;
            bool CanSale=true;
            IsShowEquiptItem = isEquiptItem;
            TitleLabel.SetText(LanguageTextManager.GetString(isEquiptItem?"IDS_I3_57":"IDS_I3_58"));
           
            if(ShowPathLinkBtn)
            {
                CanSale=false;
                Init(itemFielInfo,isLeftPos,CanSale);
                Btn_PathLink.gameObject.SetActive(true);
                transform.localPosition=Vector3.zero; 
                foreach(var item in MyPanelList)
                {
                    item.alpha=1;
                }
                
            }
            else
            {
                CanSale=true;
                Init(itemFielInfo,isLeftPos, itemFielInfo.LocalItemData._TradeFlag == 1);
                Btn_PathLink.gameObject.SetActive(false);
                TweenShow();
            }
			
		
		}

	}
}