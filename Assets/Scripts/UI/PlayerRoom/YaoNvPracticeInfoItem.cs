using UnityEngine;
using System.Collections;
namespace UI.PlayerRoom
{
    public class YaoNvPracticeInfoItem : MonoBehaviour,IPagerItem
    {

        public UILabel Label_Name;
        public UILabel Label_Addition;
        public SingleButtonCallBack Button_SwitchIn;
        public SingleButtonCallBack Button_SwitchOut;

        public delegate void RecoverSirenDelegate(int sirenID);
        public delegate void ReleaseSirenDelegate(int sirenID);
        private RecoverSirenDelegate recoverSirenDelegate;
        private ReleaseSirenDelegate releaseSirenDelegate;

        public int SirenID { get; private set; }

        public bool IsRelease { get; private set; }

        void Awake()
        {
            IsRelease = true;
            Button_SwitchIn.SetCallBackFuntion(RecoverSirenHandle, null);
            Button_SwitchOut.SetCallBackFuntion(ReleaseSirenHandle, null);
            Button_SwitchOut.gameObject.SetActive(false);
        }

        public void Set(int sirenID, string name, uint isShow, string addition, RecoverSirenDelegate recoverDelegate, ReleaseSirenDelegate releaseDelegate)
        {
            SirenID = sirenID;
            Label_Name.text = LanguageTextManager.GetString(name);
            Label_Addition.text = addition + "%";
            recoverSirenDelegate = recoverDelegate;
            releaseSirenDelegate = releaseDelegate;
            bool isTrue = isShow == 1 ? true : false;
            this.UpdateState(true);//创建出来的item默认是开启的
        }

        //收妖
        void RecoverSirenHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            IsRelease = false;
            Button_SwitchIn.gameObject.SetActive(false);
            Button_SwitchOut.gameObject.SetActive(true);
            recoverSirenDelegate(SirenID);            
        }
        //放出妖女
        void ReleaseSirenHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            IsRelease = true;
            Button_SwitchIn.gameObject.SetActive(true);
            Button_SwitchOut.gameObject.SetActive(false);
            releaseSirenDelegate(SirenID);            
        }

        /// <summary>
        /// 更新显示
        /// </summary>
        /// <param name="isTrue">显示妖女</param>
        public void UpdateState(bool isTrue)
        {
            TraceUtil.Log("[UpdateState]" + isTrue);
            IsRelease = isTrue;
            Button_SwitchIn.gameObject.SetActive(isTrue);
            Button_SwitchOut.gameObject.SetActive(!isTrue);
        }

        public void OnGetFocus()
        {            
        }

        public void OnLoseFocus()
        {            
        }

        public void OnBeSelected()
        {
        }

        public Transform GetTransform()
        {
            return transform;
        }
    }
}