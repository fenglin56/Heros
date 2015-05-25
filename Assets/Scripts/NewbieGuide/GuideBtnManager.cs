using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using System;

/// <summary>
/// 引导按钮管理器，管理城镇与副本中的UI按钮
/// </summary>
public class GuideBtnManager:ISingletonLifeCycle
{
    /// <summary>
    /// 按钮ID与按钮及参数的Mapping，注册按钮的时候加入
    /// </summary>
    private Dictionary<int, GuideBtnParam> m_guideBtnParam = new Dictionary<int, GuideBtnParam>();
	private List<GameObject> m_draggableList = new List<GameObject>(); //拖动面板管理
    /// <summary>
    /// 重置标志，在开启新的引导按钮时判断是否已经重置所有按钮
    /// </summary>
    private bool m_isResetFlag = false;
	private bool m_isEndGuide = true;    

    private static GuideBtnManager m_instance;
    public static GuideBtnManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new GuideBtnManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
   
    /// <summary>
    /// 注册拖动面板
    /// </summary>
    /// <param name="draggable"></param>
    public void RegDraggablePanel(GameObject draggable)
    {
        if (draggable != null)
        {
            if (m_draggableList.Exists(P => P == draggable))
                return;

            m_draggableList.Add(draggable);
        }
    }
	

    ///<summary>
    /// 将引导按钮添加到按钮列表管理器中
    /// </summary>
    /// <param name="btnItem"></param>
    public void RegGuideButton(GameObject btnGO, UIType uiType, SubType subUIType, out int btnID)
    {
        RegGuideButton(btnGO, uiType, subUIType, null, out btnID);

    }
    public void RegGuideButton(GameObject btnGO, UIType uiType, SubType subUIType,Action<bool> customerActHandler, out int btnID)
    {
        btnID = CalculateBtnID(uiType, subUIType);

        if (!m_guideBtnParam.ContainsKey(btnID))
        {
            GuideButtonEvent guideButtonEvent = btnGO.GetComponent<GuideButtonEvent>();
            if (guideButtonEvent == null)
                guideButtonEvent = btnGO.AddComponent<GuideButtonEvent>();
            guideButtonEvent.InitCustomerBtnEnableHandler(customerActHandler);

            m_guideBtnParam.Add(btnID, new GuideBtnParam(btnGO,btnGO.collider.enabled));

            //如果如果引导尚未结束并当前为强引导，则设置该按钮不可用
            if (!this.IsEndGuide && TaskModel.Instance.TaskGuideType == TaskGuideType.Enforce )//NewbieGuideManager_V2.Instance.IsConstraintGuide)
            {
                //btnGO.SetButtonStatus(false);
            }
        }       
    }
    ///<summary>
    /// 将引导按钮添加到按钮列表管理器中【固定按钮ID】
    /// </summary>
    /// <param name="btnItem"></param>
    public void RegGuideButton(GameObject btnGO,int btnID)
    {
        if (!m_guideBtnParam.ContainsKey(btnID))
        {
            if (btnGO.GetComponent<GuideButtonEvent>() == null)
                btnGO.AddComponent<GuideButtonEvent>();
            m_guideBtnParam.Add(btnID, new GuideBtnParam(btnGO,btnGO.collider.enabled));

            //如果引导尚未结束并当前为强引导，则设置该按钮不可用
            if (!this.IsEndGuide && TaskModel.Instance.TaskGuideType == TaskGuideType.Enforce)//NewbieGuideManager_V2.Instance.IsConstraintGuide)
            {
                //btnGO.SetButtonStatus(false);
            }
        }

    }

