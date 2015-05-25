using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class MartialArtsMainPanel : MonoBehaviour
    {

        public SingleButtonCallBack QuickJoinBtn;

        public MartialArtsRoomPanelManager MyParent { get; private set; }
        
        public GameObject HeroModelViewPrefab;
        public RoleViewPoint RoleViewPoint;
        private RoleViewPanel roleViewPanel;
        private Camera MyUICamera;

        public GameObject UIBottomBtnPrefab;
        public Transform CreatBottomBtnPoint;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        private int[] m_guideBtnID = new int[4];

        void Awake()
        {
            MyUICamera = UICamera.currentCamera;
            QuickJoinBtn.SetCallBackFuntion(QuickJoin);
        }

        void Start()
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(QuickJoinBtn.gameObject, UIType.MartialArtsRoom, SubType.MartialMainButton, out m_guideBtnID[0]);
        }

        void OnDestroy()
        {
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
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }
            else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            if (roleViewPanel == null)
            {
                roleViewPanel = (GameObject.Instantiate(HeroModelViewPrefab) as GameObject).GetComponent<RoleViewPanel>();
                roleViewPanel.SetPanelPosition(MyUICamera, RoleViewPoint);
                roleViewPanel.SetRoleAttributePanelActive(false);
                roleViewPanel.SetRoleBackGroundPanelActive(false);
            }
            else
            {
                roleViewPanel.Show();
            }
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);
            CommonBtnInfo roomListBtnInfo = new CommonBtnInfo(1, "JH_UI_Button_1116_17", "JH_UI_Button_1116_00", ShowRoomListPanel);
            CommonBtnInfo newRoomBtnInfo = new CommonBtnInfo(2, "JH_UI_Button_1116_18", "JH_UI_Button_1116_00", CreatNewRoom);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo,roomListBtnInfo,newRoomBtnInfo});
            var btnInfoComponent = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            //if (btnInfoComponent != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponent.gameObject, UIType.MartialArtsRoom, SubType.ButtomCommon, out m_guideBtnID[1]);
            var btnInfo1Component = commonUIBottomButtonTool.GetButtonComponent(roomListBtnInfo);
            //if (btnInfo1Component != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo1Component.gameObject, UIType.MartialArtsRoom, SubType.ButtomCommon, out m_guideBtnID[2]);
            var btnInfo2Component = commonUIBottomButtonTool.GetButtonComponent(newRoomBtnInfo);
            //if (btnInfo2Component != null)
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfo2Component.gameObject, UIType.MartialArtsRoom, SubType.ButtomCommon, out m_guideBtnID[3]);
        }

        void ShowRoomListPanel(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            transform.localPosition = new Vector3(0, 0, -1000);
            if (roleViewPanel != null)
            {
                roleViewPanel.Close();
            }
            MyParent.ShowRoomListPanel();
        }

        void CreatNewRoom(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            MyParent.CreatNewRoom();
        }

        void QuickJoin(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_EnterRoom");
            MyParent.QuickJoin();
        }

        void OnBackButtonTapped(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            MyParent.OnCloseBtnClick();
            //Close(null);
        }

        public void Close(object obj)
        {
            if (roleViewPanel != null)
            {
                roleViewPanel.Close();
            }
            transform.localPosition = new Vector3(0,0,-1000);
            //MyParent.OnCloseBtnClick();
        }

    }
}