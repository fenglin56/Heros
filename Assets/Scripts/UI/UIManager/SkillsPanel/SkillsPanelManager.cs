using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
using System.Linq;
using UI;

public class SkillsPanelManager : BaseUIPanel
{
    public GameObject Prefab_SkillItem;
    public GameObject Unlock_SkillItem;
    public GameObject DragParent;
    public ItemPagerManager ItemPagerManager;
    //public GameObject HeroModelViewPrefab;
    //public GameObject UITitlePrefab;
    public GameObject UIBottomBtnPrefab;
    public SkillsItem SelectedSkillItemBehaviour;
    public GameObject UpgradeSkillPanel;
    public Transform UpgradeSkillPanelPos;
    public GameObject AssemblySkillPanel;

    //public Transform CreatRolePoint;
    //public RoleViewPoint RoleViewPoint;
    public Transform CreatBottomBtnPoint;

    public static SingleSkillInfoList m_playerSkillList;//装备技能列表

    private Camera m_uiCamera;
    private bool m_isConsumeEnough;

    enum SkillUIType
    {
        None,
        AssemblySkill,
        UpgradeSkill,
        ViewSkill,
    }

    private SkillUIType m_skillUIType = SkillUIType.None;
    //private CommonBtnInfo m_buttomBtn;
    private int m_heroVocation;
    //private RoleViewPanel m_roleViewPanel;
    //public static PlaySkillForUI m_playSkillGo;
    private UpgradeSkillPanel m_upgradePanel;
    private AssemblySkillPanel m_assemblyPanel;
    private CommonUIBottomButtonTool m_commonUIBottomButtonTool;
    private Dictionary<int, SkillsItem> m_skillItemList = new Dictionary<int, SkillsItem>();
    private Dictionary<int, UnlockSkillItem> m_unlockSkillItemList = new Dictionary<int, UnlockSkillItem>();
    private int[] m_guideBtnID;
    //private List<int> m_itemGuideBtnID = new List<int>();

    public GameObject CommonToolPrefab;
    void Awake()
    {
        if (UI.MainUI.MainUIController.Instance.IsShowCommonTool) { CreatObjectToNGUI.InstantiateObj(CommonToolPrefab, transform); }
        m_guideBtnID = new int[2];
        RegisterEventHandler();
        m_uiCamera = UICamera.currentCamera;
        m_heroVocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;//玩家职业
        m_assemblyPanel = CreatObjectToNGUI.InstantiateObj(AssemblySkillPanel, DragParent.transform).GetComponent<AssemblySkillPanel>();
        m_upgradePanel = CreatObjectToNGUI.InstantiateObj(UpgradeSkillPanel, UpgradeSkillPanelPos).GetComponent<UpgradeSkillPanel>();
    }

    void Start()
    {
        //m_playerSkillLv = PlayerManager.Instance.FindHeroDataModel().UnitValues.sMsgPropCreateEntity_SC_UnitVisibleValue.UNIT_FIELD_LEVEL;
        //CreatObjectToNGUI.InstantiateObj(UITitlePrefab, transform);
        m_commonUIBottomButtonTool = CreatObjectToNGUI.InstantiateObj(UIBottomBtnPrefab, CreatBottomBtnPoint).GetComponent<CommonUIBottomButtonTool>();
        InitBottomBtn();
        InitSkillList();
    }

    public override void Show(params object[] value)
    {
        ResetSkillPanel();
        //if (m_roleViewPanel != null)
        //{
        //    m_roleViewPanel.Show();
        //}
        //else
        //{
        //    m_roleViewPanel = (GameObject.Instantiate(HeroModelViewPrefab) as GameObject).GetComponent<RoleViewPanel>();
        //    m_roleViewPanel.SetPanelPosition(m_uiCamera, RoleViewPoint);
        //    m_roleViewPanel.roleModelPanel.GetComponent<UICamera>().enabled = false;
        //    m_playSkillGo = m_roleViewPanel.roleModelPanel.GetHeroModel.AddComponent<PlaySkillForUI>();
        //}
        base.Show(value);
    }

    public override void Close()
    {
        if (!IsShow)
            return;
        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
        //if (m_roleViewPanel != null)
        //{
        //    m_roleViewPanel.Close();
        //}
        base.Close();
    }