    /// <summary>
    /// 设置当前引导按钮
    /// </summary>
    /// <param name="btnID">ButtonID</param>
    public GameObject SetEnforceClickButton(int btnID)
    {
        DisableButtonList();

        if (!m_isResetFlag)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"没有关闭其它按钮，引导失败！");
            return null;
        }
        
       // TraceUtil.Log("@@@@@@@@@@@@@@@@@@@@@SetActiveButton" + btnID);

        if (m_guideBtnParam.ContainsKey(btnID))
        {
            if (!m_guideBtnParam[btnID].GuideBtn.GetComponent<GuideClick>())
            {
                var guideClick = m_guideBtnParam[btnID].GuideBtn.AddComponent<GuideClick>();
                guideClick.BtnId = btnID;
            }
            //m_guideBtnParam[btnID].GuideBtn.SetButtonStatus(true);

            return m_guideBtnParam[btnID].GuideBtn;
        }
        else
            Debug.LogWarning("找不到ID为"+ btnID +"的引导按钮！！");

        return null;
    }

    public GameObject SetWeakClickButton(int btnID)
    {
        if (m_guideBtnParam.ContainsKey(btnID))
        {
            var guideButton = m_guideBtnParam[btnID].GuideBtn;
            if (!guideButton.GetComponent<GuideClick>())
            {
                var guideClick=guideButton.AddComponent<GuideClick>();
                guideClick.BtnId = btnID;
            }
            //guideButton.SetButtonStatus(true);

            return m_guideBtnParam[btnID].GuideBtn;
        }
        else
            Debug.LogWarning("找不到ID为" + btnID + "的引导按钮！！");

        return null;
    }
    
    public bool IsEndGuide { set{m_isEndGuide = value;} get{return m_isEndGuide;} }
    /// <summary>
    /// 关闭指定按钮的引导。在TownGuidUIManager调用StopGuideHandle结束引导时调用
    /// </summary>
    /// <param name="btnID"></param>
    public void CloseGuide(int btnID)
    {
        ResetButtonList();
        if (!m_guideBtnParam.ContainsKey(btnID))
            return;

        var guideGo = m_guideBtnParam[btnID].GuideBtn;
		var guideCom = guideGo.GetComponent<GuideClick>();
		
		if(guideCom != null)
        	guideGo.RemoveComponent<GuideClick>("GuideClick");
        //guideGo.SetButtonStatus(true);

        m_isEndGuide = true;
        
    }

    /// <summary>
    /// 关闭所有Button点击事件
    /// </summary>
    public void DisableButtonList()
    {
        m_guideBtnParam.Values.ApplyAllItem(P =>
            { 
               // P.GuideBtn.SetButtonStatus(false); 

            });
        SetDraggableActive(false);
        m_isResetFlag = true;
    }

    /// <summary>
    /// 重置引导按钮
    /// </summary>
    public void ResetButtonList()
    {
            foreach (KeyValuePair<int, GuideBtnParam> item in m_guideBtnParam)
            {
                //item.Value.GuideBtn.SetButtonStatus(m_guideBtnParam[item.Key].IsCollider);
            }
            SetDraggableActive(true);
            m_isResetFlag = false;
    }

    /// <summary>
    /// 获取Button附加参数列表
    /// </summary>
    public Dictionary<int, GuideBtnParam> GetButtonParamList
    {
        get { return m_guideBtnParam; }
    }

    /// <summary>
    /// 在引导按钮管理列表中删除按钮
    /// </summary>
    /// <param name="btnID">引导按钮ID</param>
    public void DelGuideButton(int btnID)
    {
        if (m_guideBtnParam.ContainsKey(btnID))
        {
			if(m_guideBtnParam[btnID].GuideBtn!=null)
			{
				m_guideBtnParam[btnID].GuideBtn.RemoveComponent<GuideButtonEvent>("GuideButtonEvent");
			}            
            m_guideBtnParam.Remove(btnID);
        }
    }

    public void DelDraggable(GameObject draggable)
    {
        if(m_draggableList.Exists(P => P == draggable))
        {
            m_draggableList.Remove(draggable);
        }
    }

    public Dictionary<int, GuideBtnParam> GetButtonList
    {
        get { return m_guideBtnParam; }
    }

    private void SetDraggableActive(bool enable)
    {
        for (int i = 0; i < m_draggableList.Count; i++)
        {
            m_draggableList[i].GetComponent<UIDraggablePanel>().enabled = enable;
            if (m_draggableList[i].GetComponent<UIDraggablePanel>().verticalScrollBar != null)
                m_draggableList[i].GetComponent<UIDraggablePanel>().verticalScrollBar.enabled = enable;
        }
    }
    /// <summary>
    /// 计算ButtonID
    /// </summary>
    /// <param name="btnItem"></param>
    /// <returns></returns>
    public int CalculateBtnID(UIType uiType, SubType subUIType)
    {
        int btnID = CalcuteBtn(uiType, subUIType, true);
        //m_btnIDList.Add(btnID);

        return btnID;
    }
    public int CalcuteBtn(UIType uiType, SubType subUIType,bool autoSequence)
    {
        int mainID = GetHeadID(uiType);
        int subID = GetSubID(subUIType);
        int btnID = mainID + subID;

        if (autoSequence)
        {
            while (m_guideBtnParam.ContainsKey(btnID))
            {
                btnID++;
            }
            //while (m_btnIDList.Exists(P => P == btnID))
            //{
            //    btnID++;
            //}
        }
        return btnID;
    }
    /// <summary>
    /// »ñÈ¡Ö÷ID
    /// </summary>
    /// <param name="uiType"></param>
    /// <returns></returns>
    private int GetHeadID(UIType uiType)
    {
        switch (uiType)
        {
            case UIType.Empty:
                return 100000;
            case UIType.Battle:   //副本界面
                return 110000;
            //case UIType.SocialInfo:   //好友界面
            //    return 120000;
            //case UIType.SystemSetting: //设置界面
            //    return 130000;
            //case UIType.SkillMain:  //秘籍技能界面
            //    return 140000;
            //case UIType.PVPBattle:  //PVP战斗界面
            //    return 150000;
            //case UIType.Fashion:    //时装界面
            //    return 160000;
            //case UIType.Meridians:  //经脉界面
            //    return 170000;
            //case UIType.Package:   //背包行囊界面
            //    return 180000;
            //case UIType.EquipStrengthen:  //强化锻造界面
            //    return 190000;            
            //case UIType.Siren:      //练妖界面
            //    return 210000;
            //case UIType.TeamInfo:   //组队界面
            //    return 220000;
            //case UIType.Treasure:  //宝树界面
            //    return 230000;
            //case UIType.MartialArtsRoom:  //练功房界面
            //    return 240000;
            //case UIType.TrialsEctypePanel:  //试炼副本界面
            //    return 250000;
            //case UIType.TreasureChests: //宝箱
            //    return 260000;
            //case UIType.Shop:   //商店界面
            //    return 270000;
            //case UIType.Chat:
            //    return 280000;      //Chat
            //case UIType.DailyTaskPanel:
            //    return 290000;      //女官
            //case UIType.TopUp:
            //    return 390000;      //充值
            default:
                return 1;
        }
    }

    private int GetSubID(SubType subUIType)
    {        
        switch ((int)subUIType)
        {
            case 0:
                return 1100;
            case 1:
                return 1200;
            case 2:
                return 1300;
            case 3:
                return 1400;
            case 4:
                return 1500;
            case 5:
                return 1600;
            case 6:
                return 1700;
            case 7:
                return 1800;
            case 8:
                return 1900;
            case 9:
                return 2000;

            default:
                throw new Exception("获取子ID" + subUIType);
        }
    }

    public void Instantiate()
    {
        
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}






