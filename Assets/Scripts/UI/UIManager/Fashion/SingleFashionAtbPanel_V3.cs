using UnityEngine;
using System.Collections;
using System.Linq;

namespace UI.MainUI
{

    public class SingleFashionAtbPanel_V3 : MonoBehaviour
    {

        public UILabel AtbNameLabel;
        public UILabel BaseValueLabel;
        public UILabel NewValueLabel;
        public UISprite IconSprite;


        public void Show(EffectData effectData,int baseValue,int newValue)
        {
            gameObject.SetActive(true);
            AtbNameLabel.SetText(LanguageTextManager.GetString(effectData.IDS));
			NewValueLabel.SetText(string.Format("+{0}",newValue));
            IconSprite.spriteName = effectData.EffectRes;
            //BaseValueLabel.SetText(baseValue);
            //int addValue = newValue - baseValue;
            //NewValueLabel.SetText(addValue>0?string.Format("+{0}",addValue.ToString()):"");
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }
    }
}