    void InitBottomBtn()
    {
        CommonBtnInfo btnInfo = new CommonBtnInfo(0, "JH_UI_Button_1116_06", "JH_UI_Button_1116_00", OnBackButtonTapped);

        //if (m_heroVocation == 1)
        //    m_buttomBtn = new CommonBtnInfo(1, "JH_UI_Button_1116_02", "JH_UI_Button_1116_00", ShowUpgradeSkillBtn);
        //else
        //    m_buttomBtn = new CommonBtnInfo(1, "JH_UI_Button_1116_01", "JH_UI_Button_1116_00", ShowUpgradeSkillBtn);

        UpdatePanelState(SkillUIType.AssemblySkill);
        m_commonUIBottomButtonTool.Show(new List<CommonBtnInfo>() { btnInfo });

        var btnInfoComponet = m_commonUIBottomButtonTool.GetButtonComponent(btnInfo);
        if (btnInfoComponet != null)
        {
            //TODO GuideBtnManager.Instance.RegGuideButton(btnInfoComponet.gameObject, UIType.SkillMain, SubType.ButtomCommon, out m_guideBtnID[0]);
        }
        //var m_buttomBtnComponet = m_commonUIBottomButtonTool.GetButtonComponent(m_buttomBtn);
        //if (m_buttomBtnComponet != null)
        //{
        //    //TODO GuideBtnManager.Instance.RegGuideButton(m_buttomBtnComponet.gameObject, UIType.SkillMain, SubType.ButtomCommon, out m_guideBtnID[1]);
        //}
        
    }

    //private void ShowUpgradeSkillBtn(object obj)
    //{
    //    SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
    //    if (m_heroVocation == 1)
    //    {
    //        m_commonUIBottomButtonTool.ChangeButtonInfo(m_buttomBtn, "JH_UI_Button_1116_04", "JH_UI_Button_1116_00", ShowAssemblySkillBtn);
    //    }   
    //    else
    //    {
    //        m_commonUIBottomButtonTool.ChangeButtonInfo(m_buttomBtn, "JH_UI_Button_1116_03", "JH_UI_Button_1116_00", ShowAssemblySkillBtn);
    //    }

    //    UpdatePanelState(SkillUIType.UpgradeSkill);
    //    m_playSkillGo.BreakSkill();
    //    ItemPagerManager.SelectedItem(-1);
		
    //    var unSkillList = m_playerSkillList.singleSkillInfoList.FindAll(P => P.Lock == false);
		
    //    ItemPagerManager.InitPager(unSkillList.Count);

    //    if (m_upgradePanel != null)
    //        m_upgradePanel.ResetUpgradePanel();
    //    m_skillItemList.ApplyAllItem(P => P.Value.IsUpgradeState(false));
    //    m_unlockSkillItemList.ApplyAllItem(P => P.Value.IsShow(false));
    //}

    private void ShowAssemblySkillBtn(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");

        if (m_commonUIBottomButtonTool == null)
            return;

        //if (m_heroVocation == 1)
        //{
        //    m_commonUIBottomButtonTool.ChangeButtonInfo(m_buttomBtn, "JH_UI_Button_1116_02", "JH_UI_Button_1116_00", ShowUpgradeSkillBtn);
        //}
        //else
        //{
        //    m_commonUIBottomButtonTool.ChangeButtonInfo(m_buttomBtn, "JH_UI_Button_1116_01", "JH_UI_Button_1116_00", ShowUpgradeSkillBtn);
        //}
        
        UpdatePanelState(SkillUIType.AssemblySkill);
        ItemPagerManager.InitPager(m_playerSkillList.singleSkillInfoList.Count);

        if (m_skillItemList == null)
            return;
        m_skillItemList.ApplyAllItem(P => P.Value.IsUpgradeState(true));
        m_unlockSkillItemList.ApplyAllItem(P => P.Value.IsShow(true));
    }

    private void OnBackButtonTapped(object obj)
    {
        SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
        CleanUpUIStatus();
        Close();
    }

    private void UpdatePanelState(SkillUIType skillUIType)
    {
        m_skillUIType = skillUIType;
        

        if (m_skillUIType == SkillUIType.UpgradeSkill)
        {
            m_assemblyPanel.transform.localPosition = new Vector3(0, 0, -1000);
            //m_upgradePanel.transform.localPosition = new Vector3(0, 0, -1);
        }
        else if (m_skillUIType == SkillUIType.AssemblySkill)
        {
            m_assemblyPanel.transform.localPosition = new Vector3(0, 0, -2);
            //m_upgradePanel.transform.localPosition = new Vector3(0, 0, -1000);
        }
        else
        {
            m_assemblyPanel.transform.localPosition = new Vector3(0, 0, -1000);
            //m_upgradePanel.transform.localPosition = new Vector3(0, 0, -1000);
		}
    }