public enum SubType
{
    #region 公共界面  UIType.Empty
    MainButton = 0,     //主界面主按钮
    ButtomCommon = 1,   //底部公共按钮
    TopCommon = 2,      //顶部公共按钮
    MsgBox = 3,     //弹出框
    #endregion
    #region Npc对话界面  UIType.Empty
    NpcButton = 3,  //Npc主按钮
    NpcReturn = 4, //NPC返回按钮
    #endregion
    #region 副本战斗界面  UIType.Empty
    EctypeSysSetting = 5, //系统设置
    EctypeHealth = 6, //药水槽
    EctypeSkill = 7,  //普通技能
    EctypeSpecialSkill = 8, //特殊技能
    #endregion
    #region 摇杆控制
    JoySticker = 9,    //摇杆控制
	#endregion

    #region 副本界面
    EctypeBattleGo = 2,
    ShowYaoqiExp = 3,
    AddYaoqiValue = 4,
    CloseYaoqiExp = 5,
    EctypeCard = 6,
    #endregion
    #region 好友界面
    FriendPage = 2,  //好友翻页
    #endregion

    #region 强化锻造界面
    EquipStrenButton = 2, //强化按钮
    EquipStrenItemList = 3, //强化列表
	EquipStrenOperate = 4, //强化操作
	EquipStrenSelectArtifice = 5, //选择炼化界面
    EquipStrenOperateItem = 6, //炼化列表项
    EquipStrenMsg = 7, //装备消息
    #endregion
    #region 时装界面
    FashionPage = 3, //时装翻页
    FashionEquipBtn = 4, //试空按钮
    FashionItem = 5,   //时装列表项
    #endregion
    #region 技能秘籍界面
    SkillEmpty = 0,  //无用
    SkillMainAssembly = 2, //技能装配列表
    SkillMainItem = 3,  //技能列表
    SkillMainViewSkill = 4,   //技能查看按钮
    SkillMainUpgrade = 5, //技能升级按钮
    SkillMainItemed = 6, //技能已装配列表
    SkillMainTips = 7,  //技能项目的Tips
    #endregion
    #region 背包行囊界面
    PackageContainerBoxSlot = 2, //未装备背包格
    PackageHeroBoxSlot = 3, //英雄已装备背包格
    PackagePage = 4, //背包上下页
    packageReset = 5, //背包整理
    PackageViewSkill = 6, //背包列表查看
    PackageEquipItem = 7, //背包物品项
    PackageEquipDescPanel=8, //背包界面道具描述面板
    #endregion
    #region PVP界面
    PVPCategoryRank = 3, //PVP分类排行
    #endregion
    #region 经脉界面
    MeridiansPoint = 3, //经脉列表点
    #endregion
    #region 组队界面
    TeamRoom = 3,  //组队房间
    #endregion
    #region 试炼副本界面
    ShowAtrribute = 3,  //显示试炼属性，搜索按钮
    GoButton = 4,   // 开始试炼按钮
    #endregion
    #region 炼妖界面
    SirenMainButton = 3,  //炼妖主按钮
    SirenItemList = 4,  //妖女列表
    #endregion
    #region 宝树界面
    TreasureTreesMainButton = 3,  //宝树主按钮
    TreasureTreesItem = 4,  //宝树列表
    #endregion
    #region 炼功房界面
    MartialMainButton = 3,  //快速加入主按钮
    MartialResetRoom = 4, //次界面换一批按钮
    MartialSearchRoom = 5,  //搜索房间界面
    MartialPlayerRoom = 6,  //玩家房间
    PlayerRoomBreakPanel = 7,  //
    PlayerRoomBoxControl = 8, //
    #endregion
   
