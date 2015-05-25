using UnityEngine;
using System.Collections;

namespace UI.MainUI{
	public class BaseTweenShowPanel : View {

		
		public Vector3 ShowPos;
		public Vector3 ClosePos;
		float animTime = 0.1f;

		protected override void RegisterEventHandler ()
		{
			throw new System.NotImplementedException ();
		}
		
		public virtual void TweenShow()
		{
			TweenAlpha.Begin(gameObject,animTime,1);
			TweenPosition.Begin(gameObject,animTime,ClosePos,ShowPos);
		}
		
		public virtual void TweenClose()
		{
			TweenAlpha.Begin(gameObject,animTime,0);
			TweenPosition.Begin(gameObject,animTime,ShowPos,ClosePos);
		}
		
		public virtual void Close()
		{
			TweenAlpha.Begin(gameObject,0,0);
			transform.localPosition = ClosePos;
		}

	}
}