    /// <summary>
    /// 1、记下选择装备
    /// 2、从背包数据中取出装备数据，进行排序
    /// 3、遍历装备数据集，实例化SingleEquipDragPanel预设件，把装备数据传入预设件实例处理
    /// 4、选中之前记下的装备项
    /// </summary>
    public void InitSkillList()
    {
        m_playerSkillList = new SingleSkillInfoList();
        //m_assemblyPanel.GetComponent<AssemblySkillPanel>().InitSkillEquip(m_playerSkillList.EquipSkillsList);

        //foreach (var item in m_playerSkillList.singleSkillInfoList)
        //{
        //    m_playSkillGo.AddSkillBase(item.localSkillData.m_skillId);
        //}
        
        ItemPagerManager.InitPager(m_playerSkillList.singleSkillInfoList.Count);
    }

    private void ResetSkillPanel()
    {
        if(m_upgradePanel != null)
                m_upgradePanel.ResetUpgradePanel();

        ShowAssemblySkillBtn(null);

        m_playerSkillList = new SingleSkillInfoList();
        ItemPagerManager.InitPager(m_playerSkillList.singleSkillInfoList.Count);
    }

    /// <summary>
    /// 根据页码，生成数据传送到ItemPagerManager
    /// </summary>
//    private void DisplayItems(int startPage, int pageSize)
//    {
//        int startIndex = (startPage - 1) * pageSize;
//        if (m_playerSkillList.singleSkillInfoList.Count < startIndex)
//        {
//            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"分页出错 总数：" + m_playerSkillList.singleSkillInfoList.Count + " 开始:" + startIndex);
//        }
//        int itemCount = startIndex + pageSize > m_playerSkillList.singleSkillInfoList.Count ? m_playerSkillList.singleSkillInfoList.Count : startIndex + pageSize;
//
//        IPagerItem[] equipItems = new IPagerItem[itemCount - startIndex];
//        
//        GameObject equipItem;
//
//        foreach (SkillsItem item in m_skillItemList.Values)
//        {
//            DestroyImmediate(item.gameObject);
//        }
//
//        m_skillItemList.Clear();
//
//        foreach (UnlockSkillItem item in m_unlockSkillItemList.Values)
//        {
//            DestroyImmediate(item.gameObject);
//        }
//        m_unlockSkillItemList.Clear();
//        for (int i = startIndex; i < itemCount; i++)
//        {
//            var item = m_playerSkillList.singleSkillInfoList[i];
//            if (!item.Lock)
//            {
//                equipItem = GameObject.Instantiate(Prefab_SkillItem) as GameObject;
//                equipItem.name = Prefab_SkillItem.name;
//                var commonItem = equipItem.GetComponent<SkillsItem>();
//                commonItem.InitGuideID(1); //新手引导用，1代表生成ID的类型
//                if (m_skillItemList.ContainsKey(item.localSkillData.m_skillId))
//                    m_skillItemList[item.localSkillData.m_skillId] = commonItem;
//                else
//                    m_skillItemList.Add(item.localSkillData.m_skillId, commonItem);
//                commonItem.InitItemData(item);
//                equipItems[i - startIndex] = commonItem;
//
//                //int guideBtnID;
//                ////TODO GuideBtnManager.Instance.RegGuideButton(commonItem.gameObject, UIType.SkillMain, SubType.SkillMainItem, out guideBtnID);
//                //m_itemGuideBtnID.Add(guideBtnID);
//            }
//            else
//            {
//                equipItem = GameObject.Instantiate(Unlock_SkillItem) as GameObject;
//                equipItem.name = Prefab_SkillItem.name;
//
//                var commonItem = equipItem.GetComponent<UnlockSkillItem>();
//                if (m_unlockSkillItemList.ContainsKey(item.localSkillData.m_skillId))
//                    m_unlockSkillItemList[item.localSkillData.m_skillId] = commonItem;
//                else
//                    m_unlockSkillItemList.Add(item.localSkillData.m_skillId, commonItem);
//                commonItem.InitItemData(item.localSkillData.m_unlockLevel);
//                equipItems[i - startIndex] = commonItem;
//            }
//            
//        }
//        if (m_skillUIType == SkillUIType.UpgradeSkill)
//        {
//            m_skillItemList.ApplyAllItem(P => P.Value.IsUpgradeState(false));
//            m_unlockSkillItemList.ApplyAllItem(P => P.Value.IsShow(false));
//        }
//        else
//        {
//            m_skillItemList.ApplyAllItem(P => P.Value.IsUpgradeState(true));
//            m_unlockSkillItemList.ApplyAllItem(P => P.Value.IsShow(true));
//        }
//        ItemPagerManager.SelectedIndex = 0;  
//        ItemPagerManager.UpdateItems(equipItems, Prefab_SkillItem.name);
//    }
    /// <summary>
    /// Item分页处理，整理显示的数据，传给ItemPagerManager
    /// </summary>
    /// <param name="arg"></param>
//    private void ItemPageChangedHandle(PageChangedEventArg pageChangedEventArg)
//    {
//        //PageChangedEventArg pageChangedEventArg = (PageChangedEventArg)arg;
//        
//        DisplayItems(pageChangedEventArg.StartPage, pageChangedEventArg.PageSize);
//		UIEventManager.Instance.TriggerUIEvent(UIEventType.ResetDragComponentStatus, null);
//    }

