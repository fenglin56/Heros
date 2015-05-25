using UnityEngine;
using System.Collections;

namespace UI.Battle
{
	public class StoreCoverUI : MonoBehaviour 
	{
		public UIPanel Panel_StoreCover;
		public Transform Trans_TopCover;
		public Transform Trans_BottomCover;

		private float m_durtion = 0.1666f;
		private float m_move = 78;
		private Vector3 m_showPos_TopCover;
		private Vector3 m_showPos_BottomCover;
		private Vector3 m_hidePos_TopCover;
		private Vector3 m_hidePos_BottomCover;

		void Awake()
		{
			//Panel_StoreCover.alpha = 0;
			m_showPos_TopCover = Trans_TopCover.localPosition;
			m_showPos_BottomCover = Trans_BottomCover.localPosition;
			m_hidePos_TopCover = Trans_TopCover.localPosition + Vector3.up * m_move;
			m_hidePos_BottomCover = Trans_BottomCover.localPosition + Vector3.down * m_move;
			Trans_TopCover.localPosition = m_hidePos_TopCover;
			Trans_BottomCover.localPosition = m_hidePos_BottomCover;
		}

		public void Appear(bool isFlag)
		{
			StopAllCoroutines();
			if(isFlag)
			{
				//StartCoroutine(AlphaTween(Panel_StoreCover,0,1,m_durtion));
				StartCoroutine(TranslateTo(Trans_TopCover,m_hidePos_TopCover,m_showPos_TopCover,m_durtion));
				StartCoroutine(TranslateTo(Trans_BottomCover,m_hidePos_BottomCover,m_showPos_BottomCover,m_durtion));
			}
			else
			{
				StartCoroutine(TranslateTo(Trans_TopCover,m_showPos_TopCover,m_hidePos_TopCover,m_durtion));
				StartCoroutine(TranslateTo(Trans_BottomCover,m_showPos_BottomCover,m_hidePos_BottomCover,m_durtion));
				//StartCoroutine(AlphaTween(Panel_StoreCover,1,0,m_durtion));
			}
		}

		IEnumerator TranslateTo (Transform trans, Vector3 startPos, Vector3 endPos , float time )
		{
			float i = 0;
			//float nowAlpha = sprite.alpha;
			//time = (endAlpha - nowAlpha)/ ((endAlpha - startAlpha)/time );
			float nowPosY = trans.transform.localPosition.y;
			time = (endPos.y - nowPosY)/((endPos.y - startPos.y)/time);
			if(time > 0)
			{
				float rate = 1.0f/time;
				while (i < 1.0) {
					i += Time.deltaTime * rate;
					trans.localPosition =  Vector3.Lerp(startPos, endPos, i);
					yield return null;
				}
			}

		}
		IEnumerator AlphaTween (UIPanel panel, float startAlpha, float endAlpha , float time )
		{
			float i = 0;
			float nowAlpha = panel.alpha;
			time = (endAlpha - nowAlpha)/ ((endAlpha - startAlpha)/time );
			if(time > 0)
			{
				float rate = 1.0f/time;
				while (i < 1.0) {
					i += Time.deltaTime * rate;
					panel.alpha =  Mathf.Lerp(nowAlpha, endAlpha, i);
					yield return null;
				}
			}
		}
	}
}