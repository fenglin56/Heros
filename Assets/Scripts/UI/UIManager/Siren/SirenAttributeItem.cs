using UnityEngine;
using System.Collections;

namespace UI.Siren
{
	public class SirenAttributeItem : MonoBehaviour 
	{
		//public SpriteSwith SpriteSwitch_Icon;
		public UISlicedSprite Sprite_Icon;
		public UILabel Label_txt;
		public UILabel Label_value;
		public UISlider Slider;

		public void Init(string nameIDS,string spriteName,int value, int maxValue)
		{
			Label_txt.text = LanguageTextManager.GetString(nameIDS);
			Sprite_Icon.spriteName  = spriteName;
			Label_value.text = "+"+value.ToString();
			if(maxValue<=0)
			{
				maxValue = 1;
			}
			Slider.sliderValue = value*1f/maxValue;
		}

		public void InitBreakLabel(string nameIDS, string spriteName, int curMaxValue, int nextMaxValue)
		{
			Label_txt.text = LanguageTextManager.GetString(nameIDS);
			Sprite_Icon.spriteName  = spriteName;
			Label_value.text = curMaxValue.ToString()+" → "+NGUIColor.SetTxtColor(nextMaxValue,TextColor.green);
		}
	}
}