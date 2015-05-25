using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Linq;

namespace UI.Forging
{
    public class MainAttribute : MonoBehaviour {
        public UISprite Effect1Icon;
        public UISprite Effect2Icon;
        public UILabel Effect1NameLabel;
        public UILabel Effect1NumLabel;
        public UILabel Effect2NameLabel;
        public UILabel Effect2NumLabel;
        public void Init(ForgeRecipeData data)
        {
            ItemData itemData=ItemDataManager.Instance.GetItemData(data.ForgeEquipmentID);
            string[] neweffects=((EquipmentData)itemData)._vectEffects.Split('|');
            string[] newEffectsStr1 = neweffects[0].Split('+');
            EffectData effectdata1 = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == newEffectsStr1[0]);
            string[] newEffectsStr2 = neweffects[1].Split('+');
            EffectData effectdata2 = ItemDataManager.Instance.EffectDatas._effects.First(P=>P.m_SzName == newEffectsStr2[0]);
            Effect1Icon.spriteName=effectdata1.EffectRes;
            Effect2Icon.spriteName=effectdata2.EffectRes;
            Effect1NameLabel.text=LanguageTextManager.GetString(effectdata1.IDS);
            Effect2NameLabel.text=LanguageTextManager.GetString(effectdata2.IDS);
            Effect1NumLabel.SetText(newEffectsStr1[1]);
            Effect2NumLabel.SetText(newEffectsStr2[1]);
           
        }
	
    }
}