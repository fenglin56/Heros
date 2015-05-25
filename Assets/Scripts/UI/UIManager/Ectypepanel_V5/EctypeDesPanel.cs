using UnityEngine;
using System.Collections;

namespace UI.MainUI{

	public class EctypeDesPanel : MonoBehaviour {

		public UILabel NameLabel;
		public UILabel BossDesLabel;
		public UILabel EctypeDesLabel;
		public UIPanel MyPanel;

		public Vector3 ShowPos;
		public Vector3 ClosePos;

		float animTime = 0.1f;

		void Awake()
		{
			Close();
		}

		public void TweenShow(EctypeContainerData selectData)
		{
			NameLabel.SetText(LanguageTextManager.GetString(selectData.lEctypeName));
			BossDesLabel.SetText(LanguageTextManager.GetString(selectData.EctypeBossDescription).Replace("\\n","\n"));
			EctypeDesLabel.SetText(LanguageTextManager.GetString(selectData.EctypeDescription).Replace("\\n","\n"));
			TweenAlpha.Begin(gameObject,animTime,1);
			TweenPosition.Begin(gameObject,animTime,ShowPos);
		}

		public void TweenClose()
		{
			TweenAlpha.Begin(gameObject,animTime,0);
			TweenPosition.Begin(gameObject,animTime,ClosePos);
		}

		public void Close()
		{
			TweenAlpha.Begin(gameObject,0,0);
			transform.localPosition = ClosePos;
		}

//		void DestroyTweenObj()
//		{
//			if(TweenObj!=null)
//			{
//				DestroyImmediate(TweenObj);
//			}
//		}
	}
}