using UnityEngine;
using System.Collections;
using System;
namespace UI.PlayerRoom
{
    public class PlayerRoomBoxControlPanel : MonoBehaviour
    {
        public enum BoxType
        {
            HomeOwner,
            Roomer_Get,
            Roomer_GetAndLeave,
        }

        public GameObject HomeOwnerWindow;
        public GameObject RoomerWindow_Get;
        public GameObject RoomerWindow_GetAndLeave;

        public UILabel Label_homeOwner;
        public UILabel Label_roomerGet;
        public UILabel Label_roomerGetAndLeave;

        public SingleButtonCallBack Button_HomeOwner_GoOn;
        public SingleButtonCallBack Button_HomeOwner_GetAndLeave;

        public SingleButtonCallBack Button_Roomer_GoOn;
        public SingleButtonCallBack Button_Roomer_Leave;

        public SingleButtonCallBack Button_Roomer_Get;
        public SingleButtonCallBack Button_Roomer_GetAndLeave;

        private Action m_Action_GetAndReturnTown;
        
        private int[] m_guideBtnID = new int[4];

		private bool m_IsLeaving = false;

        void Awake()
        {
            Button_HomeOwner_GoOn.SetCallBackFuntion(HomeOwnerGoOnHandle, null);
            Button_HomeOwner_GetAndLeave.SetCallBackFuntion(HomeOwnerGetAndLeaveHandle, null);
            Button_Roomer_GetAndLeave.SetCallBackFuntion(RoomerGetAndLeaveHandle, null);
            Button_Roomer_GoOn.SetCallBackFuntion(RoomerGetHandle, null);
            Button_Roomer_Leave.SetCallBackFuntion(RoomerLeaveHandle, null);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_HomeOwner_GoOn.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.PlayerRoomBoxControl, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_HomeOwner_GetAndLeave.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.PlayerRoomBoxControl, out m_guideBtnID[1]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Roomer_Get.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.PlayerRoomBoxControl, out m_guideBtnID[2]);
            //TODO GuideBtnManager.Instance.RegGuideButton(Button_Roomer_GetAndLeave.gameObject, UI.MainUI.UIType.MartialArtsRoom, SubType.PlayerRoomBoxControl, out m_guideBtnID[3]);
        }

        void OnDestroy()
        {
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void ShowPanel(BoxType type, int practiceResult)
        {
			if(m_IsLeaving)
				return;
            transform.localPosition = Vector3.zero;
            switch (type)
            {
                case BoxType.HomeOwner:
                    HomeOwnerWindow.SetActive(true);
                    string str = string.Format(LanguageTextManager.GetString("IDS_H1_497"), practiceResult.ToString());
                    Label_homeOwner.text = str;
                    break;
                case BoxType.Roomer_Get:
                    RoomerWindow_Get.SetActive(true);
                    string roomerGetStr = string.Format(LanguageTextManager.GetString("IDS_H1_500"), practiceResult.ToString());
                    Label_roomerGet.text = roomerGetStr;
                    break;
                case BoxType.Roomer_GetAndLeave:
                    RoomerWindow_GetAndLeave.SetActive(true);
                    string strr = string.Format(LanguageTextManager.GetString("IDS_H1_498") +"\n"+ LanguageTextManager.GetString("IDS_H1_499"), practiceResult.ToString());
                    Label_roomerGetAndLeave.text = strr;
                    break;
            }
        }

        public void SetActionCallBack(Action callBack)
        {
            m_Action_GetAndReturnTown = callBack;
        }

        public void HidePanel()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }

        void HomeOwnerGoOnHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            HomeOwnerWindow.SetActive(false);
            HidePanel();
        }

        void HomeOwnerGetAndLeaveHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            HomeOwnerWindow.SetActive(false);
            HidePanel();
            m_Action_GetAndReturnTown();
            StartCoroutine(LateReturnCity());
			m_IsLeaving = true;
        }

        void RoomerGetAndLeaveHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            RoomerWindow_GetAndLeave.SetActive(false);
            HidePanel();
            m_Action_GetAndReturnTown();
            StartCoroutine(LateReturnCity());
			m_IsLeaving = true;
        }

        void RoomerGetHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
             RoomerWindow_Get.SetActive(false);
            HidePanel();
        }

        void RoomerLeaveHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            RoomerWindow_GetAndLeave.SetActive(false);
            HidePanel();
            m_Action_GetAndReturnTown();
            StartCoroutine(LateReturnCity());
			m_IsLeaving = true;
        }

        IEnumerator LateReturnCity()
        {
            yield return new WaitForSeconds(4f);
            //请求返回城镇
            long UIDEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
            NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(UIDEntity);
        }
    }
}