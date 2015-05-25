using UnityEngine;
using System.Collections;
namespace UI.Siren
{
	public class SirenConditionItem : MonoBehaviour {
		public SpriteSwith Icon;
		public SpriteSwith Tip;
		public UILabel Label;
		public bool IsMeet{get;private set;}
		public void Init(bool isMeet, string content)
		{
			IsMeet = isMeet;
			int no = isMeet == true? 2: 1;
			Label.text = content;
			Icon.ChangeSprite(no);
			Tip.ChangeSprite(no);
		}
	}
}