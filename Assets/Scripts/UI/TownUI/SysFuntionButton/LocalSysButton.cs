using UnityEngine;
using System.Collections;
using UI.MainUI;

public delegate void ButtonCallBack(object obj);
namespace UI
{
    public class LocalSysButton : MonoBehaviour
    {
        private bool Effisshow;
        /// <summary>
        /// UI系统按钮下的每个单个按钮的控制代码，赋予到每个按钮上面
        /// </summary>
        //int ButtonHight = 90;//按钮的高度，用于按钮单位高度位移
        //int ButtonWidth = 100;//按钮的宽度，用于按钮的单位宽度位移
        float AnimTime = 0.2f;//按钮动画播放时间
        public GameObject BtnEffect;
        Vector3 m_insertPos = Vector3.zero;
		private UIType m_buttonFunc = UIType.Empty;
		//private MainButtonConfigData m_curButtonData;
        bool Showing = false;//状态
        private string m_AudioName;
        public bool m_DefaultEnable;
        public UIType ButtonFunc 
        {
            get { return m_buttonFunc; }
        }

        public void SetBtnAtrribute(UIType func, Vector3 insertPos, bool Showing,bool defaultEnable,string AudioName)//初始化按钮属性，包括图片样式点击后返回操作等
        {
            gameObject.SetActive(false);
            this.m_buttonFunc = func;
            this.m_insertPos = insertPos;
            m_DefaultEnable=defaultEnable;
            m_AudioName = AudioName;
            this.Showing = Showing;

            if (Showing)
            {
                ShowInsertBtn();
            }

            if(defaultEnable)
            {
                ShowInsertBtn();
            }
        }


        void OnClick()
        {
			if(m_buttonFunc == UIType.Empty)
			{
				return;
			}
            SoundManager.Instance.PlaySoundEffect(m_AudioName);
            UIEventManager.Instance.TriggerUIEvent(UIEventType.OpentMainUI, m_buttonFunc);
        }

        void ShowInsertBtn()//显示出本按钮在插入的位置
        {
            ShowBtn(false);
        }

        public void ShowBtnEff(UIType uiType)
        {
            if (BtnEffect)
           {
            if (this.ButtonFunc != uiType)
                return;
            BtnEffect.SetActive(true);
                Effisshow=true;
           }
        }
        public void HideBtnEff(UIType uitype)
        {

            if (BtnEffect)
            {
                if (this.ButtonFunc != uitype)
                    return;
                BtnEffect.SetActive(false);
                Effisshow=false;
            }
        }
        public void PlayBtnAnim(UIType uiType)
        {
            if (this.ButtonFunc != uiType||animation.isPlaying)
                return;
            animation.Play();
        }

        public void StopBtnAnim(UIType uitype)
        {
            if (this.ButtonFunc != uitype)
                return;
            animation.Stop();
        }

        public void ShowBtn(bool NeedAnim)
        {
            CancelInvoke("hideBtn");
            gameObject.SetActive(true);
            Vector3 MoveTarget = m_insertPos;
            if (NeedAnim)
            {
               // gameObject.GetComponent<TweenPosition>().Reset();
                TweenPosition.Begin(gameObject, AnimTime, MoveTarget);
            }
            else
            {
                transform.localPosition = MoveTarget;
            }
            Showing = true;
            StartCoroutine(ShowEff());
        }
        IEnumerator ShowEff()
        {
            yield return new WaitForSeconds(AnimTime);
            if (BtnEffect&&Effisshow)
            {
                BtnEffect.SetActive(true);
            }
        }
        public void CloseBtn()
        {
            if (BtnEffect&&BtnEffect.activeSelf)
            {
                BtnEffect.SetActive(false);
                Effisshow=true;
            }
            TweenPosition.Begin(gameObject, AnimTime, Vector3.zero);
            Showing = false;
            CancelInvoke("hideBtn");
            Invoke("hideBtn",AnimTime);
        }
        void  hideBtn()
        {
            gameObject.SetActive(false);
            //解决Bug #23501::任务，打开菜单后，点击引导头像，菜单按钮显示穿帮
            //在引导保存好按钮状态后，功能按钮收起时会改变按钮状态。这时需要改变引导保存的状态
            var guideBtnBehaviour= GetComponent<GuideBtnBehaviour>();
            if (guideBtnBehaviour!= null)
            {
                var guideBtnParam = TaskBtnManager.Instance.FindGuideBtnParamViaMappingId(guideBtnBehaviour.MappingId);
                if (guideBtnParam != null)
                {
                    guideBtnParam.SaveStatus();
                }
            }
        }


        public void ResetPosition(int btnPos)
        {

            if (Showing)
            {
                ShowBtn(true);
            }
        }

        public void DestroyMyself()
        {
            Destroy(this.gameObject);
        }


    }
}