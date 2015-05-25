using UnityEngine;
using System.Collections;
using System.Linq;
using UI.MainUI;

namespace Guide
{
    ///// <summary>
    ///// TownUI场景中的NewbieGuideUI物体
    ///// </summary>
    //public class TownGuideUIManger_V2 : View
    //{
    //    /// <summary>
    //    /// 任务面板
    //    /// </summary>
    //    public GameObject TaskPanel;
    //    /// <summary>
    //    /// 城镇对话框面板
    //    /// </summary>
    //    public GameObject GuideDialogPanel;
    //    /// <summary>
    //    /// 任务完成特效1
    //    /// </summary>
    //    public GameObject ComplateEffect;
    //    /// <summary>
    //    /// 任务完成特效2
    //    /// </summary>
    //    public GameObject TaskComplateEffectB;
    //    /// <summary>
    //    /// 引导遮罩
    //    /// </summary>
    //    public BoxCollider NebieGuideMask;
    //    public Camera UICamera;

    //    private TaskPanel_V3 m_taskPanel;
    //    private GameObject m_dialogPanel;  //对话UI面板
    //    private GameObject m_btnSignPanel;  //引导提示框
    //    /// <summary>
    //    ///  当前引导序号
    //    /// </summary>
    //    private int m_guideOrder = 0;

    //    private GameObject m_sourceFrame;  //被拖拽按钮亮框或者是引导按钮亮框
    //    private GameObject m_targetFrame;  //被拖拽目标区域亮框
    //    private GameObject m_draggingArrow;  //拖拽的手
    //    private GameObject m_completeEffect;  //引导完成特效
    //    private GameObject m_completeEffectB;  //引导完成特效
    //    /// <summary>
    //    /// 当前引导数据
    //    /// </summary>
    //    private GuideConfigData m_curGuideData;  //当前引导数据
    //    /// <summary>
    //    /// 当前任务数据
    //    /// </summary>
    //    private TaskConfigData m_curTaskData;  //当前任务数据
    //    private Transform m_NpcTrans = null;
    //    private int m_packViewBtnId;

    //    private bool m_isOverEvent = false;
    //    // Use this for initialization
    //    void Awake()
    //    {           
    //        this.RegisterEventHandler();
    //    }

    //    private static TownGuideUIManger_V2 m_instance;
    //    public static TownGuideUIManger_V2 Instance{ get{ return m_instance; } }
		
    //    private GameObject m_sourceBtn;
    //    /// <summary>
    //    /// 任务是否完成标记【为了从副本返回城镇时获得之前城镇的任务状态，所以设为静态变量】
    //    /// </summary>
    //    private static bool m_isComplateTaskFromLocal = true;  //

    //    void Start()
    //    {
    //        m_instance = this;
    //        NebieGuideMask.enabled = false;

    //        if (!m_isComplateTaskFromLocal)
    //            NewbieGuideManager_V2.Instance.CheckNextTaskStart();
    //    }

    //    void FixedUpdate()
    //    {
    //        if (NewbieGuideManager_V2.Instance.m_isComplateTaskFromServer)
    //        {
    //            NewbieGuideManager_V2.Instance.m_isComplateTaskFromServer = false;
    //            CompleteGuide(NewbieGuideManager_V2.Instance.m_enableButton);
    //        }

    //        if (NewbieGuideManager_V2.Instance.m_isStartTask)
    //        {
    //            StartTaskGuide(NewbieGuideManager_V2.Instance.ExecuteTask);
    //            NewbieGuideManager_V2.Instance.m_isStartTask = false;
    //            m_isComplateTaskFromLocal = false;
    //        }
    //        else if(m_isComplateTaskFromLocal)
    //        {
    //            NewbieGuideManager_V2.Instance.CheckNextTaskStart();
    //        }
    //    }

    //    public void StartTaskGuide(TaskConfigData taskData)
    //    {
    //        if (taskData != null)
    //        {
    //            m_curTaskData = taskData;

    //            if (m_taskPanel == null)
    //            {
    //                m_taskPanel = (Instantiate(TaskPanel) as GameObject).GetComponent<TaskPanel_V3>();
    //                m_taskPanel.transform.parent = this.transform;
    //                m_taskPanel.transform.localPosition = new Vector3(0, 0, 150);
    //                m_taskPanel.transform.localScale = Vector3.one;
    //                m_taskPanel.GetComponent<UIAnchor>().uiCamera = UICamera;
    //            }
    //            if (!m_taskPanel.gameObject.activeSelf)
    //            {
    //                m_taskPanel.gameObject.SetActive(true);
    //            }

