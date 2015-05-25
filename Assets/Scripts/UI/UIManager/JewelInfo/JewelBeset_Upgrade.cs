using UnityEngine;
using System.Collections;
using System.Linq;
namespace UI.MainUI
{
    public class JewelBeset_Upgrade : JewelBesetUIBase {
		public UILabel JewelName;
		public UILabel JewelPosition;
		public UILabel JewelLevel;
		public GameObject AttContent_prefab;
		public Transform CurrentAttPoint,NextAttPoint;
		private AttributeContent CurrentAtt,NextrAtt;
        public UILabel NoneAtt;
		public UILabel Progress_text;
		public UISlider Progress_slider;
		public GameObject FullLevel;
		public Transform IconPoint;
		public UILabel NoneJewel;
        public GameObject AttributeGO;
		public GameObject Effect_Prefab,SwallowEff2_profab;
        private GameObject Effect_Show,SwallowEff2;
		public Transform Effect_point,SwallowEff2_piont;

		private PassiveSkillData currentskill,nextSkill;

		void Awake()
		{
			CurrentAtt= CreatObjectToNGUI.InstantiateObj(AttContent_prefab,CurrentAttPoint).GetComponent<AttributeContent>();
			NextrAtt=CreatObjectToNGUI.InstantiateObj(AttContent_prefab,NextAttPoint).GetComponent<AttributeContent>();
            Effect_Show=CreatObjectToNGUI.InstantiateObj(Effect_Prefab,Effect_point);
			SwallowEff2=CreatObjectToNGUI.InstantiateObj(SwallowEff2_profab,SwallowEff2_piont);
            //UILabel lable = NoneJewel.GetComponentInChildren<UILabel>();
            NoneJewel.SetText(LanguageTextManager.GetString("IDS_I9_16"));
            NoneAtt.SetText(LanguageTextManager.GetString("IDS_I9_30"));
			SwallowEff2.SetActive(false);
		}

//		public IEnumerator ShowSwallow2()
//		{
//			SwallowEff2.SetActive(true);
//			yield return new WaitForSeconds(1);
//			SwallowEff2.SetActive(false);
//		}
	    public void  Init(ItemFielInfo itemFileInfo)
		{
			if(itemFileInfo!=null)
			{
			HideOrShow(true);
			Jewel jewel=ItemDataManager.Instance.GetItemData(itemFileInfo.LocalItemData._goodID) as Jewel;
            JewelName.SetText(NGUIColor.SetTxtColor( LanguageTextManager.GetString(itemFileInfo.LocalItemData._szGoodsName),(TextColor)itemFileInfo.LocalItemData._ColorLevel));
			JewelPosition.SetText(ItemInfoTips_Jewel.getJewelPosion(jewel.StonePosition,"、"));
			JewelLevel.SetText(itemFileInfo.materiel.ESTORE_FIELD_LEVEL);
			IconPoint.ClearChild();
			CreatObjectToNGUI.InstantiateObj(jewel._picPrefab,IconPoint);
			if(itemFileInfo.materiel.ESTORE_FIELD_LEVEL<jewel.MaxLevel)
			{
				Progress_text.gameObject.SetActive(true);
				FullLevel.SetActive(false);
             
			Progress_text.SetText(itemFileInfo.materiel.ESTORE_FIELD_EXP+"/"+jewel.StoneExp[itemFileInfo.materiel.ESTORE_FIELD_LEVEL-1]);
			Progress_slider.sliderValue=(float)itemFileInfo.materiel.ESTORE_FIELD_EXP/(float)jewel.StoneExp[itemFileInfo.materiel.ESTORE_FIELD_LEVEL-1];
			}
			else
			{
				Progress_text.gameObject.SetActive(false);
                Progress_slider.sliderValue=1;
				FullLevel.SetActive(true);
			}
			//服务器bug器魂初始等级为0为了调试这里+1
			currentskill=JewelBesetManager.GetInstance().passiveSkillDataBase._dataTable.First(c=>c.SkillID==jewel.PassiveSkill&&c.SkillLevel==itemFileInfo.materiel.ESTORE_FIELD_LEVEL);
            CurrentAtt.Init(currentskill);
                if(itemFileInfo.materiel.ESTORE_FIELD_LEVEL<jewel.MaxLevel)
                {
                   NextrAtt.gameObject.SetActive(true);
			       nextSkill=JewelBesetManager.GetInstance().passiveSkillDataBase._dataTable.First(c=>c.SkillID==jewel.PassiveSkill&&c.SkillLevel==(itemFileInfo.materiel.ESTORE_FIELD_LEVEL+1));
                   NextrAtt.Init(nextSkill);
                   NoneAtt.gameObject.SetActive(false);
                }
                else
                {
                    NextrAtt.gameObject.SetActive(false);
                    NoneAtt.gameObject.SetActive(true);
                }
			
			
		  }
			else
			{
				HideOrShow(false);
			}
		}
	void HideOrShow(bool isshow)
		{
			JewelName.gameObject.SetActive(isshow);
			JewelPosition.gameObject.SetActive(isshow);
			JewelLevel.gameObject.SetActive(isshow);
			Progress_text.gameObject.SetActive(isshow);
			Progress_slider.gameObject.SetActive(isshow);
		    IconPoint.gameObject.SetActive(isshow);
            AttributeGO.SetActive(isshow);
			NoneJewel.transform.parent.gameObject.SetActive(!isshow);
            Effect_Show.SetActive(isshow);
		}

}
}
