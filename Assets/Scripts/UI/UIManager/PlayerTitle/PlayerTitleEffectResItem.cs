using UnityEngine;
using System.Collections;

namespace UI.PlayerTitle
{
	public class PlayerTitleEffectResItem : MonoBehaviour 
	{
		public UILabel Label_Attribute;
		public UILabel Label_Value;
		public UISprite Sprite_Icon;

		public void UpdateView(EffectData effectData, int value)
		{
			Label_Attribute.text = LanguageTextManager.GetString( effectData.IDS);
			Sprite_Icon.spriteName = effectData.EffectRes;
			Label_Value.text = value.ToString();
		}
	}
}