    //            SoundManager.Instance.PlaySoundEffect("Sound_Voice_GetNewQuest");

    //            m_taskPanel.InitTaskPanel(taskData);

    //            if (taskData._GuideType == 1)  //强引导&弱引导
    //                NewbieGuideManager_V2.Instance.IsConstraintGuide = true;
    //            else
    //                NewbieGuideManager_V2.Instance.IsConstraintGuide = false;
    //        }
    //    }

    //    void CloseAllUI()
    //    {
    //        MainUIController.Instance.CloseAllPanel();
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, null);
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseAllUI, null);
    //    }

    //    void OpenSystemMainButton()
    //    {
    //        CloseAllUI();
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.OpenSystemButton, null);
    //    }
		
		
    //    public void ContinueGuideButton()
    //    {
    //        switch (m_curTaskData._CloseUI)
    //        {
    //            case 1:
    //                CloseAllUI();
    //                break;
    //            case 2:
    //                OpenSystemMainButton();
    //                break;
    //            default:
    //                break;
    //        }
			
    //        ContinueGuide();
    //    }

    //    public void ContinueGuide()
    //    {
    //        m_isOverEvent = true;
    //        GuideBtnManager.Instance.IsEndGuide = false;

    //        if (NewbieGuideManager_V2.Instance.TownGuideList.Count > m_guideOrder)
    //        {
    //            m_curGuideData = NewbieGuideManager_V2.Instance.TownGuideList[m_guideOrder];
    //            if (m_curGuideData._GuideType == 4)
    //            {
    //                //通过物品Id查找物品所在的位置，并根据位置生成该位置的ButtonId
    //                PackInfo_V3 getPanel = MainUIController.Instance.GetPanel(UIType.Package) as PackInfo_V3;
    //                int goodsID = m_curGuideData._GuideBtnID[0];
    //                int pos = getPanel.containerPackList.TurningToPage(goodsID);
    //                //如果返回-1，表示找不到物品。直接中断引导
    //                if(pos==-1)
    //                {
    //                    return;
    //                }
    //                var containerBoxSlot_V2=getPanel.containerPackList.ContainerBoxList[pos];
    //                m_packViewBtnId = containerBoxSlot_V2.MyContainerBox.GuideID;
    //                TraceUtil.Log("_GuideType == 4:" + m_packViewBtnId);
    //            }
    //            ShowGuideDialog(m_curGuideData);
    //        }
    //    }
    //    /// <summary>
    //    /// 任务完成处理，播放特效，开启新功能
    //    /// </summary>
    //    /// <param name="enableFuncList"></param>
    //    public void CompleteGuide(UIType enableFuncList)
    //    {
    //        if (m_completeEffect != null)
    //            Destroy(m_completeEffect);

    //        m_isComplateTaskFromLocal = true;
    //        StopGuideHandle(null);
    //        GuideBtnManager.Instance.IsEndGuide = true;
    //        NewbieGuideManager_V2.Instance.IsConstraintGuide = false;
    //        m_completeEffect = Instantiate(ComplateEffect) as GameObject;
    //        m_completeEffect.transform.parent = this.transform;
    //        m_completeEffect.transform.localScale = Vector3.one;
            
    //        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_CompleteQuest");
    //        PlayTaskEffect();
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.ShowMissionFailPanelLate,null);
    //        UIEventManager.Instance.TriggerUIEvent(UIEventType.EnableMainButton, enableFuncList);
    //    }

    //    //void OnGUI()
    //    //{
    //    //    if (GUILayout.Button("DDDDDDDDDDDDDDDDD"))
    //    //    {
    //    //        if (m_completeEffect != null)
    //    //            DestroyImmediate(m_completeEffect);

    //    //        m_completeEffect = Instantiate(ComplateEffect) as GameObject;
    //    //        m_completeEffect.transform.parent = this.transform;
    //    //        m_completeEffect.transform.localScale = Vector3.one;

    //    //        PlayTaskEffect();
    //    //    }
    //    //}

    //    void PlayTaskEffect()
    //    {
    //        if (m_taskPanel == null)
    //            return;

    //        if (m_completeEffectB != null)
    //            DestroyImmediate(m_completeEffectB);

    //        m_completeEffectB = Instantiate(TaskComplateEffectB) as GameObject;
    //        m_completeEffectB.transform.parent = m_taskPanel.transform;
    //        m_completeEffectB.transform.localPosition = Vector3.zero;
    //        m_completeEffectB.transform.localScale = Vector3.one;
    //    }

    //    /// <summary>
    //    /// 设置对话面板
    //    /// </summary>
    //    /// <param name="item"></param>
    //    private void ShowGuideDialog(GuideConfigData item)
    //    {
    //        if (item._PreDialogList[0] == "0" || item._PreDialogList[0].Length <= 1)  //不需要对话框，直接跳过
    //        {
    //            DialogEndHandle();
    //            return;
    //        }

    //        if (m_dialogPanel == null)
    //        {
    //            m_dialogPanel = (GameObject)GameObject.Instantiate(GuideDialogPanel, this.transform.position, Quaternion.identity);
    //            m_dialogPanel.transform.parent = this.transform;
    //            m_dialogPanel.transform.localScale = Vector3.one;
    //        }
    //        m_dialogPanel.GetComponent<NewbieDialogPanel>().InitDialogPanel(item); 
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="notifyArgs"></param>
    //    public void DialogEndHandle()
    //    {
            
    //        if (m_curTaskData._TaskType == 15 && m_curGuideData._GuideBtnID [0] == 0) 
    //        {
    //            m_guideOrder = 0;
    //            StopGuideHandle(null);
    //            m_isOverEvent = false;
    //            //NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
    //        }
    //        else
    //        {
    //            NebieGuideMask.enabled = true;
    //            Invoke("OverRule", 0.7f);
    //        }
    //    }

    //    /// <summary>
    //    /// 设置引导按钮的高光亮圈
    //    /// </summary>
    //    private void SetSourceFrame(Transform btn)
    //    {
    //        var offsetPos = m_curGuideData._BtnPosOffset;
    //        if (m_sourceFrame == null)
    //        {
    //            m_sourceFrame = (GameObject)GameObject.Instantiate(m_curGuideData.SourceFrame , Vector3.zero, Quaternion.identity);
    //            m_sourceFrame.transform.parent = btn;
    //            m_sourceFrame.transform.localScale = Vector3.one;
    //        }
    //        //偏移光圈和BoxCollider      
    //        m_sourceFrame.transform.localPosition = btn.position + offsetPos;
    //        TraceUtil.Log(m_curGuideData._GuideID+"  "+btn.name + "  " + m_sourceFrame.name + "  " + offsetPos);
    //        var collider = btn.GetComponent<BoxCollider>();
    //        collider.center += offsetPos;


    //    }

    //    private void SetTargetFrame(Transform btn)
    //    {
    //        if (m_targetFrame == null)
    //        {
    //            m_targetFrame = (GameObject)GameObject.Instantiate(m_curGuideData.TargetFrame, Vector3.zero, Quaternion.identity);
    //            m_targetFrame.transform.parent = btn.parent;
    //            m_targetFrame.transform.localScale = new Vector3(284, 98, 1);
    //        }
			
    //        m_targetFrame.transform.localPosition = btn.localPosition;
    //    }

    //    /// <summary>
    //    /// 设置引导的箭头指示
    //    /// </summary>
    //    /// <param name="item"></param>
    //    private void ShowGuideArrow(GuideConfigData item, Transform btn)
    //    {
    //        Vector3 pos = btn.position;
    //        pos.z = -1f;
          
    //        if (m_btnSignPanel == null)
    //        {
    //            m_btnSignPanel = (GameObject)GameObject.Instantiate(item.ArrowPrefab, Vector3.zero, Quaternion.identity);
    //            m_btnSignPanel.transform.parent = btn;
    //            m_btnSignPanel.transform.localScale = Vector3.one;               
    //        }
    //        //偏移引导箭头
    //        var offsetPos = m_curGuideData._BtnPosOffset;
    //        //m_btnSignPanel.transform.localPosition = btn.position + new Vector3(item._ArrowOffsetX,item._ArrowOffsetY)+ offsetPos;
    //        m_btnSignPanel.transform.position = new Vector3(pos.x + item._ArrowOffsetX , pos.y + item._ArrowOffsetY , pos.z) ; //+ item._BtnSignOffsetX + item._BtnSignOffsetY
    //        m_btnSignPanel.transform.localPosition += offsetPos;

    //        TraceUtil.Log(m_curGuideData._GuideID + "  " + btn.name + "  " + m_btnSignPanel.name + "  " + offsetPos);

    //        m_btnSignPanel.GetComponent<BtnSignPanel>().InitSignPanel(item);

           
    //    }

    //    /// <summary>
    //    /// 判断强化是否满足所需金
    //    /// </summary>
    //    /// <param name="itemFielInfo"></param>
    //    /// <param name="playerMoney"></param>
    //    /// <returns></returns>
    //    private bool NormalStrengthenConsume(ItemFielInfo itemFielInfo, int playerMoney)
    //    {
    //        ItemData itemData = itemFielInfo.LocalItemData;
    //        EquipmentEntity equipmentEntity = itemFielInfo.equipmentEntity;
    //        int normalStrengthenLv = equipmentEntity.EQUIP_FIELD_STRONGE_LEVEL + 1;
    //        var equipItemData = ItemDataManager.Instance.GetItemData(itemData._goodID) as EquipmentData;
    //        var costParameter = equipItemData._StrengthCost;

    //        var consumeValue = Mathf.FloorToInt((costParameter[0] * Mathf.Pow(normalStrengthenLv, 2) + costParameter[1] * normalStrengthenLv + costParameter[2]) / costParameter[3]) * costParameter[3];

    //        return (consumeValue > playerMoney);
    //    }

    //    /// <summary>
    //    /// 按钮终止规则
    //    /// </summary>
    //    void OverRule()
    //    {
    //        NebieGuideMask.enabled = false;

    //        var playerMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;  //当前玩家金币

    //        ///如果在当前按钮管理列表中无当前引导按钮ID，则判断当前按钮是否可以跳过，否则就会报错
    //        switch (m_curGuideData._OverRole)
    //        {
    //            case 1: //铜币小于背包中第一个装备强化需求的铜币。或者背包中第一个不是武器装备。
    //                var equipDatainfoList = ContainerInfomanager.Instance.itemFielArrayInfo.Where(P => P.severItemFielType == SeverItemFielInfoType.Equid && ((EquipmentData)P.LocalItemData)._StrengFlag == 1).ToList();
    //                if (NormalStrengthenConsume(equipDatainfoList[0], playerMoney))
    //                {
    //                    StopGuideHandle("终止规则1：铜币小于背包中第一个装备强化需求的铜币。");
    //                    return;
    //                }
    //                break;
    //            case 2:  //身上所有的铜币小于技能栏第一个技能升级需要的铜币。
    //                var skillFirst = SkillsPanelManager.m_playerSkillList.singleSkillInfoList[0];
    //                int upgradeMoney = SkillValue.GetSkillValue(skillFirst.SkillLevel, skillFirst.localSkillData.m_upgradeMoneyParams);
    //                if (skillFirst.Lock || upgradeMoney > playerMoney)
    //                {
    //                    StopGuideHandle("终止规则2：身上所有的铜币小于技能栏第一个技能升级需要的铜币。");
    //                    return;
    //                }
    //                break;
    //            case 3: //当天的剩余活力小于8。
    //                var playerActiveLife = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE; //玩家活力
    //                if (playerActiveLife < 8)
    //                {
    //                    StopGuideHandle("终止规则3：当天的剩余活力小于8。");
    //                    return;
    //                }
    //                break;
    //            case 4: //身上的铜币小于100。
    //                if (playerMoney < 100)
    //                {
    //                    StopGuideHandle("终止规则4：身上的铜币小于100。");
    //                    return;
    //                }
    //                break;
    //            case 5: //身上的元宝数小于10。
    //                var playerGold = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;  //玩家元宝
    //                if (playerGold < 10)
    //                {
    //                    StopGuideHandle("终止规则5：身上的元宝数小于10。");
    //                    return;
    //                }
    //                break;
    //            case 6: //当天的PVP次数为0。
    //                var pvpTimes = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PVP_TIMES;  //PVP次数
    //                if (pvpTimes == 0)
    //                {
    //                    StopGuideHandle("终止规则6：当天的PVP次数为0。");
    //                    return;
    //                }
    //                break;
    //            default:
    //                break;
    //        }

    //        //进行跨过规则判断
    //        SkipRule(m_curGuideData);
            

    //    }

    //    /// <summary>
    //    /// 按钮跳过规则
    //    /// </summary>
    //    void SkipRule(GuideConfigData curGuideItem)
    //    {
    //        var playerMoney = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_HOLDMONEY;  //当前玩家金币
            
    //        ///如果在当前按钮管理列表中无当前引导按钮ID，则判断当前按钮是否可以跳过，否则就会报错
    //        switch (curGuideItem._SkipRole)
    //        {
    //            case 1: //铜币小于背包中第一个装备强化需求的铜币。或者背包中第一个不是武器装备。
    //                var equipDatainfoList = ContainerInfomanager.Instance.itemFielArrayInfo.Where(P => P.severItemFielType == SeverItemFielInfoType.Equid && ((EquipmentData)P.LocalItemData)._StrengFlag == 1).ToList();
    //                if (NormalStrengthenConsume(equipDatainfoList[0], playerMoney))
    //                {
    //                    NextGuideHandle("跳过规则1：铜币小于背包中第一个装备强化需求的铜币。或者背包中第一个不是武器装备。");
    //                    return;
    //                }
    //                break;
    //            case 2:  //身上所有的铜币小于技能栏第一个技能升级需要的铜币。
    //                var skillFirst = SkillsPanelManager.m_playerSkillList.singleSkillInfoList[0];
    //                int upgradeMoney = SkillValue.GetSkillValue(skillFirst.SkillLevel, skillFirst.localSkillData.m_upgradeMoneyParams);
    //                if(skillFirst.Lock || upgradeMoney > playerMoney)
    //                {
    //                    NextGuideHandle("跳过规则2：身上所有的铜币小于技能栏第一个技能升级需要的铜币。。");
    //                    return;
    //                }
    //                break;
    //            case 3: //当天的剩余活力小于8。
    //                var playerActiveLife = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_CURRENCY_ACTIVELIFE; //玩家活力
    //                if(playerActiveLife < 8)
    //                {
    //                    NextGuideHandle("跳过规则3：当天的剩余活力小于8。");
    //                    return;
    //                }
    //                break;
    //            case 4: //身上的铜币小于100。
    //                if (playerMoney < 100)
    //                {
    //                    NextGuideHandle("跳过规则4：身上的铜币小于100。");
    //                    return;
    //                }
    //                break;
    //            case 5: //身上的元宝数小于10。
    //                var playerGold = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_BINDPAY;  //玩家元宝
    //                if (playerGold < 10)
    //                {
    //                    NextGuideHandle("跳过规则5：身上的元宝数小于10。");
    //                    return;
    //                }
    //                break;
    //            case 6: //当天的PVP次数为0。
    //                var pvpTimes = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PLAYER_FIELD_PVP_TIMES;  //PVP次数
    //                if (pvpTimes == 0)
    //                {
    //                    NextGuideHandle("跳过规则6：当天的PVP次数为0。");
    //                    return;
    //                }
    //                break;
    //            case 7:  //当果实成熟时跳过此步引导
    //                var fruitPointGo = GuideBtnManager.Instance.GetButtonList[curGuideItem._GuideBtnID[0]].GuideBtn;
    //                if (fruitPointGo != null)
    //                {
    //                    TreasureTreesFruitPoint fruitPoint = fruitPointGo.GetComponent<TreasureTreesFruitPoint>();
    //                    if ((FruitPrucStatusType)fruitPoint.MyFruitData.byFruitStatus == FruitPrucStatusType.RIPEN_FRUIT_STATUS_TYPE)
    //                    {
    //                        NextGuideHandle("跳过规则7：当前被引导的果实成熟。");
    //                        return;
    //                    }
    //                }
    //                break;
    //            case 8: //如果按钮ID不存在跳过此步引导
    //                if (!GuideBtnManager.Instance.GetButtonList.ContainsKey(curGuideItem._GuideBtnID[0]))
    //                {
    //                    NextGuideHandle("跳过规则8：当前被引导的ID不存在，则跳过。");
    //                    return;
    //                }
    //                break;
    //            case 9: //如果当前被引导妖女已解锁
    //                if (SirenDataManager.Instance.CurSelectSiren.IsUnlock())
    //                {
    //                    NextGuideHandle("跳过规则9：当前被引导妖女已解锁，则跳过。");
    //                    return;
    //                }
    //                break;
    //            case 10: //如果系统菜单已经展开
    //                if (UI.SystemFuntionButton.Instance.Show)
    //                {
    //                    NextGuideHandle("跳过规则10：系统菜单已经展开，则跳过。");
    //                    return;
    //                }
    //                break;
    //            case 11: //如果当前客户端没有打开NPC对话框
    //                if (MainUIController.Instance.CurrentUIStatus!=UIType.NPCTalkPanel)
    //                {
    //                    NextGuideHandle("跳过规则11：当前客户端没有打开NPC对话框，则跳过。");
    //                    return;
    //                }
    //                break;
    //            case 12: //如果当前客户端没有显示背包界面
    //                if (MainUIController.Instance.CurrentUIStatus != UIType.Package)
    //                {
    //                    NextGuideHandle("跳过规则12：当前客户端没有显示背包界面，则跳过。");
    //                    return;
    //                }
    //                break;
    //            case 13: //如果当前客户端没有显示技能界面
    //                if (MainUIController.Instance.CurrentUIStatus != UIType.Skill)
    //                {
    //                    NextGuideHandle("跳过规则13：当前客户端没有显示技能界面，则跳过。");
    //                    return;
    //                }
    //                break;
    //            case 14: //如果当前客户端没有显示炼妖界面
    //                if (MainUIController.Instance.CurrentUIStatus != UIType.Siren)
    //                {
    //                    NextGuideHandle("跳过规则14：当前客户端没有显示炼妖界面，则跳过。");
    //                    return;
    //                }
    //                break;
    //            //case 15: //如果当前客户端没有显示强化界面
    //            //    if (MainUIController.Instance.CurrentUIStatus != UIType.EquipStrengthen)
    //            //    {
    //            //        NextGuideHandle("跳过规则15：当前客户端没有显示强化界面，则跳过。");
    //            //        return;
    //            //    }
    //                break;
    //            case 16: //如果当前客户端没有显示宝树界面
    //                if (MainUIController.Instance.CurrentUIStatus != UIType.Treasure)
    //                {
    //                    NextGuideHandle("跳过规则16：如果当前客户端没有显示宝树界面，则跳过。");
    //                    return;
    //                }
    //                break;
    //            //case 17: //如果当前客户端没有显示武馆界面
    //            //    if (MainUIController.Instance.CurrentUIStatus != UIType.MartialArtsRoom)
    //            //    {
    //            //        NextGuideHandle("跳过规则17：如果当前客户端没有显示武馆界面，则跳过。");
    //            //        return;
    //            //    }
    //            //    break;
    //            //case 18: //当前客户端没有显示经脉界面
    //            //    if (MainUIController.Instance.CurrentUIStatus != UIType.Meridians)
    //            //    {
    //            //        NextGuideHandle("跳过规则18：当前客户端没有显示经脉界面，则跳过。");
    //            //        return;
    //            //    }
    //            //    break;
    //            //case 19: //如果当前客户端没有显示时装界面
    //            //    if (MainUIController.Instance.CurrentUIStatus != UIType.Fashion)
    //            //    {
    //            //        NextGuideHandle("跳过规则19：如果当前客户端没有显示时装界面，则跳过。");
    //            //        return;
    //            //    }
    //            //    break;
    //            case 20: //如果妖女界面为显示状态
    //                if (MainUIController.Instance.CurrentUIStatus == UIType.Siren)
    //                {
    //                    NextGuideHandle("跳过规则20：如果当妖女界面为显示状态，则跳过。");
    //                    return;
    //                }
    //                break;
    //            default:
    //                break;
    //        }    

    //        //如果无须跳过则屏蔽其它按钮，显示当前按钮，并显示亮圈
    //        SetActiveButton();
    //    }       

    //    void PathFinding()
    //    {
    //        if (m_NpcTrans == null)
    //            return;

    //        float distance = Vector3.Distance(m_NpcTrans.position, PlayerManager.Instance.FindHero().transform.position);

    //        if (distance < ConfigDefineManager.DISTANCE_ARRIVED_NPC)
    //        {
    //            PlayerManager.Instance.HeroAgent.enabled = false;
    //            //RaiseEvent(EventTypeEnum.TargetSelected.ToString(), new TargetSelected() { Target = m_NpcTrans, Type = ResourceType.NPC });
    //            CancelInvoke("PathFinding");
    //            NextGuideHandle(null);
    //        }
    //    }

    //    void SetActiveButton()
    //    {
    //        int guideBtnID = m_curGuideData._GuideBtnID[0];
    //        TraceUtil.Log("#####GuideType" + m_curGuideData._GuideType + "####GuideBtnID:" + guideBtnID);

    //        if (m_curGuideData._GuideType == 3)
    //        {
    //            //寻路引导，先把所有按钮关闭
    //            GuideBtnManager.Instance.DisableButtonList();

    //            m_NpcTrans = NPCManager.Instance.GetNpcTransform(guideBtnID);
    //            var playerBehaviour = (PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour;

    //            var nav = PlayerManager.Instance.HeroAgent;
    //            nav.speed = playerBehaviour.WalkSpeed;
    //            nav.enabled = true;
    //            nav.updateRotation = true;
    //            nav.updatePosition = true;
    //            nav.SetDestination(m_NpcTrans.position);
    //            playerBehaviour.FSMSystem.PerformTransition(Transition.PlayerToNpc);

    //            InvokeRepeating("PathFinding", 0.5f, 0.5f);                
    //        }
    //        else
    //        {
    //            if (m_curGuideData._GuideType == 4)
    //            {
    //                guideBtnID = m_packViewBtnId;
    //            }
    //            if (GuideBtnManager.Instance.GetButtonList.ContainsKey(guideBtnID))
    //            {
    //                m_sourceBtn = GuideBtnManager.Instance.GetButtonList[guideBtnID].GuideBtn;
    //                if (m_sourceBtn != null)
    //                {
    //                    if (m_sourceBtn.GetComponent<GuideButtonEvent>() != null)
    //                        m_sourceBtn.GetComponent<GuideButtonEvent>().IsEnable = false;
    //                    else
    //                    {
    //                        m_sourceBtn.AddComponent<GuideButtonEvent>();
    //                        m_sourceBtn.GetComponent<GuideButtonEvent>().IsEnable = false;
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                TraceUtil.Log("指定的ButtonID:" + guideBtnID + "不存在！！");
    //                StopGuideHandle(null);
    //                return;
    //            }

    //            //sourceTransform.RemoveComponent<GuideButtonEvent>("GuideButtonEvent");
    //            SetSourceFrame(m_sourceBtn.transform);

    //            switch (m_curGuideData._GuideType)
    //            {
    //                case 0:     //普通引导箭头  
    //                case 4:     //智能物品ID箭头  
    //                    ShowGuideArrow(m_curGuideData, m_sourceBtn.transform);
    //                    if (m_curTaskData._GuideType == 0)
    //                        GuideBtnManager.Instance.SetWeakClickButton(guideBtnID);
    //                    else
    //                        GuideBtnManager.Instance.SetEnforceClickButton(guideBtnID);
    //                    break;
    //                case 1:     //智能引导箭头
    //                    var containerID = m_sourceBtn.GetComponent<LocalEctypePanel_v3>().ContainerID;
    //                    UIEventManager.Instance.TriggerUIEvent(UIEventType.EctypePageSkip, containerID);
    //                    ShowGuideArrow(m_curGuideData, m_sourceBtn.transform);
    //                    if (m_curTaskData._GuideType == 0)
    //                        GuideBtnManager.Instance.SetWeakClickButton(guideBtnID);
    //                    else
    //                        GuideBtnManager.Instance.SetEnforceClickButton(guideBtnID);
    //                    break;
    //                case 2:     //拖动引导箭头
    //                    var guideBtn = GuideBtnManager.Instance.GetButtonList[m_curGuideData._GuideBtnID[1]].GuideBtn;
    //                    if (guideBtn != null)
    //                    {
    //                        var targetTransform = GuideBtnManager.Instance.GetButtonList[m_curGuideData._GuideBtnID[1]].GuideBtn.transform;
    //                        if (m_curTaskData._GuideType == 0)
    //                            GuideBtnManager.Instance.SetWeakClickButton(guideBtnID);
    //                        else
    //                            GuideBtnManager.Instance.SetEnforceClickButton(guideBtnID); //设置拖动引导为强引导
    //                        NewbieGuideManager_V2.Instance.DragSourceBtnID = guideBtnID;
    //                        NewbieGuideManager_V2.Instance.DragTargetBtnID = m_curGuideData._GuideBtnID[1];
    //                        SetTargetFrame(targetTransform);
    //                        ShowDraggingArrow(m_curGuideData, m_sourceBtn.transform, targetTransform);
    //                        var targetSlot1 = targetTransform.GetComponent<HeroEquipMedicineSlot>();
    //                        if (null != targetSlot1)
    //                        {
    //                            targetSlot1.CanDrag = false;
    //                        }
    //                        else
    //                        {
    //                            var targetSlot2 = targetTransform.GetComponent<HeroEquiptBoxSlot_V2>();
    //                            if (targetSlot2 != null)
    //                            {
    //                                targetSlot2.CanDrag = false;
    //                            }
    //                            else
    //                            {
    //                                TraceUtil.Log(targetTransform.name + " 找不到组件去设置CanDrag属性");
    //                            }
    //                        }
    //                    }
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// Shows the dragging arrow.
    //    /// </summary>
    //    private void ShowDraggingArrow(GuideConfigData item, Transform srcTrans, Transform targetTrans)
    //    {
    //        if (m_draggingArrow == null)
    //        {
    //            m_draggingArrow = (GameObject)GameObject.Instantiate(item.ArrowPrefab, Vector3.zero, Quaternion.identity);
    //            m_draggingArrow.transform.parent = srcTrans.parent;
    //            m_draggingArrow.transform.localScale = Vector3.one;
    //        }

    //        var srcTransPoint = UICamera.ViewportToWorldPoint(srcTrans.position);
    //        var targetTransPoint = UICamera.ViewportToWorldPoint( targetTrans.position);
    //        //var direction = targetTransPoint - srcTransPoint;

    //        m_draggingArrow.transform.localPosition = srcTrans.localPosition + new Vector3(0, -40f, 0);
    //        m_draggingArrow.GetComponent<DraggingArrow>().InitDragging(srcTransPoint, targetTransPoint);
    //    }

    //    protected override void RegisterEventHandler()
    //    {
    //        UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickOtherButton, StopGuideHandle);
    //        UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickTheGuideBtn, NextGuideHandle);
    //        AddEventHandler(EventTypeEnum.NPCInteraction.ToString(), CreateNpcTalkPanel);
    //    }
    //    /// <summary>
    //    /// 点击NPC,需要关闭引导
    //    /// </summary>
    //    /// <param name="notifyArgs"></param>
    //    public void CreateNpcTalkPanel(INotifyArgs notifyArgs)
    //    {
    //        DelGuideArrow();
    //    }
    //    /// <summary>
    //    /// 单击引导按钮后的回调函数
    //    /// </summary>
    //    /// <param name="notifyArgs"></param>
    //    private void NextGuideHandle(object obj)
    //    {
    //        if (m_curGuideData._GuideType == 2)  //如果是拖动引导，则不响应点击
    //        {
    //            return;
    //        }
    //        if (obj != null)
    //        {
    //            TraceUtil.Log("NextGuideHandle" + obj.ToString());
    //        }
			
    //        DelGuideArrow();
    //        if(m_sourceBtn)
    //        {
    //        if(m_sourceBtn.GetComponent<GuideButtonEvent>())
    //            m_sourceBtn.GetComponent<GuideButtonEvent>().IsEnable = true;
    //        }
    //        //GuideBtnManager.Instance.GetButtonList[m_curGuideData._GuideBtnID[0]].AddComponent<GuideButtonEvent>();
    //        ResetNewbieGuide();
    //    }


    //    /// <summary>
    //    /// 引导结束处理，1、对话引导结束，2、服务器下发引导结束，3、要引导的按钮不存在时，4、重置新手引导时
    //    /// </summary>
    //    /// <param name="notifyArgs"></param>
    //    private void StopGuideHandle(object obj)
    //    {
    //        if (obj != null)
    //        {
    //            TraceUtil.Log((string)obj);
    //        }
			
    //        DelGuideArrow();
			
    //        if (!m_isOverEvent)
    //            return;
    //        TraceUtil.Log("OtherButtonHandle");

    //        if (m_taskPanel != null)
    //        {
    //            if (NewbieGuideManager_V2.Instance.ExecuteTask != null)
    //            {
    //                m_taskPanel.GetComponent<TaskPanel_V3>().ResetContinue();
    //            }
    //            else
    //            {
    //                m_taskPanel.gameObject.SetActive(false);
    //            }
    //        }

    //        m_guideOrder = 0;

    //        if (m_curGuideData != null)
    //        {
    //            GuideBtnManager.Instance.CloseGuide(m_curGuideData._GuideBtnID[0]);
    //        }
    //        NewbieGuideManager_V2.Instance.DragSourceBtnID = 0;
    //        NewbieGuideManager_V2.Instance.DragTargetBtnID = 0;

    //        GuideBtnManager.Instance.IsEndGuide = true;
    //    }

    //    /// <summary>
    //    /// 重置新手引导
    //    /// </summary>
    //    private void ResetNewbieGuide()
    //    {
    //        if (m_guideOrder < NewbieGuideManager_V2.Instance.TownGuideList.Count - 1)
    //        {
    //            m_guideOrder += 1;
    //            ContinueGuide();
    //        }
    //        else
    //        {
    //            m_guideOrder = 0;
    //            StopGuideHandle(null);
    //            m_isOverEvent = false;
    //            //NetServiceManager.Instance.InteractService.SendEctypeDialogOver();
    //        }
    //    }

    //    public void DelGuideArrow()
    //    {
    //        if(m_sourceFrame != null)
    //            DestroyImmediate(m_sourceFrame);
    //        if (m_btnSignPanel != null)
    //            DestroyImmediate(m_btnSignPanel);
    //        if (m_draggingArrow != null)
    //            DestroyImmediate(m_draggingArrow);
    //        if (m_targetFrame != null)
    //            DestroyImmediate(m_targetFrame);
    //    }

    //    void OnDestroy()
    //    {
    //        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClickOtherButton, StopGuideHandle);
    //        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClickTheGuideBtn, NextGuideHandle);
    //        m_instance = null;
    //    }
    //}


}