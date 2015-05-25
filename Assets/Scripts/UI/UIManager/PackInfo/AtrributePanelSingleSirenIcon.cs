using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class AtrributePanelSingleSirenIcon : MonoBehaviour {

		public UISprite SirenIcon;
		public UISprite Background;
		public SingleButtonCallBack LevelLabel;
		

		
		public void Show(string resName,int level)
		{
			bool isUnLock = level>0;
			SirenIcon.spriteName = resName;
			LevelLabel.SetButtonText(string.Format("Lv.{0}", level.ToString()));
			LevelLabel.gameObject.SetActive(isUnLock);
			Background.color = SirenIcon.color = isUnLock?Color.white:Color.gray;
		}

	}
}