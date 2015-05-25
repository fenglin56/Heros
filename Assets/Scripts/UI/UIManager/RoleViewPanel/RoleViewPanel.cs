using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace UI.MainUI
{

    public class RoleViewPanel : View
    {


        public Camera BackUICamera;
        public Camera FrontUICamera;
        public Transform BackGrondPanel;

        public UILabel NameLabel;
        public UILabel AtkLabel;//战力

        public GameObject AddAtkNumberTitle;

        public SingleButtonCallBack ViewAtbBtn;

        public RoleModelPanel roleModelPanel;
        public RoleAttributePanel roleAttributePanel;
        
        private bool IsFirstShow = true;
        private int CurrentAtkNumber = 0;

        void Awake()
        {
            ViewAtbBtn.SetCallBackFuntion(OnViewAtbBtnClick);
            AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetContainerGoods, ChangeHeroWeapon);
            ShowPanelInfo();
        }

        void OnDestroy()
        {
            RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
            UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetContainerGoods, ChangeHeroWeapon); 
        }

        public void SetRoleAttributePanelActive(bool flag)
        {
            FrontUICamera.gameObject.SetActive(flag);
        }

        public void SetRoleBackGroundPanelActive(bool flag)
        { 
            BackUICamera.gameObject.SetActive(flag);
        }

        void UpdateViaNotify(INotifyArgs inotifyArgs)
        {
            EntityDataUpdateNotify entityDataUpdateNotify = (EntityDataUpdateNotify)inotifyArgs;
            if (entityDataUpdateNotify.IsHero && entityDataUpdateNotify.nEntityClass == TypeID.TYPEID_PLAYER)
            {
                if (gameObject.active != false)
                {
                    roleModelPanel.ChangeHeroFashion();
                    ShowPanelInfo();
                }
                roleAttributePanel.ShowAttributePanelInfo();
            }
        }

        void ShowPanelInfo()
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            NameLabel.SetText(m_HeroDataModel.Name);
            ShowAtkInfo();
        }
        /// <summary>
        /// 显示战力信息
        /// </summary>
        void ShowAtkInfo()
        {
            int NewAtk = HeroAttributeScale.GetScaleAttribute(HeroAttributeScaleType.Display_Combat, PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_FIGHTING);
            if (CurrentAtkNumber == NewAtk)
                return;
            //TraceUtil.Log("刷新人物信息");
            if (IsFirstShow)
            {
                IsFirstShow = false;
                AtkLabel.SetText(NewAtk);
            }
            else
            {
                if (NewAtk > CurrentAtkNumber)
                {
                    StartCoroutine(ShowAddAtkAnim(NewAtk - CurrentAtkNumber));
                }
                TweenFloat.Begin(1, CurrentAtkNumber, NewAtk, SetAtkLabel);
            }
            CurrentAtkNumber = NewAtk;
        }

        IEnumerator ShowAddAtkAnim(int addNumber)
        {
            yield return null;
            SingleButtonCallBack creatTitle = CreatObjectToNGUI.InstantiateObj(AddAtkNumberTitle, AtkLabel.transform.parent).GetComponent<SingleButtonCallBack>();
            creatTitle.SetButtonText(string.Format("+{0}", addNumber.ToString()));
            Vector3 toPosition = AtkLabel.transform.localPosition;
            Vector3 fromPosition = toPosition;
            fromPosition += new Vector3(0, 0,-10);
            toPosition+= new Vector3(0, 50, -10);
            TweenPosition.Begin(creatTitle.gameObject,0.5f,fromPosition,toPosition,DestroyObj);
            TweenAlpha.Begin(creatTitle.gameObject,0.5f,1,0,null);
        }

        void DestroyObj(object obj)
        {
            Destroy(obj as GameObject);
        }

        void SetAtkLabel(float number)
        {
            AtkLabel.SetText((int)number);
        }

        public void SetPanelPosition(Camera uiCamera, RoleViewPoint roleViewPoint)
        {
            Vector3 CameraPosition = uiCamera.WorldToScreenPoint(roleViewPoint.transform.position);
            BackGrondPanel.transform.position = BackUICamera.ScreenToWorldPoint(CameraPosition);
            roleAttributePanel.SetPanelPosition(CameraPosition);

            Vector3 LPosition = uiCamera.WorldToScreenPoint(roleViewPoint.LBound.position);
            Vector3 RPosition = uiCamera.WorldToScreenPoint(roleViewPoint.RBound.position);

            var lRoleRec = uiCamera.ScreenToViewportPoint(LPosition);
            var rRoleRec = uiCamera.ScreenToViewportPoint(RPosition);

            var w = rRoleRec.x - lRoleRec.x;

            StartCoroutine(roleModelPanel.SetCameraPosition(lRoleRec, w));

            //StartCoroutine(roleModelPanel.SetCameraPosition(CameraPosition));
           
        }
        public void SetPanelPosition(Camera uiCamera,Transform transformPoint)
        {
            Vector3 CameraPosition = uiCamera.WorldToScreenPoint(transformPoint.position);
            BackGrondPanel.transform.position = BackUICamera.ScreenToWorldPoint(CameraPosition);
            roleAttributePanel.SetPanelPosition(CameraPosition);

            float aa =160f/ uiCamera.aspect;
            TraceUtil.Log("Aspect:" + uiCamera.aspect + " aa:" + aa);
            //4:3   120
            //3:2   115
            //16:10 110
            //16:9  100            
            var newPoint1 = new Vector3((CameraPosition.x - 110f), CameraPosition.y, CameraPosition.z);  
            var newPoint2 = new Vector3((CameraPosition.x + 115f), CameraPosition.y, CameraPosition.z); 
           
            var roleRec = uiCamera.ScreenToViewportPoint(newPoint1);
            var roleRec1 = uiCamera.ScreenToViewportPoint(newPoint2);

            var w = roleRec1.x - roleRec.x;           

            StartCoroutine(roleModelPanel.SetCameraPosition(roleRec, w));
           
        }

        public void Show()
        {
            TraceUtil.Log("ShowRolePanel");
            gameObject.SetActive(true);
            roleModelPanel.Show();
            ShowAtkInfo();
        }

        public void Close()
        {
            StopAllCoroutines();
            roleModelPanel.StopAllCoroutines();
            roleModelPanel.Close();
            gameObject.SetActive(false);
            roleAttributePanel.ClosePanel();
            ViewAtbBtn.SetButtonBackground(1);
        }

        #region edit by lee
        public Dictionary<RoleAttributeType, SingleRoleAtrribute> ChangeInterface(string spriteName,string spritePressName, string nameLabel, Action callBack)
        {
            var viewSprite = ViewAtbBtn.GetComponentInChildren<UISprite>();
            if (viewSprite!=null)
            {
                viewSprite.spriteName = spriteName;
            }
            SpriteSwith spriteSwith = ViewAtbBtn.GetComponentInChildren<SpriteSwith>();
            if (spriteSwith != null)
            {
                spriteSwith.SpriteCount = 2;
                spriteSwith.SpriteArray = new string[2] { spriteName, spritePressName };
            }
            NameLabel.text = nameLabel;
            return roleAttributePanel.GetListAndResetCallBack(callBack);
        }
        public void ChangeInterface(string nameLabel)
        {
            NameLabel.text = nameLabel;
        }
        #endregion

        void ChangeHeroWeapon(object obj)
        {
            if (gameObject.active == false)
                return;
            roleModelPanel.ChangeHeroWeapon(null,true);
        }

        void OnViewAtbBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            bool IsShow = roleAttributePanel.ChangePanelActive();
            ViewAtbBtn.SetButtonBackground(!IsShow?1:2);
        }

        protected override void RegisterEventHandler()
        {
            throw new System.NotImplementedException();
        }
    }
}