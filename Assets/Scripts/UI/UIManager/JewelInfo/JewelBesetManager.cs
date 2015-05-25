using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.MainUI
{
    public class JewelBesetManager : BaseUIPanel
    {
        #region     右侧按钮列表管理
        public GameObject RightBtnManagerPrefab;
        public Vector3 RightBtnShowPos;
        public Vector3 RightBtnHidPos;
        private PackRightBtnManager m_PackRightBtnManager; 
        #endregion
        #region Prefab
        public GameObject CommonPanelTitle_Prefab;
        public GameObject JewelContainerListPanel_prefab;
        public GameObject JewelupgradePanel_prefab;
        public GameObject JewelBesetPanel_prefab;
        public GameObject ChoseJewel_Beset_prefab;
        public GameObject Swallow_chose_prefab;
        public GameObject Swallow_Ensure_prefab;
        #endregion 
        public SingleButtonCallBack BackBtn;

        private bool m_createInstance;

        private GameObject JewelContainerListPanel, JewelupgradePanel, JewelBesetPanel, ChoseJewel_Beset, Swallow_chose, Swallow_Ensure;

        private JewelState  CurrentState;
        public JewelBset_Container Sc_Container;
        private JewelBeset_Beset sc_Beset;
        private JewelBeset_Upgrade sc_upgrade;
        private ChoseJewelContainer_Beset sc_Chose_Beset;
        private Swallow_Chose sc_Swallow_Chose;
        private Swallow_Ensure sc_Swallow_Ensure;
        private static JewelBesetManager instance;
        public ItemFielInfo SelectedJewel;
        public PassiveSkillDataBase passiveSkillDataBase;
      
        public GameObject BesetSuccessEff1_profab;
        private GameObject BesetSuccessEff1;
        public GameObject SwallowSuccessEff1_prefab;
        private GameObject SwallowSuccessEff1;
        private bool IsFirst=true;
       
        void Awake()
        {
            m_PackRightBtnManager = CreatObjectToNGUI.InstantiateObj(RightBtnManagerPrefab, transform).GetComponent<PackRightBtnManager>();
            m_PackRightBtnManager.transform.localPosition = RightBtnHidPos;
            BesetSuccessEff1 = CreatObjectToNGUI.InstantiateObj(BesetSuccessEff1_profab, transform);
            SwallowSuccessEff1 = CreatObjectToNGUI.InstantiateObj(SwallowSuccessEff1_prefab, transform);
            SwallowSuccessEff1.transform.localPosition=new Vector3(0,0,-25);
            BesetSuccessEff1.SetActive(false);
            SwallowSuccessEff1.SetActive(false);
            //返回按钮点击
            BackBtn.SetCallBackFuntion((obj) =>
            {
                SoundManager.Instance.PlaySoundEffect("Sound_Button_Equipment_Cancel");
             
                this.Close();
            });
            //返回按钮按下/松开效果
            BackBtn.SetPressCallBack((isPressed) =>
            {
                BackBtn.spriteSwithList.ApplyAllItem(P => P.ChangeSprite(isPressed ? 2 : 1));
            });
            m_createInstance = true;
            var commonPanel = NGUITools.AddChild(gameObject, CommonPanelTitle_Prefab);
            commonPanel.transform.localPosition = CommonPanelTitle_Prefab.transform.localPosition;
            commonPanel.GetComponent<CommonPanelTitle>().TweenShow();
            //RegisterEventHandler ();
            TaskGuideBtnRegister();
        }
        /// <summary>
        /// 引导按钮注入代码
        /// </summary>
        private void TaskGuideBtnRegister()
        {
            BackBtn.gameObject.RegisterBtnMappingId(UIType.Gem, BtnMapId_Sub.Gem_Back);
        }

        public IEnumerator ShowSwallowSuccseeEff()
        {
            SwallowSuccessEff1.SetActive(true);
            yield return new WaitForSeconds(1f);
            //BesetEff1.SetActive(false);
            SwallowSuccessEff1.SetActive(false);
        
        }

        public IEnumerator ShowBesetSuccseeEff()
        {
            BesetSuccessEff1.SetActive(true);
            yield return new WaitForSeconds(1f);
            //BesetEff1.SetActive(false);
            BesetSuccessEff1.SetActive(false);
            
        }

        public static JewelBesetManager GetInstance()
        {
            if (!instance)
            {
                instance = (JewelBesetManager)GameObject.FindObjectOfType(typeof(JewelBesetManager));
                if (!instance)
                    Debug.LogError("没有附加JewelBesetManager脚本的gameobject在场景中");
            }
            return instance;
        }

        public override void Show(params object[] value)
        {
            object defultSelectItem=null;
            IsFirst = true;
            if (value==null||value.Length==0)
            {
                CurrentState = JewelState.JewelBeset;
            }
            else if(value.Length==1)
            {
                CurrentState = (JewelState)value [0];
            }
            else
            {
                CurrentState = (JewelState)value [0];
                defultSelectItem=value[1];
            }

            InitRightTipsButton(CurrentState);
            Init();
            TweenAlpha.Begin(m_PackRightBtnManager.gameObject, 0.1f, 0, 1, null);
            TweenPosition.Begin(m_PackRightBtnManager.gameObject, 0.1f, m_PackRightBtnManager.transform.localPosition, RightBtnShowPos);
            base.Show(value);
//                      StartCoroutine (HideAll ());
            Sc_Container.Init(CurrentState,defultSelectItem);
            //ChangeTab (tab); 
        }

        void Init()
        {
            CreatObject();
        }


        /// <summary>
        /// 初始化右侧tipsbutton
        /// </summary>
        /// <param name="tab">Tab.</param>
        public void InitRightTipsButton(JewelState tab)
        {
            PackBtnType[] btnTypeList = null;
            if (tab == JewelState.JewelBeset)
            {
                btnTypeList = new PackBtnType[]
                {
                    PackBtnType.Package,
                };
            } else if (tab == JewelState.JewelUpgrad)
            {

                
                btnTypeList = new PackBtnType[]
                {
                    PackBtnType.Upgrade,
                    PackBtnType.Package,
                };
            }
            m_PackRightBtnManager.PackBtnOnClick = OnButtonClick;
            StartCoroutine(m_PackRightBtnManager.AddBtn(btnTypeList));

            #region`    引导注入代码

            StartCoroutine(RegisterRightBtn());
            #endregion
        }
        private IEnumerator RegisterRightBtn()
        {
            //因为右边按钮的创建，在下一帧进行位置调整，所以这里两帧后再注入引导，保证引导信息会加在正确的按钮上
            yield return null;
            yield return null;
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Upgrade, UIType.Gem, BtnMapId_Sub.Gem_Upgrade);
            m_PackRightBtnManager.RegisterGuideBtn(PackBtnType.Package, UIType.Gem, BtnMapId_Sub.Gem_PackageBtn);
        }
        void OnButtonClick(PackBtnType clickBtnType)
        {
            switch (clickBtnType)
            {
            
                case PackBtnType.Upgrade:
                    {
                        Jewel jewel = ItemDataManager.Instance.GetItemData (Sc_Container.SelectedItemList [0].ItemFielInfo.LocalItemData._goodID) as Jewel;
                        if(Sc_Container.SelectedItemList [0].ItemFielInfo.materiel.ESTORE_FIELD_LEVEL<jewel.MaxLevel)
                        {
                            UpdateChosejewelList_Swallow(Sc_Container.SelectedItemList [0].ItemFielInfo);       
                            ChangeSubUistate(JewelState.jewelChose_Upgrade);
                        }
                        else
                        {
                        MessageBox.Instance.ShowTips(3,LanguageTextManager.GetString("IDS_I9_25"),1);
                        }
                    }
                    break;
                case PackBtnType.Package:
                    MainUIController.Instance.OpenMainUI(UIType.Package);
                    break;
                default:
                    break;
            }
            // Close(null);
        }

        void CreatObject()
        {
            if (m_createInstance)
            {
                m_createInstance = false;

                JewelContainerListPanel = NGUITools.AddChild(gameObject, JewelContainerListPanel_prefab);
                JewelContainerListPanel.transform.localPosition = JewelContainerListPanel_prefab.transform.localPosition;
                Sc_Container = JewelContainerListPanel.GetComponent<JewelBset_Container>();
                                

                JewelBesetPanel = NGUITools.AddChild(gameObject, JewelBesetPanel_prefab);   
                JewelBesetPanel.transform.localPosition = JewelBesetPanel_prefab.transform.localPosition;
                sc_Beset = JewelBesetPanel.GetComponent<JewelBeset_Beset>();
                                

                JewelupgradePanel = NGUITools.AddChild(gameObject, JewelupgradePanel_prefab);
                JewelupgradePanel.transform.localPosition = JewelupgradePanel_prefab.transform.localPosition;
                sc_upgrade = JewelupgradePanel.GetComponent<JewelBeset_Upgrade>();
                            

                ChoseJewel_Beset = NGUITools.AddChild(gameObject, ChoseJewel_Beset_prefab);
                ChoseJewel_Beset.transform.localPosition = ChoseJewel_Beset_prefab.transform.localPosition;
                sc_Chose_Beset = ChoseJewel_Beset.GetComponent<ChoseJewelContainer_Beset>();
                               
                Swallow_chose = NGUITools.AddChild(gameObject, Swallow_chose_prefab);
                Swallow_chose.transform.localPosition = Swallow_chose_prefab.transform.localPosition;
                sc_Swallow_Chose = Swallow_chose.GetComponent<Swallow_Chose>();

                Swallow_Ensure = NGUITools.AddChild(gameObject, Swallow_Ensure_prefab);
                Swallow_Ensure.transform.localPosition = Swallow_Ensure_prefab.transform.localPosition;
                sc_Swallow_Ensure = Swallow_Ensure.GetComponent<Swallow_Ensure>();
                                
            } 

                
        }

        public override void Close()
        {
            if (!IsShow)
                return;

            StartCoroutine(AnimToClose());
            HideEff();
        }

        IEnumerator AnimToClose()
        {
            Sc_Container.CloseAnim();
            sc_Beset.CloseAnim();
            sc_upgrade.CloseAnim();
            sc_Chose_Beset.CloseAnim();
            TweenPosition.Begin(m_PackRightBtnManager.gameObject, 0.1f, RightBtnHidPos);
            TweenAlpha.Begin(m_PackRightBtnManager.gameObject, 0.1f, 1, 0, null);
            yield return new WaitForSeconds(0.16f);
          
            base.Close();
        }
               
        public void InitContianerTab(JewelState tab)
        {
            sc_Chose_Beset.HidePanel();
            Sc_Container.ShowAnim();
            sc_Swallow_Chose.HidePanel();
            sc_Swallow_Ensure.HidePanel();
            if (tab == JewelState.JewelBeset)
            {
                sc_Beset.ShowAnim();
                sc_upgrade.HidePanel();
            } else
            {
                
                sc_Beset.HidePanel();
                sc_upgrade.ShowAnim();
            }
        }

        public void ChangeSubUistate(JewelState state)
        {
                        
            switch (state)
            {
                case JewelState.JewelBeset:
                    if(IsFirst)
                    {
                        Sc_Container.ShowAnim();
                        sc_Beset.ShowAnim();
                        IsFirst=false;
                    }
                    else
                    {
                        Sc_Container.ShowPanel();
                        sc_Beset.ShowPanel();
                    }
                    sc_Chose_Beset.HidePanel();
                    sc_upgrade.HidePanel();
                    sc_Swallow_Chose.HidePanel();
                    sc_Swallow_Ensure.HidePanel();
                            
                    break;
                case JewelState.JewelUpgrad:
                    if(IsFirst)
                    {
                        Sc_Container.ShowAnim();
                        sc_upgrade.ShowAnim();
                        IsFirst=false;
                    }
                    else
                    {
                        Sc_Container.ShowPanel();
                        sc_upgrade.ShowPanel();
                    }
                    sc_Chose_Beset.HidePanel();
                  
                    sc_Beset.HidePanel();
                   
                    sc_Swallow_Chose.HidePanel();
                    sc_Swallow_Ensure.HidePanel();
                    break;
                case JewelState.JewelChose_Beset:
                                
                                //sc_Chose_Beset.Init (1001052, 1);
                    sc_Chose_Beset.ShowPanel();
                    Sc_Container.HidePanel();
                    sc_Beset.ShowPanel();
                    sc_upgrade.HidePanel();
                    sc_Swallow_Chose.HidePanel();
                    sc_Swallow_Ensure.HidePanel();
                    break;
                case JewelState.jewelChose_Upgrade:
                                
                    sc_Chose_Beset.HidePanel();
                    Sc_Container.ShowPanel();
                    sc_Beset.HidePanel();
                    sc_upgrade.ShowPanel();
                                //sc_Swallow_Chose.Init (JewelTab.jewelChose_Upgrade, 0);
                    sc_Swallow_Chose.ShowPanel();
                    sc_Swallow_Ensure.HidePanel();
                    break;
                case JewelState.JewelChose_Upgrade_ensure:
                    sc_Chose_Beset.HidePanel();
                    Sc_Container.ShowPanel();
                    sc_Beset.HidePanel();
                    sc_upgrade.ShowPanel();
                    sc_Swallow_Chose.HidePanel();
                    sc_Swallow_Ensure.ShowPanel();
                    break;

            }
                        
        }
                
        public void UPdateContain(JewelState tab, ItemFielInfo itemFielInfo)
        {
            Sc_Container.UpdateContain(tab, itemFielInfo);
        }


        public void InitBeset_Attribute(ItemFielInfo itemFileInfo,JewelBset_ContainerItem item)
        {
            sc_Beset.InitAttribute(itemFileInfo,item);
        }
        /// <summary>
        ///刷新装备背包，（指定装备） 
        /// </summary>
        /// <param name="itemFileInfo">Item file info.</param>
        public void UpdateBesetPanel(ItemFielInfo itemFileInfo)
        {
            sc_Beset.Init(itemFileInfo);
        }
        /// <summary>
        /// 刷新宝石背包（指定升级哪个宝石）
        /// </summary>
        /// <param name="itemFileInfo">Item file info.</param>
        public void UpdateUpgradePanel(ItemFielInfo itemFileInfo)
        {
            sc_upgrade.Init(itemFileInfo);
        }
        /// <summary>
        ///更新宝石的包裹(选择用来镶嵌)
        /// </summary>
        /// <param name="itemfileInfo">Itemfile info.</param>
        /// <param name="Place">Place.</param>
        public void UpdateChoseJewelList_Beset(ItemFielInfo itemfileInfo, int Place)
        {
            sc_Chose_Beset.Init(itemfileInfo, Place);
        }
        /// <summary>
        /// 更新宝石的包裹(选择用来吞噬).
        /// </summary>
        /// <param name="itemFileInfo">Item file info.</param>
        public void UpdateChosejewelList_Swallow(ItemFielInfo itemFileInfo)
        {
            //Jewel jewel = ItemDataManager.Instance.GetItemData (itemFileInfo.LocalItemData._goodID) as Jewel;
            sc_Swallow_Chose.Init(itemFileInfo);
        }
        /// <summary>
        /// 更新宝石的包裹(确认吞噬).
        /// </summary>
        public void UpdateChodeJewelList_swallow_ensure(ItemFielInfo itemfielInfo, List<JewelBset_ContainerItem> SelectedList)
        {

            sc_Swallow_Ensure.Init(itemfielInfo, SelectedList);
        }

//        public void ShowRemoveJewel1Effect()
//        {
//            StartCoroutine(sc_Beset.ShowRemoveJewelEffect1());
//        }

        public void ShowRemoveJewel2Effect()
        {
            StartCoroutine(sc_Beset.ShowRemoveJewelEffect2());
        }

        public Vector3 GetFirsHolePos()
        {
            return sc_Beset.FirstHolePoint.transform.position;
        }

        public Vector3 GetSecondHolePos()
        {
            return sc_Beset.SecondHolePoint.transform.position;
        }

        public Vector3 GetUpdagradeHolePos()
        {
            return sc_upgrade.IconPoint.transform.position;
        }
        public void DisableAllHoleButton()
        {
            sc_Beset.DisableAllHoleButton();
        }
        
        public void EnableAllHoleButton()
        {
            sc_Beset.EnableAllHoleButton();
        }
        public void HideEff()
        {
            sc_Chose_Beset.ItemList.ApplyAllItem(c => {

                c.StopAllCoroutines();
                c.Eff_point.gameObject.SetActive(false);
            });
            
            sc_Swallow_Ensure.ItemList.ApplyAllItem(c => {
                
                c.StopAllCoroutines();
                c.Eff_point.gameObject.SetActive(false);
            });
        }
       
    }
}
public class JewelInfo
{
    public int JewelID;
    public int JewelLevel;
    public int jewelExp;
}
public enum JewelState
{
    JewelUpgrad,//器魂升级
    JewelBeset,//器魂镶嵌
    JewelChose_Beset,//镶嵌器魂选择
    jewelChose_Upgrade,//吞噬器魂选择
    JewelChose_Upgrade_ensure,//确认吞噬
}