using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class MartialArtsRoomListPanel : MonoBehaviour
    {

        public SearchRoomPanel SearchRoomPanel;
        public SingleButtonCallBack ResetRoomListBtn;
        public SingleButtonCallBack QuickJoinBtn;
        public MartialArtsRoomPanelManager MyParent { get; private set; }

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        public List<SingleMartialArtsRoomPanel> SingleMartialArtsRoomPanelList;
        private int[] m_guideBtnID = new int[5];

        void Awake()
        {
            UIEventManager.Instance.RegisterUIEvent(UIEventType.MartialArtsRoomList,ResetCurrentRoomListPanel);
            ResetRoomListBtn.SetCallBackFuntion(OnResetRoomListBtnClick);
            QuickJoinBtn.SetCallBackFuntion(OnQuickJoinBtnClick);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(QuickJoinBtn.gameObject, UIType.MartialArtsRoom, SubType.MartialMainButton, out m_guideBtnID[0]);
            //TODO GuideBtnManager.Instance.RegGuideButton(ResetRoomListBtn.gameObject, UIType.MartialArtsRoom, SubType.MartialResetRoom, out m_guideBtnID[1]);
        }

        void OnDestroy()
        {
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.MartialArtsRoomList, ResetCurrentRoomListPanel);
            for (int i = 0; i < m_guideBtnID.Length; i++)
            {
                //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
            }
        }

        public void Init(MartialArtsRoomPanelManager myParent)
        {
            MyParent = myParent;
        }

        public void Show()
        {
            transform.localPosition = Vector3.zero;
            SingleMartialArtsRoomPanelList.ApplyAllItem(P => P.Close());
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            MyParent.SendGetRoomListMsgToSever();
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            CommonBtnInfo SearchBtnInfo = new CommonBtnInfo(1, "JH_UI_Button_1116_19", "JH_UI_Button_1116_00", OnSearchRoomBtnClick);
            CommonBtnInfo newRoomBtnInfo = new CommonBtnInfo(2, "JH_UI_Button_1116_18", "JH_UI_Button_1116_00", CreatNewRoom);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo, SearchBtnInfo, newRoomBtnInfo });
            var btnInfoComponent = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if (btnInfoComponent != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponent.gameObject, UIType.MartialArtsRoom, SubType.ButtomCommon, out m_guideBtnID[2]);
            var btnInfo1Component = commonUIBottomButtonTool.GetButtonComponent(SearchBtnInfo);
            //if (btnInfo1Component != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo1Component.gameObject, UIType.MartialArtsRoom, SubType.ButtomCommon, out m_guideBtnID[3]);
            var btnInfo2Component = commonUIBottomButtonTool.GetButtonComponent(newRoomBtnInfo);
            //if (btnInfo2Component != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo2Component.gameObject, UIType.MartialArtsRoom, SubType.ButtomCommon, out m_guideBtnID[4]);
        }

        void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            Close(null);
        }

        void ResetCurrentRoomListPanel(object obj)
        {
            SMSGEctypePraicticeList_SC sMSGEctypePraicticeList_SC = (SMSGEctypePraicticeList_SC)obj;
            for (int i = 0; i < SingleMartialArtsRoomPanelList.Count; i++)
            {
                if (i < sMSGEctypePraicticeList_SC.EctypePraicticeList.Count)
                {
                    SingleMartialArtsRoomPanelList[i].Show(this, sMSGEctypePraicticeList_SC.EctypePraicticeList[i]);
                }
                else
                {
                    SingleMartialArtsRoomPanelList[i].Close();
                }
            }
        }

        void OnSearchRoomBtnClick(object obj)
        {
            TraceUtil.Log("SearchRoom");
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            SearchRoomPanel.Show(this);
        }

        void CreatNewRoom(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            MyParent.CreatNewRoom();
        }

        void OnQuickJoinBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_EnterRoom");
            MyParent.QuickJoin();
        }

        void OnResetRoomListBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
            MyParent.SendGetRoomListMsgToSever();
        }

        public void Close(object obj)
        {
            transform.localPosition = new Vector3(0, 0, -1000);
        }


    }
}