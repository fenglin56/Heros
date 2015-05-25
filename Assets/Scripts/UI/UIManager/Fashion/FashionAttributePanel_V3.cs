using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class FashionAttributePanel_V3 : BaseTweenShowPanel
    {


        //public SpriteSwith TitleSpriteSwith;
        //public Transform CreatTitleIconPoint;
        public Transform NamePos;
        public UILabel LevelLabel;
		public UILabel ForceLabel;
        public SingleFashionAtbPanel_V3[] SingleAtbPanelList;

        private FashionPanel_V3 MyParent;

        public void Show(ItemData selectFashionData, FashionPanel_V3 fashionPanel)
        {
            //CreatTitleIconPoint.ClearChild();
            //CreatObjectToNGUI.InstantiateObj(selectFashionData._picPrefab, CreatTitleIconPoint);
            //TitleSpriteSwith.ChangeSprite(seleteFashionBtn.DataToSpriteID(seleteFashionBtn.MyFashionData));
            SingleAtbPanelList.ApplyAllItem(P => P.Close());
            this.MyParent = fashionPanel;
			NamePos.ClearChild();
			NamePos.InstantiateNGUIObj(selectFashionData.DisplayBig_prefab);
			this.LevelLabel.SetText(selectFashionData._Level);
            EquipmentData itemdata = selectFashionData as EquipmentData;
            string[] neweffects = itemdata._vectEffects.Split('|');
            string[] currentEffects = MyParent.CurrentMaxFashionData == null ? null : MyParent.CurrentMaxFashionData._vectEffects.Split('|');
			int forceNum = 0;
            for (int i = 0; i < neweffects.Length; i++)
            {
                bool IsUp = false;
                string currentEffect = currentEffects == null ? string.Empty : currentEffects.FirstOrDefault(P => P.Split('+')[0] == neweffects[i].Split('+')[0]);
                IsUp = string.IsNullOrEmpty(currentEffect);
                string[] newEffectsStr = neweffects[i].Split('+');
                EffectData effectdata = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == newEffectsStr[0]);
                string newEffectName =LanguageTextManager.GetString(effectdata.IDS);
                int newAddNumber = HeroAttributeScale.GetScaleAttribute(newEffectName,int.Parse(newEffectsStr[1]));
                //string[] currentEffectsStr = currentEffects.FirstOrDefault(P=>P);

                int currentNumber = 0;
                if (!IsUp)
                {
                    string[] curef = currentEffect.Split('+');
                    var currentEffectData = ItemDataManager.Instance.EffectDatas._effects.First(P => P.m_SzName == curef[0]);
                    currentNumber = HeroAttributeScale.GetScaleAttribute(currentEffectData, int.Parse(curef[1]));
                    //if (newAddNumber > currentNumber)
                    //{
                    //    IsUp = true;
                    //}
                    //else
                    //{
                    //    newAddNumber = currentNumber;
                    //}
                }
                SingleAtbPanelList[i].Show(effectdata, currentNumber, newAddNumber);
				forceNum+=newAddNumber;
            }
			int newAtk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, forceNum);
			ForceLabel.SetText(newAtk);
        }

		[ContextMenu("GetSingleFashionAtbList")]
		void GetSingleFashionAtbList()
		{
			SingleAtbPanelList = transform.GetComponentsInChildren<SingleFashionAtbPanel_V3>();
		}
    }
}