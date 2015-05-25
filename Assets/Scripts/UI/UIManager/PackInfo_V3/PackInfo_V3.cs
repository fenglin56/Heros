using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class PackInfo_V3 : BaseUIPanel
    {

        private static PackInfo_V3 m_instace;
        public static PackInfo_V3 Instance { get { return m_instace; } }

        public GameObject HeroModelViewPrefab;
        //public GameObject UITitlePrefab;
        public GameObject UIBottomBtnPrefab;
        public Transform loadingPanel;

        public Transform CreatRolePoint;
        public RoleViewPoint RoleViewPoint;
        public Transform CreatBottomBtnPoint;

        public ContainerPackList_V2 containerPackList;
        public HeroEquiptItemList_V2 heroEquiptItemList;
        public ContainerTipsManager_V2 ContainerTipsManager;
        public GameObject CommonToolPrefab;

        //private Camera MyUICamera;
        //private RoleViewPanel roleViewPanel;
        private CommonUIBottomButtonTool commonUIBottomButtonTool;

        private int m_guideBtnID;
        void Awake()
        {
            if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform);}
            m_instace = this;
            //MyUICamera = UICamera.currentCamera;
        }

        void OnDestroy()
        {
            m_instace = null;

            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID);
        }

        void ShowBottomBtn()
        {
            CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnCloseBtnClick);
            commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() {btnInfo });

            var btnInfoComponet = commonUIBottomButtonTool.GetButtonComponent(btnInfo);
            if (btnInfoComponet != null)
            {
                //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponet.gameObject, UIType.Package, SubType.ButtomCommon, out m_guideBtnID);
            }
            
        }

        public override void Show(params object[] value)
        {
            heroEquiptItemList.Init(this);
            containerPackList.Init(this);
            base.Show(value);
            transform.localPosition = new Vector3(0, 0, -1000);
            LoadingUI.Instance.Show();
            StartCoroutine(ShowPanelInfoForSeconds());
        }

        IEnumerator ShowPanelInfoForSeconds()
        {
            yield return new WaitForSeconds(0.5f);
            transform.localPosition = Vector3.zero;
            LoadingUI.Instance.Close();
            //if (roleViewPanel == null)
            //{
            //    roleViewPanel = (GameObject.Instantiate(HeroModelViewPrefab) as GameObject).GetComponent<RoleViewPanel>();
            //    roleViewPanel.SetPanelPosition(MyUICamera, RoleViewPoint);
            //}else
            //{
            //    roleViewPanel.Show();
            //}
            if (commonUIBottomButtonTool == null)
            {
                commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
                ShowBottomBtn();
            }else
            {
                commonUIBottomButtonTool.ShowAnim();
            }
            containerPackList.Show();
            heroEquiptItemList.ResetPanel(null);

        }

        void OnCloseBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            CleanUpUIStatus();
            Close();
        }

        public override void Close()
        {
            if (!IsShow)
                return;
            UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
            StopAllCoroutines();
            LoadingUI.Instance.Close();
            CloseContainerTips();
            //if (roleViewPanel != null)
            //{
            //    roleViewPanel.Close();
            //}
            base.Close();
        }

        public void CloseContainerTips()
        {
            ContainerTipsManager.CloseTips(); 
        }
    }
}