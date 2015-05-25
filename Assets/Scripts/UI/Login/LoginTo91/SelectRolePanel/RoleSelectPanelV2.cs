using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace UI.Login
{
    public class RoleSelectPanelV2 : IUIPanel
    {
        private SSActorInfo[] m_roleListInfo;

        private RoleSelectItem SelectRoleItem;

        public ItemPagerManager ItemPagerManager;
        public SingleButtonCallBack DelectRoleButton;
        public SingleButtonCallBack JoinGameButton;
        public SingleButtonCallBack CreatRoleButton;
        public SingleButtonCallBack BackBtn;
        public HeroModelManager HeroModelCamera;
        public GameObject RoleSelectItemPrefab;
        public GameObject OperatingModelSelectPanel;
        public UILabel RoleNameLabel;
        public UILabel RoleLevelLabel;
        public UILabel RoleTotalNumber;

        public UISprite Jiangke_vocation;
        public UISprite Cike_vocation;

        public UISprite Jiangke_bg;
        public UISprite Cike_bg;

        public DeleteRolePanel RoleDeletePanel;

        private int m_roleTotalNumber = 0;

        void Awake()
        {
            TraceUtil.Log("RoleSelectPanelV2 Awake");
            DelectRoleButton.SetCallBackFuntion(OnDelectRoleBtnClick);
            JoinGameButton.SetCallBackFuntion(OnJoinGameBtnClick);
            CreatRoleButton.SetCallBackFuntion(OnCreatRoleBtnClick);
            BackBtn.SetCallBackFuntion(OnBackBtnClick);
            ItemPagerManager.OnPageItemSelected += this.ItemPagerManager_OnPageItemSelected;
            ItemPagerManager.OnPageChanged += this.ItemPageChangedHandle;
        }

        void CheckIsShowOperatingPanel()
        {
            if (m_roleListInfo != null && m_roleListInfo.Length == 1 && m_roleListInfo[0].lLevel == 1)
            {
                OperatingModelSelectPanel.gameObject.SetActive(true);
                OperatingModelSelectPanel.GetComponent<OperatingModelSelectPanel>().Show(this);
            }
            else
            {
                OperatingModelSelectPanel.gameObject.SetActive(false); 
            }
        }

        void ItemPagerManager_OnPageItemSelected(IPagerItem selectedItem)
        {
            SelectedItem(selectedItem);
        }
        private void ItemPageChangedHandle(PageChangedEventArg pageChangedEventArg)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Page");
            int startIndex = (pageChangedEventArg.StartPage - 1) * pageChangedEventArg.PageSize;
            if (m_roleListInfo.Length < startIndex)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"分页出错 总数：" + m_roleListInfo.Length + " 开始:" + startIndex);
            }
            int itemCount = startIndex + pageChangedEventArg.PageSize > m_roleListInfo.Length ? m_roleListInfo.Length : startIndex + pageChangedEventArg.PageSize;

            IPagerItem[] roleItems = new IPagerItem[itemCount - startIndex];
            GameObject roleItem;
            for (int i = startIndex; i < itemCount; i++)
            {
                roleItem = GameObject.Instantiate(RoleSelectItemPrefab) as GameObject;
                roleItem.name = RoleSelectItemPrefab.name;
                var item = m_roleListInfo[i];
                var commonItem = roleItem.GetComponent<RoleSelectItem>();
                commonItem.InitItemData(item);
                roleItems[i - startIndex] = commonItem;
            }

            ItemPagerManager.UpdateItems(roleItems, RoleSelectItemPrefab.name);
        }
        public override void Show()
        {
            PlatformLoginBehaviour.message += " RoleSelectPanelV2 Show";

            this.m_roleListInfo = (SSActorInfo[])GameDataManager.Instance.PeekData(DataType.ActorSelector);
            SelectedItem(null);

            ItemPagerManager.InitPager(this.m_roleListInfo.Length,1,0);

            m_roleTotalNumber = this.m_roleListInfo.Length;
            RoleTotalNumber.SetText(m_roleTotalNumber.ToString());
            
            transform.localPosition = Vector3.zero;

            PlatformLoginBehaviour.message += " ServerListPanelV3 Position:" + GameObject.FindObjectOfType<ServerListPanel_V3>().transform.localPosition;

           // CheckIsShowOperatingPanel();这个版本暂时不用操作设置，所以先注释掉
            GameManager.Instance.m_gameSettings.JoyStickMode = true;//强制默认为摇杆
        }
        private void SelectedItem(IPagerItem selectedItem)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            RoleSelectItem selectedEquipItem = selectedItem as RoleSelectItem;
            if (selectedEquipItem != null)
            {
                SelectRoleItem = selectedEquipItem;
                this.RoleNameLabel.text=selectedEquipItem.GetItemData(ItemDataType.Name);
                this.RoleLevelLabel.text=selectedEquipItem.GetItemData(ItemDataType.Level);
                TraceUtil.Log("Item信息：" + selectedEquipItem.GetItemData(ItemDataType.Level) + this.RoleLevelLabel.text);
                TraceUtil.Log("Item信息：" + this.RoleNameLabel.text +" ddd "+ selectedEquipItem.GetItemData(ItemDataType.Name));
                GameDataManager.Instance.ResetData(DataType.SelectRoleData, SelectRoleItem);
                SetRoleModelViewInfo();

                byte vocation = selectedEquipItem.GetItemVocation();
                if(1 == vocation)
                {
                    Jiangke_vocation.enabled = true;;
                    Cike_vocation.enabled = false;
                    
                    Jiangke_bg.enabled = true;
                    Cike_bg.enabled = false;
                }
                else if(4 == vocation)
                {
                    Jiangke_vocation.enabled = false;;
                    Cike_vocation.enabled = true;
                    
                    Jiangke_bg.enabled = false;
                    Cike_bg.enabled = true;
                }

            }
            else
            {
                SelectRoleItem = null;
                TraceUtil.Log("Item信息：Empty");
                this.RoleNameLabel.SetText(string.Empty);
                this.RoleLevelLabel.SetText(string.Empty);
            }
        }
        public override void Close()
        {
            transform.localPosition = new Vector3(0, 0, -1000);
            HeroModelCamera.ClearModel();
        }

        public override void DestroyPanel()
        {
        }

        void OnDelectRoleBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (!LoginManager.Instance.DeleteActorButtonEnable) return;
            if (SelectRoleItem != null)
            {
                RoleDeletePanel.Show(SelectRoleItem.ItemDataInfo);
            }
        }

        void OnJoinGameBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (!LoginManager.Instance.EnterTownButtonEnable) return;
            LoginManager.Instance.EnterTownButtonEnable = false;
            if (SelectRoleItem != null)
            {
                LoginManager.Instance.LoginSSActorInfo = SelectRoleItem.ItemDataInfo;
            }
            NetServiceManager.Instance.LoginService.NewJumpToGamtway();
            #if (UNITY_ANDROID && !UNITY_EDITOR) 
            #if ANDROID_OPPO  
            JHPlatformConnManager.Instance.ExtendInfoSubmi(m_roleListInfo[0].SZServerIP,m_roleListInfo[0].SZName,m_roleListInfo[0].lLevel.ToString());
            #endif
            #endif
           
        }

        void OnCreatRoleBtnClick(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            if (this.m_roleListInfo.Length < 9)
            {
                UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowLodingUI, LoginUIType.CreatRole);
            }
            else
            {
                MessageBox.Instance.Show(3, "", LanguageTextManager.GetString("IDS_H1_216"), LanguageTextManager.GetString("IDS_H2_55"), null);
            }
        }
        void SetRoleModelViewInfo()
        {
            NewCharacterConfigData configData = LoginDataManager.Instance.GetNewCharacterConfigData(SelectRoleItem.ItemDataInfo.byKind);
            this.HeroModelCamera.ShowRoleModel(configData);
            var item = ItemDataManager.Instance.GetItemData(SelectRoleItem.ItemDataInfo.WeaponResID);
            if (item != null)
            {
                var weaponEff= (ItemDataManager.Instance.GetItemData(item._goodID)as EquipmentData).WeaponEff;
                this.HeroModelCamera.ChangeWeapon(PlayerFactory.Instance.GetWeaponPrefab(item._ModelId),weaponEff);
            }
            else
            {
                this.HeroModelCamera.ChangeWeapon(PlayerFactory.Instance.GetDefulWeaponPrefab(SelectRoleItem.ItemDataInfo.byKind),null);
            }
            if (SelectRoleItem.ItemDataInfo.FasionResID == 0)
            {
                StartCoroutine(this.HeroModelCamera.ChangeDefulsFashion());
            }
            else
            {
                StartCoroutine(this.HeroModelCamera.ChangeFashion(SelectRoleItem.ItemDataInfo.FasionResID));
            }

            //this.HeroModelCamera.PlayerSelectRoleAnimation();
            StartCoroutine(LatePlayerSelectRoleAnimation());
        }
        IEnumerator LatePlayerSelectRoleAnimation()
        {

            this.HeroModelCamera.PlayerSelectRoleAnimation();
            yield return null;//new WaitForSeconds(0.1f);
        }

        void OnBackBtnClick(object obj)
        {
            GameManager.Instance.QuitToLogin();
        }
    }
}