    #region 宝箱界面
    TreasureChestsHelp = 3,  //宝箱帮助
    TreasureChestsItem = 4, //宝箱列表
    #endregion
   
    #region 商店界面
    ShopInfoGoodItem = 3,  //商店列表项
    ShopInfoGoodInfo = 4, //商店项Detail面板
    ShopInfoNextPage = 5,   //下一页
    ShopInfoLastPage = 6,   //上一页
    ShopInfoBuyTips = 7,  //购买Tips
    #endregion
}

//public enum SubUIType
//{
//    NoSubType = 0, //¹«¹²°ŽÅ¥£¬·Ç×ÓÀà
//    /// ºÃÓÑœçÃæStart
//    AgreeAddFriend = 1,     //ÔöŒÓºÃÓÑÁÐ±í--Í¬Òâ°ŽÅ¥
//    RefuseAddFriend = 2,    //ÔöŒÓºÃÓÑÁÐ±í--ŸÜŸø°ŽÅ¥
//    DelFriend = 3,          //ºÃÓÑÁÐ±í--ÉŸ³ýºÃÓÑ
//    JoinFriendGame = 4,     //ºÃÓÑÁÐ±í--ŒÓÈë¶ÓÎé
//    IncreaseFriend = 5,     //žœœüÍæŒÒ--ŒÓÎªºÃÓÑ
//    ///ºÃÓÑœçÃæEnd
//    ///ŒŒÄÜœçÃæStart
//    SkillItem = 1,      //SkillMain ŒŒÄÜÁÐ±íÔªËØ
//    ///ŒŒÄÜœçÃæEnd
//    ///ž±±ŸÑ¡ÔñÒ³ÃæStart
//    EctypeCard = 1,   //¹Ø¿šÑ¡Ôñ¿šÆ¬
//    ///ž±±ŸÑ¡ÔñÒ³ÃæEnd
//    ///MessageBox--Start
//    SingleMsgBox = 1,   //µ¥°ŽÅ¥ÏûÏ¢µ¯³ö¿ò
//    DoubleMsgBox = 2,     //Ë«°ŽÅ¥ÏûÏ¢µ¯³ö¿ò
//    ///MessageBox--End
//    ///±³°üÒ³ÃæStart
//    CharacterContainer = 1,  //±³°üÒ³Ãæ--œÇÉ«×°Åä¿ò
//    PackBoxList = 2,        //±³°üÒ³Ãæ--±³°üÎïÆ·ÁÐ±í
//    ///±³°üÒ³ÃæEnd
//    ///TipsœçÃæStart
//    TipsStyle0 = 1,    //Tips·çžñ1----¶ÔÓŠÀàLocalContainerTips0
//    TipsStyle1 = 2,
//    TipsStyle2 = 3,
//    TipsStyle3 = 4,
//    TipsStyle4 = 5,
//    TipsStyle5 = 6,
//    ///TipsœçÃæEnd
//    ///×é¶ÓœçÃæStart
//    JoinTeam = 1,
//    ///×é¶ÓœçÃæEnd
//    ///MainBtnStart
//    MainBtn = 3,
//    ///MainBtnEnd
//    ///×°±žÇ¿»¯¿ªÊŒ
//    EquipList = 1,
//    EquipItemBtn = 2,
//    ///×°±žÇ¿»¯œáÊø
//}