    protected override void RegisterEventHandler()
    {
       // AddEventHandler(EventTypeEnum.PageChanged.ToString(), ItemPageChangedHandle);
        //ItemPagerManager.OnPageChanged += this.ItemPageChangedHandle;
        //AddEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        UIEventManager.Instance.RegisterUIEvent(UIEventType.UpgrateSkillInfo, UpgradeSkill);
        //ItemPagerManager.OnPageItemSelected += this.ItemPagerManager_OnPageItemSelected;
    }

    void OnDestroy()
    {
        //RemoveEventHandler(EventTypeEnum.PageChanged.ToString(), ItemPageChangedHandle);
        //RemoveEventHandler(EventTypeEnum.EntityUpdateValues.ToString(), UpdateViaNotify);
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.UpgrateSkillInfo, UpgradeSkill);
        //RemoveEventHandler(EventTypeEnum.PageChanged.ToString(), ItemPageChangedHandle);

        for (int i = 0; i < m_guideBtnID.Length; i++ )
        {
            //TODO GuideBtnManager.Instance.DelGuideButton(m_guideBtnID[i]);
        }
        //for (int i = 0; i < m_itemGuideBtnID.Count; i++ )
        //{
        //    //TODO GuideBtnManager.Instance.DelGuideButton(m_itemGuideBtnID[i]);
        //}
        
    }

    public void UpgradeSkill(object obj)
    {
        LoadingUI.Instance.Close();
        //技能升级
        if (typeof(SSkillInfo) == obj.GetType())
        {
            SSkillInfo sSkillInfo = (SSkillInfo)obj;
            //TraceUtil.Log("收到技能更新：ID"+sSkillInfo.wSkillID+",Lv:"+sSkillInfo.wSkillLV);
            var upgradeSkillInfo = m_playerSkillList.singleSkillInfoList.Single(P => P.localSkillData.m_skillId == sSkillInfo.wSkillID);
            upgradeSkillInfo.SkillLevel = sSkillInfo.wSkillLV;

//            if (m_skillItemList.ContainsKey(upgradeSkillInfo.localSkillData.m_skillId))
//                m_skillItemList[upgradeSkillInfo.localSkillData.m_skillId].UpdateSkillItem(upgradeSkillInfo);

            //m_assemblyPanel.UpgradeEquipedSkill(upgradeSkillInfo);
            m_upgradePanel.UpgradeSkillData(upgradeSkillInfo);
        }
    }

    /// <summary>
    /// 装备选中事件处理。获得选中的装备，取出信息。显示在信息版面上
    /// </summary>
    /// <param name="selectedItem"></param>
//    void ItemPagerManager_OnPageItemSelected(IPagerItem selectedItem)
//    {
//        SkillsItem selectedSkillItem = selectedItem as SkillsItem;
//        if (selectedSkillItem == null)
//        {
//            m_upgradePanel.SetPanelItemActive(false);
//            return;
//        }
//        if (m_upgradePanel !=null)
//            m_upgradePanel.InitUpgradeInfo(selectedSkillItem);
//        //if (m_playSkillGo == null)
//            //return;
//        //m_playSkillGo.BreakSkill();
//        //m_playSkillGo.PlaySkill(selectedSkillItem.ItemFielInfo.localSkillData.m_skillId);
//        //switch (m_skillUIType)
//        //{
//        //    case SkillUIType.AssemblySkill:
//        //        //m_playSkillGo.PlaySkill(selectedSkillItem.ItemFielInfo.localSkillData.m_skillId);
//        //        break;
//
//        //    case SkillUIType.UpgradeSkill:
//        //        m_upgradePanel.InitUpgradeInfo(selectedSkillItem);
//        //        break;
//        //    default:
//        //        break;
//        //}
//    }
}
