using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    public delegate void FuntionCallBack();
    public class FloatTips : MonoBehaviour
    {

        float TitleAnimTime = 1;
        private ButtonCallBack CompleteCallBack;

        public UILabel TipsLabel;

        public void Show(string ShowInfo, ButtonCallBack CallBackFuntion,float TipsTime)
        {
            this.TitleAnimTime = TipsTime;
            this.CompleteCallBack = CallBackFuntion;
            TipsLabel.text = ShowInfo;
            //Color tipsColor = Color.white;
            //tipsColor.a = 1;
            //TweenColor.Begin(gameObject, 0.2f, tipsColor);
            TweenAlpha.Begin(gameObject,0.2f,1);
            TweenPosition.Begin(gameObject, TitleAnimTime, transform.localPosition, transform.localPosition += new Vector3(0, 50, 0));
            StartCoroutine(TransparentMySelf());
        }

        IEnumerator TransparentMySelf()
        {
            yield return new WaitForSeconds(TitleAnimTime-0.2f);
            TweenAlpha.Begin(gameObject, 0.2f,0);
            StartCoroutine(DistoryTitleObj());
        }

        IEnumerator DistoryTitleObj()
        {
            yield return new WaitForSeconds(0.3f);
            if (this.CompleteCallBack != null) { this.CompleteCallBack(null); }
            Destroy(gameObject);
        }
    }
}