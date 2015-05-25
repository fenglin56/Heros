using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.MainUI;
using UnityEngine;
using System.Text;

/// <summary>
/// 新的任务引导按钮管理器
/// 按钮的注册。为按钮添加控制脚本。
/// 按钮的引导（设置引导及特效箭头）
/// 
/// </summary>
public class TaskBtnManager :Controller, ISingletonLifeCycle
{
    public Transform UIRoot;
    private List<int> MappingIds = new List<int>();
    /// <summary>
    /// 按钮ID与按钮及参数的Mapping，注册按钮的时候加入
    /// </summary>
    private Dictionary<int, GuideBtnParam> m_guideBtnParam = new Dictionary<int, GuideBtnParam>();
    private bool m_isEndGuide = true;
    private static TaskBtnManager m_instance;
    public static TaskBtnManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new TaskBtnManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
    /// <summary>
    /// 按钮注册,为按钮添加GuideButtonEvent组件
    /// </summary>
    /// <param name="btnGO"></param>
    /// <param name="uiType"></param>
    /// <param name="subUIType"></param>
    /// <returns></returns>
    public int RegGuideButton(GameObject btnGO, UIType uiType, BtnMapId_Sub subUIType)
    {
        return RegGuideButton(btnGO, uiType, subUIType, null);
    }
    public bool SavingBtnState { get; private set; }
    public int RegGuideButton(GameObject btnGO, UIType uiType, BtnMapId_Sub subUIType, Action<bool> customerActHandler)
    {
        int btnID = CalcBtnId(uiType, subUIType);
        if (btnID != 0)
        {
            if (!m_guideBtnParam.ContainsKey(btnID))
            {
                m_guideBtnParam.Add(btnID, new GuideBtnParam(btnID, btnGO, btnGO.collider.enabled, uiType, subUIType));

                if (!this.m_isEndGuide)
                {
                    SetGuideBtnStatus(m_guideBtnParam[btnID], false, false);
                }
            }
        }

        return btnID;
    }

    ///// <summary>
    ///// 在子界面打开的时候调用，用于控制强引导时其他控制不可用
    ///// </summary>
    ///// <param name="btnGO"></param>
    //public void RegisterDisableBtnId(GameObject btnGO)
    //{
    //    if (TaskModel.Instance.TaskGuideType == TaskGuideType.Enforce)
    //    {
    //        List<PlayDataStruct<BoxCollider>> playDataStructs;
    //        btnGO.transform.RecursiveGetComponent("BoxCollider", out  playDataStructs);
    //        foreach (var item in playDataStructs)
    //        {
    //            var guideBtnBehaviour=item.AnimComponent.GetComponent<GuideBtnBehaviour>();
    //            if (guideBtnBehaviour == null)
    //            {
    //                guideBtnBehaviour = item.AnimComponent.gameObject.AddComponent<GuideBtnBehaviour>();
    //                guideBtnBehaviour.RegisterBtnMappingId(UIType.CommonButton, BtnMapId_Sub.CommonButton_DiableButton);
    //            }
    //        }
    //    }
    //}
    public GuideBtnParam FindGuideBtnParamViaMappingId(UIType mappingCategory,int mappintId)
    {
        var targetBtnObject = m_guideBtnParam.Values.LastOrDefault(P => P.BtnBehaviour != null 
            && P.BtnBehaviour.MappingId == mappintId 
            && P.BtnBehaviour.MainUiType == mappingCategory );//&& !P.InVoid);
        return targetBtnObject;
    }
    public GuideBtnParam FindGuideBtnParamViaMappingId(int mappintId)
    {
        var targetBtnObject = m_guideBtnParam.Values.LastOrDefault(P => P.BtnBehaviour != null
            && P.BtnBehaviour.MappingId == mappintId);//&& !P.InVoid);
        return targetBtnObject;
    }
    public IEnumerable<GuideBtnParam> FindGuideBtnParam(int mappintId)
    {
        var targetBtnObject = m_guideBtnParam.Values.FirstOrDefault(P => P.BtnBehaviour != null && P.BtnBehaviour.MappingId == mappintId);// && !P.InVoid);
        if (targetBtnObject != null)
        {
            var targetBtnObjects = m_guideBtnParam.Values.Where(P => P.BtnBehaviour != null
                && P.BtnBehaviour.MainUiType == targetBtnObject.BtnBehaviour.MainUiType
                && P.BtnBehaviour.SubBtnIdType == targetBtnObject.BtnBehaviour.SubBtnIdType);
            Debug.Log(targetBtnObject.BtnBehaviour.MainUiType + "  " + targetBtnObject.BtnBehaviour.SubBtnIdType + "  " + targetBtnObjects.Count());           
            return targetBtnObjects;
        }
        return null;
    }
    
    ///<summary>
    /// 将引导按钮添加到按钮列表管理器中【固定按钮ID】
    /// </summary>
    /// <param name="btnItem"></param>
    public void RegGuideButton(GameObject btnGO, int btnID)
    {
        if (!m_guideBtnParam.ContainsKey(btnID))
        {
            m_guideBtnParam.Add(btnID, new GuideBtnParam(btnID, btnGO, btnGO.collider.enabled));

            //如果引导尚未结束并当前为强引导，则设置该按钮不可用
            if (!this.m_isEndGuide)//&& NewbieGuideManager_V2.Instance.IsConstraintGuide)
            {
                SetGuideBtnStatus(m_guideBtnParam[btnID], false, false);
            }
        }

    }
    /// <summary>
    /// 在引导按钮管理列表中删除按钮
    /// </summary>
    /// <param name="btnID">引导按钮ID</param>
    public void DelGuideButton(int btnID)
    {
        if (m_guideBtnParam.ContainsKey(btnID))
        {
            //TraceUtil.Log(SystemModel.Rocky, "成功从字典中移除:" + btnID);
            m_guideBtnParam.Remove(btnID);
        }
    }
    
    /// <summary>
    /// 当前引导按钮Id
    /// </summary>
    public int GuidingBtnId { get; private set; }
    /// <summary>
    /// 设置当前引导按钮(强引导)
    /// </summary>
    /// <param name="btnID">ButtonID</param>
    public GameObject SetGuideButton(int btnID, TaskGuideExtendData newGuideConfigData)
    {         
        if (m_guideBtnParam.ContainsKey(btnID))
        {            
            var btnPram = m_guideBtnParam[btnID];
            SetGuideBtnStatus(btnPram, true,true);           
            
            //Destroy all frame and focus effect
            m_guideBtnParam.Values.ApplyAllItem(P =>
            {
                if (P.BtnFrame != null)
                {
                    GameObject.Destroy(P.BtnFrame);
                }
                if (P.BtnArrow != null)
                {
                    GameObject.Destroy(P.BtnArrow);
                }
            });
            var parScale = btnPram.GuideBtn.transform.localScale;
            GameObject arrowFrame = null, hightLightFrame = null;
            Vector3 frameOffset = Vector3.zero, arrowOffset = Vector3.zero;
            if (newGuideConfigData.NewGuideConfigDatas.BtnSignPrefab != null)
            {
                var offsetPosVal1 = newGuideConfigData.NewGuideConfigDatas.BtnSignOffset.Split('+');
				arrowOffset = new Vector3(float.Parse(offsetPosVal1[1]), float.Parse(offsetPosVal1[2]), float.Parse(offsetPosVal1[3]));
                arrowFrame = (GameObject)GameObject.Instantiate(newGuideConfigData.NewGuideConfigDatas.BtnSignPrefab, Vector3.zero, Quaternion.identity);
                arrowFrame.transform.parent = UIRoot;
                arrowFrame.transform.position = btnPram.GuideBtn.transform.position;
                arrowFrame.transform.localScale = Vector3.one;
				arrowFrame.transform.localPosition = arrowFrame.transform.localPosition + arrowOffset;

                arrowFrame.GetComponent<BtnSignPanel>().InitSignPanel(newGuideConfigData.NewGuideConfigDatas.BtnSignText);
               
            }
            if (newGuideConfigData.NewGuideConfigDatas.HighlightRes != null)
            {
                var offsetPosVal2 = newGuideConfigData.NewGuideConfigDatas.BtnPositionOffset.Split('+');
				frameOffset = new Vector3(float.Parse(offsetPosVal2[1]), float.Parse(offsetPosVal2[2]), float.Parse(offsetPosVal2[3]));
                hightLightFrame = (GameObject)GameObject.Instantiate(newGuideConfigData.NewGuideConfigDatas.HighlightRes, Vector3.zero, Quaternion.identity);
                hightLightFrame.transform.parent = UIRoot;
                hightLightFrame.transform.position = btnPram.GuideBtn.transform.position;
                hightLightFrame.transform.localScale = new Vector3(newGuideConfigData.NewGuideConfigDatas.HighlightScale, newGuideConfigData.NewGuideConfigDatas.HighlightScale, 1);
                //偏移光圈和BoxCollider      
				hightLightFrame.transform.localPosition = hightLightFrame.transform.localPosition + frameOffset;
				//jamfing 20141204//
				/*var collider = btnPram.BtnCollider;
				collider.center += frameOffset;*/
            }

            btnPram.BtnBehaviour.SetBtnGuideFrame(hightLightFrame, frameOffset, arrowFrame, arrowOffset);
            GuidingBtnId = btnID;
            return m_guideBtnParam[btnID].GuideBtn;
        }
        else
            Debug.LogWarning("找不到ID为" + btnID + "的引导按钮！！");
        return null;
    }
    public GameObject SetGuideButton(TaskGuideExtendData newGuideConfigData)
    {
        return SetGuideButton(newGuideConfigData.MappingId, newGuideConfigData);
    }
    /// <summary>
    /// 引导中断（弱引导点击其他按钮或引导完成）
    /// </summary>
    public void BreakGuide()
    {
		Debug.Log ("BreakGuide====");
        RecoverAllButtonStatus();
        GuideFinish();
        ResetAllButtonStatus(true);
    }
    public void GuideFinish()
    {
        GuidingBtnId = 0;       
        RemoveGuideFrame();
    }
    public void RemoveGuideFrame()
    {
        m_guideBtnParam.Values.ApplyAllItem(P =>
        {
            if (P.BtnFrame != null)
            {
                GameObject.Destroy(P.BtnFrame);
            }
            if (P.BtnArrow != null)
            {
                GameObject.Destroy(P.BtnArrow);
            }
        });
    }
    /// <summary>
    /// 获得所有BtnId的对应，用于给策划填表
    /// </summary>
    /// <returns></returns>
    public string GetBtnId()
    {
        StringBuilder btnIdStringBuilder = new StringBuilder();
        var uiTypeNames = Enum.GetNames(typeof(UIType));
        byte[] uiTypeValues = (byte[])Enum.GetValues(typeof(UIType));
        var btnMapId_SubNames = Enum.GetNames(typeof(BtnMapId_Sub));
        int[] btnMapId_SubValues = (int[])Enum.GetValues(typeof(BtnMapId_Sub));
        int btnMapIdLength = btnMapId_SubNames.Length;
        int uiTypeLength = uiTypeNames.Length;

        string uiTypeName = string.Empty, subTypeName = string.Empty, uiTypeDesc = string.Empty, subTypeDesc = string.Empty;
        int btnId = 0, subTypeEnumValue = 0;
        for (int j = 0; j < uiTypeLength; j++)
        {
            uiTypeName = uiTypeNames[j];
            UIType uiTypeVal = (UIType)uiTypeValues[j];
            uiTypeDesc = uiTypeVal.ToDescription<UIType>();
            btnId = CalcBtnId((UIType)uiTypeValues[j], BtnMapId_Sub.Empty, true);
            subTypeDesc = BtnMapId_Sub.Empty.ToDescription<BtnMapId_Sub>();

            btnIdStringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5} \n", uiTypeDesc, uiTypeName, subTypeDesc, "", btnId, subTypeEnumValue);            
        }

        for (int i = 0; i < btnMapIdLength; i++)
        {
            subTypeName = btnMapId_SubNames[i];
            string[] nameArray = subTypeName.Split('_');
            for (int j = 0; j < uiTypeLength; j++)
            {
                if (uiTypeNames[j] == nameArray[0])
                {
                    uiTypeName = uiTypeNames[j];
                    UIType uiTypeVal = (UIType)uiTypeValues[j];
                    uiTypeDesc = uiTypeVal.ToDescription<UIType>();
                    BtnMapId_Sub btnMapId_Sub = (BtnMapId_Sub)btnMapId_SubValues[i];
                    subTypeEnumValue = btnMapId_SubValues[i];
                    subTypeDesc = btnMapId_Sub.ToDescription<BtnMapId_Sub>();
                    btnId = CalcBtnId(uiTypeVal, btnMapId_Sub, true);
                    break;
                }
            }
            btnIdStringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5} \n", uiTypeDesc, uiTypeName, subTypeDesc, subTypeName, btnId, subTypeEnumValue);
            /*if (subTypeEnumValue == 904)  //  技能装配项 另外三个位置
            {
                btnIdStringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5} \n", uiTypeDesc, uiTypeName, subTypeDesc, subTypeName, btnId + 1, subTypeEnumValue);
                btnIdStringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5} \n", uiTypeDesc, uiTypeName, subTypeDesc, subTypeName, btnId + 2, subTypeEnumValue);
                btnIdStringBuilder.AppendFormat("{0},{1},{2},{3},{4},{5} \n", uiTypeDesc, uiTypeName, subTypeDesc, subTypeName, btnId + 3, subTypeEnumValue);
            }*/
        }
        return btnIdStringBuilder.ToString();
    }    
    /// <summary>
    /// 获得BtnId对应的UIType，用于打开指定UI
    /// </summary>
    /// <returns></returns>
    public UIType GetUITypeByBtnId(TaskGuideExtendData newGuideConfigData)
    {
        int btnID = int.Parse(newGuideConfigData.NewGuideConfigDatas.GuideBtnID);

        return (UIType)(btnID/1000000);
    }
    public UIType GetUITypeByBtnId(string id)
    {
        int btnID = int.Parse(id);

        return GetUITypeByBtnId(btnID);
    }
    public UIType GetUITypeByBtnId(int btnID)
    {
        int enumVal = (btnID / 1000000);
        return (UIType)enumVal;
    }
    /// <summary>
    /// 根据flag重置所有Button点击事件,不触发按钮状态变化事件
    /// </summary>
    public void ResetAllButtonStatus(bool flag)
    {
        List<int> invoidBtnIds = new List<int>();
        foreach (var key in m_guideBtnParam.Keys)
        {
            var P = m_guideBtnParam[key];
            if (P.GuideBtn != null)
            {
                SetGuideBtnStatus(P, flag, false);
            }
            else
            {
                invoidBtnIds.Add(key);
            }
        }
        invoidBtnIds.ApplyAllItem(P => m_guideBtnParam.Remove(P));
    }
    /// <summary>
    /// 保存按钮上一次状态，然后重置
    /// </summary>
    /// <param name="flag"></param>
    public void SaveAndResetAllButtonStatus(bool flag)
    {
        m_guideBtnParam.Values.ApplyAllItem(P =>
        {
            if (P.UIType == UIType.Empty || P.SubUIType==BtnMapId_Sub.Empty)
            {
                P.SaveStatus();
                P.GuideBtn.SetActive(flag);
            }
        });
        SavingBtnState = true;
    }
    /// <summary>
    /// 恢复按钮状态
    /// </summary>
    public void RecoverAllButtonStatus()
    {
        if (SavingBtnState)
        {
            m_guideBtnParam.Values.ApplyAllItem(P =>
            {
                if (P.UIType == UIType.Empty || P.SubUIType == BtnMapId_Sub.Empty)
                {
                    P.RecoverStatus();
                }
            });
            SavingBtnState = false;
        }
    }
    /// <summary>
    /// 设置引导按钮状态
    /// </summary>
    /// <param name="btnParam"></param>
    /// <param name="flag">按钮可用</param>
    /// <param name="triggerEvent"></param>
    private void SetGuideBtnStatus(GuideBtnParam btnParam, bool flag,bool triggerEvent)
    {
        btnParam.BtnCollider.enabled = flag;
    }
    private int CalcBtnId(UIType btnMapId_Main, BtnMapId_Sub btnMapId_Sub)
    {
        int btnID = CalcBtnId(btnMapId_Main, btnMapId_Sub, true);

        return btnID;
    }
    private int CalcBtnId(UIType btnMapId_Main, BtnMapId_Sub btnMapId_Sub, bool autoSequence)
    {
        int btnID = (byte)btnMapId_Main*1000000+(int)btnMapId_Sub *100;
        if (autoSequence)
        {
            while (m_guideBtnParam.ContainsKey(btnID))
            {
                btnID++;
            }
        }
        return btnID;
    }

    protected override void RegisterEventHandler()
    {
        //UIEventManager.Instance.RegisterUIEvent(UIEventType.ClickTheGuideBtn, GuideFinishHandle);
    }

    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        //UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ClickTheGuideBtn, GuideFinishHandle);
        m_instance = null;
    }
}
public class GuideBtnParam
{
    public bool IsCollider;
    public UIType UIType;
    public BtnMapId_Sub SubUIType;
    /// <summary>
    /// 引导按钮游戏物体
    /// </summary>
    public GameObject GuideBtn;
    /// <summary>
    /// 引导按钮的引导事件组件
    /// </summary>
    public GuideBtnBehaviour BtnBehaviour;
    /// <summary>
    /// 引导按钮的碰撞组件
    /// </summary>
    public BoxCollider BtnCollider;
    /// <summary>
    /// 引导按钮的偏移光圈
    /// </summary>
    public GameObject BtnFrame { get { if (BtnBehaviour != null)return BtnBehaviour.BtnFrame; else return null; } }
    /// <summary>
    /// 引导按钮的偏移箭头
    /// </summary>
    public GameObject BtnArrow { get { if (BtnBehaviour != null)return BtnBehaviour.BtnArrow; else return null; } }
    /// <summary>
    /// 过期
    /// </summary>
    //public bool InVoid;

    private bool m_cacheColliderEnable;
    private bool m_catchActiveState;
    /// <summary>
    /// Save guide button status
    /// </summary>
    public void SaveStatus()
    {
        if (BtnCollider != null)
        {
            m_cacheColliderEnable = BtnCollider.enabled;
            m_catchActiveState = GuideBtn.activeSelf;
        }
        else
        {
            Debug.Log("GuideBtnParam 没有碰撞盒:" + BtnBehaviour.name);
        }
        
    }
   
    /// <summary>
    /// Recover guide button status
    /// </summary>
    public void RecoverStatus()
    {
        BtnCollider.enabled=m_cacheColliderEnable  ;
        GuideBtn.SetActive(m_catchActiveState);
    }
    public GuideBtnParam(GameObject guideBtn, bool IsCollider)
    {
        this.IsCollider = IsCollider;
        BtnCollider = guideBtn.GetComponent<BoxCollider>();
        this.GuideBtn = guideBtn;
        m_cacheColliderEnable = BtnCollider == null ? false : BtnCollider.enabled;
        //InVoid = false;
    }
    public GuideBtnParam(int btnId,GameObject guideBtn, bool isCollider, UIType uiType, BtnMapId_Sub subUIType)
    {
        this.IsCollider = isCollider;
        BtnCollider = guideBtn.GetComponent<BoxCollider>();
        this.UIType = uiType;
        this.SubUIType = subUIType;
        this.GuideBtn = guideBtn;
        BtnBehaviour = GuideBtn.GetComponent<GuideBtnBehaviour>();
        m_cacheColliderEnable =BtnCollider==null?false: BtnCollider.enabled;
    }
    public GuideBtnParam(int btnId, GameObject guideBtn, bool IsCollider)
        : this(btnId,guideBtn, IsCollider, UIType.Empty, BtnMapId_Sub.Empty)
    {
    }
}
public enum BtnMapId_Sub
{
    Empty=0,
    #region 公共界面  Main   UIType.Empty
    [EnumDesc(Description = "主界面主按钮")]
    Empty_MainButton = 1,     //
    [EnumDesc(Description = "任务引导头像")]
    Empty_TaskGuidePic,   //
    [EnumDesc(Description = "购买元宝")]
    Empty_BuyIngot,       //
    [EnumDesc(Description = "购买活力")]
    Empty_BuyActivity,   //
    [EnumDesc(Description = "购买铜币")]
    Empty_BuyMoney,       //
    #endregion
    #region 副本界面  Battle   UIType.Battle
    [EnumDesc(Description = "副本左边，副本区域列表")]
    Battle_EctypeTab = 101,   //
    [EnumDesc(Description = "副本右边，副本选择")]
    Battle_EctypeChoice,  //
    [EnumDesc(Description = "副本难度1（普通）")]
    Battle_Difficulty01,  //
    [EnumDesc(Description = "副本难度2（宗师）")]
    Battle_Difficulty02,  //
    [EnumDesc(Description = "副本开始挑战")]
    Battle_Start,         //
    [EnumDesc(Description = "副本组队")]
    Battle_Team,          //
    [EnumDesc(Description = "副本掉落")]
    Battle_DropList,      //
    [EnumDesc(Description = "副本Boss描述")]
    Battle_BossDescription,//
    [EnumDesc(Description = "副本返回")]
    Battle_Back,//
    [EnumDesc(Description = "副本加铜币")]
    Battle_AddMoney,//
    [EnumDesc(Description = "副本加元宝")]
    Battle_AddCopper,//
    [EnumDesc(Description = "开启宝箱")]
    Battle_TreaureChestItem,//
	[EnumDesc(Description = "宝箱奖励领取")]
	Battle_TreaureChestItem_Get,//
	[EnumDesc(Description = "宝箱奖励关闭")]
	Battle_TreaureChestItem_Close,//
    #endregion
    #region 器魂吞噬  Gem   UIType.Gem
    [EnumDesc(Description = "主界面 器魂镶嵌Tab")]
    Gem_Inset=201,   //
    [EnumDesc(Description = "主界面 器魂升级Tab")]
    Gem_Upgrade, //   
    [EnumDesc(Description = "主界面 返回")]
    Gem_Back, //
    [EnumDesc(Description = "主界面右边 背包按钮")]
    Gem_PackageBtn,  //
    [EnumDesc(Description = "主界面右边 升级按钮")]
    Gem_UpgradeBtn,  //

    [EnumDesc(Description = "主界面 列表中的项")]
    Gem_JewelContainerListPanel_Item,  //
    [EnumDesc(Description = "主界面 镶嵌器魂列表中的项")]
    Gem_JewelBesetPanel_Item,  //
    [EnumDesc(Description = "主界面 镶嵌器魂列表中的左边镶嵌/摘除按钮")]
    Gem_JewelBesetPanel_JewlBasetHoleLeft,  //
    [EnumDesc(Description = "主界面 镶嵌器魂列表中的右边镶嵌/摘除按钮")]
    Gem_JewelBesetPanel_JewlBasetHoleRight,  //

    [EnumDesc(Description = "升级器魂弹出界面列表中的项")]
    Gem_SwallowContainerListPanel_Item,  //
    [EnumDesc(Description = "升级器魂弹出界面中的返回按钮")]
    Gem_SwallowContainerListPanel_Back,  //
    [EnumDesc(Description = "升级器魂弹出界面中的吞噬按钮")]
    Gem_SwallowContainerListPanel_Swallow,  //
    [EnumDesc(Description = "升级器魂吞噬确认界面的返回按钮")]
    Gem_SwallowConfirmContainerListPanel_Back,  //
    [EnumDesc(Description = "升级器魂吞噬确认界面的吞噬按钮")]
    Gem_SwallowConfirmContainerListPanel_Swallow,  //

    [EnumDesc(Description = "镶嵌器魂弹出界面的列表项")]
    Gem_ChoseJewelContainerListPanel_Item,  //
    [EnumDesc(Description = "镶嵌器魂弹出界面的返回按钮")]
    Gem_ChoseJewelContainerListPanel_Back,  //
    [EnumDesc(Description = "镶嵌器魂弹出界面的镶嵌按钮")]
    Gem_ChoseJewelContainerListPanel_Inset,  //
	
	[EnumDesc(Description = "器魂 装备部位 武器")]
	Gem_JewelBesetPanel_Weapon,
	[EnumDesc(Description = "器魂 装备部位 药水")]
	Gem_JewelBesetPanel_Medicine,
	[EnumDesc(Description = "器魂 装备部位 帽子")]
	Gem_JewelBesetPanel_Heard,
	[EnumDesc(Description = "器魂 装备部位 衣服")]
	Gem_JewelBesetPanel_Body,
	[EnumDesc(Description = "器魂 装备部位 鞋子")]
	Gem_JewelBesetPanel_Shoes,
	[EnumDesc(Description = "器魂 装备部位 戒指")]
	Gem_JewelBesetPanel_Accessories,
    #endregion
    #region 抽奖系统  PlayerLuckDraw   UIType.PlayerLuckDraw
    [EnumDesc(Description = "聚宝十次")]
    PlayerLuckDraw_TenTimes=301, //
    [EnumDesc(Description = "聚宝一次")]
    PlayerLuckDraw_OneTime,  //
    [EnumDesc(Description = "抽奖返回")]
    PlayerLuckDraw_Back,  //
    #endregion   

    #region 任务系统  Task   UIType.Task
    [EnumDesc(Description = "任务项")]
    Task_TaskStateItem=401, //
    [EnumDesc(Description = "任务返回")]
    Task_Back,      //
    [EnumDesc(Description = "任务前往挑战")]
    Task_GotoFight, //
    [EnumDesc(Description = "任务加铜币")]
    Task_AddMoney,  //
    [EnumDesc(Description = "任务加元宝")]
    Task_AddIngot,  //
    [EnumDesc(Description = "任务奖励确定")]
    Task_FinishAwardConfirm,  //
    #endregion

	#region 任务快速引导  Task   UIType.NewbieGuide
	[EnumDesc(Description = "快速引导界面箭头")]
	NewbieGuide_Arrow=451, //
	[EnumDesc(Description = "快速引导界面任务项")]
	NewbieGuide_TaskItem, //
	#endregion

    #region 背包系统  Package   UIType.Package
    [EnumDesc(Description = "主UI购买元宝")]
    Package_BuyIngot=501,
    [EnumDesc(Description = "主UI购买铜币")]
    Package_BuyMoney,
    [EnumDesc(Description = "返回")]
    Package_Back,
    //ContainerItemListPanel 
    [EnumDesc(Description = "主UI全部Tab")]
    Package_TabAll,
    [EnumDesc(Description = "主UI装备Tab")]
    Package_TabEquip,
    [EnumDesc(Description = "主UI道具Tab")]
    Package_TabItem,
    [EnumDesc(Description = "主UI器魂Tab")]
    Package_TabGem,    
    [EnumDesc(Description = "主UI称号")]
    Package_Title,
    [EnumDesc(Description = "主UI时装")]
    Package_Fasion,
    [EnumDesc(Description = "主UI角色属性")]
    Package_PlayerProperty,
    [EnumDesc(Description = "整理背包")]
    Package_Tidy,           //
    [EnumDesc(Description = "批量出售")]
    Package_BatchingSell,   //
    //ContainerItemListPanel-SingleItemLineArea-SinglePackItemSlot
    [EnumDesc(Description = "主UI背包列表子项 SinglePackItemSlot")]
    Package_Cell,
    //ContainerItemListPanel-SingleItemLineArea
    [EnumDesc(Description = "背包 解锁背包栏")]
    Package_PackUnLock,
    //ContainerItemListPanel-UnlockContainerBoxTips 解锁确认两个按钮
    [EnumDesc(Description = "背包 解锁确认")]
    Package_PackUnLock_Sure,
    [EnumDesc(Description = "背包 解锁取消")]
    Package_PackUnLock_Cancel,
    //SingleEquiptSlot根据EquiptSlotType添加引导
	[EnumDesc(Description = "背包装备部位 武器")]
	Package_HeroEquiptPanel_Weapon,
	[EnumDesc(Description = "背包装备部位 药水")]
	Package_HeroEquiptPanel_Medicine,
	[EnumDesc(Description = "背包装备部位 帽子")]
    Package_HeroEquiptPanel_Heard,
	[EnumDesc(Description = "背包装备部位 衣服")]
	Package_HeroEquiptPanel_Body,
	[EnumDesc(Description = "背包装备部位 鞋子")]
    Package_HeroEquiptPanel_Shoes,
	[EnumDesc(Description = "背包装备部位 戒指")]
    Package_HeroEquiptPanel_Accessories,

    [EnumDesc(Description = "主UI角色转动")]
    Package_HeroEquiptPanel_DragRoleModel,
    //SellItemsPanel 批量出售按钮
    [EnumDesc(Description = "批量出售UI 取消出售")]
    Package_SellItemsPanel_Cancel, //
    [EnumDesc(Description = "批量出售UI 出售")]
    Package_SellItemsPanel_Sell,
    //SellItemConfirmPanel 出售确认面板  --SelectNumPanel 选择出售数量
    [EnumDesc(Description = "出售确认面板 减少数量")]
    Package_SellItemConfirmPanel_SelectNumPanel_SubtractAmount,//
    [EnumDesc(Description = "出售确认面板 增加数量")]
    Package_SellItemConfirmPanel_SelectNumPanel_AddAmount,//
    [EnumDesc(Description = "出售数量滑动调整")]
    Package_SellItemConfirmPanel_SelectNumPanel_AmountSlider,//
    [EnumDesc(Description = "出售确认面板 取消按钮")]
    Package_SellItemConfirmPanel_SelectNumPanel_AmountCancel,//
    [EnumDesc(Description = "出售确认面板 确认按钮")]
    Package_SellItemConfirmPanel_SelectNumPanel_AmountConfirm,//
    //PlayerTitlePanel  玩家称号面板
    [EnumDesc(Description = "背包 玩家称号面板 返回")]
    Package_PlayerTitlePanel_Return,
    [EnumDesc(Description = "背包 玩家称号面板 使用")]
    Package_PlayerTitlePanel_Use,
    //FashionPanel_V3   时装面板
    [EnumDesc(Description = "时装面板买元宝")]
    Package_FashionPanel_V3_BuyIngot,
    [EnumDesc(Description = "时装面板买铜币")]
    Package_FashionPanel_V3_BuyMoney,
    [EnumDesc(Description = "时装面板拖动按钮")]
    Package_FashionPanel_V3_DragButton,
    [EnumDesc(Description = "时装面板返回按钮")]
    Package_FashionPanel_V3_BackButton,
    [EnumDesc(Description = "时装面板帮助提示")]
    Package_FashionPanel_V3_HelpTips,
    [EnumDesc(Description = "时装面板-信息提示装备按钮")]
    Package_FashionPanel_V3_FashionInfoTips_V3_EquipBtn,
    [EnumDesc(Description = "时装面板-时装列表子项")]
    Package_FashionPanel_V3_SingleFashionBtn,
    //EquipmentUpgradePanel 装备升级面板
    [EnumDesc(Description = "装备项")]
    Package_EquipmentUpgradePanel_UpgradeItem,  //
    [EnumDesc(Description = "装备升级返回按钮")]
    Package_EquipmentUpgradePanel_Back,  //
    [EnumDesc(Description = "装备升级按钮")]
    Package_EquipmentUpgradePanel_Upgrade,   //
    //ItemInfoTipsManager  背包Item提示管理面板
    [EnumDesc(Description = "背包 Item信息提示 返回")]
    Package_ItemInfoTips_Back,
    [EnumDesc(Description = "背包 Item信息提示 吞噬升级")]
    Package_ItemInfoTips_Swallow,
    [EnumDesc(Description = "背包 Item信息提示 穿上")]
    Package_ItemInfoTips_Puton,
    [EnumDesc(Description = "背包 Item信息提示 强化")]
    Package_ItemInfoTips_Strength,
    [EnumDesc(Description = "背包 Item信息提示 器魂")]
    Package_ItemInfoTips_Gem,
    [EnumDesc(Description = "背包 Item信息提示 出售")]
    Package_ItemInfoTips_Sell,
    [EnumDesc(Description = "背包 Item信息提示 升星")]
    Package_ItemInfoTips_StatUp,
    //ItemInfoTipsManager-礼箱
    [EnumDesc(Description = "返回")]
    Package_Gift_Back,   //
    [EnumDesc(Description = "出售")]
    Package_Gift_Sell,  //
    [EnumDesc(Description = "全部使用")]
    Package_Gift_UseAll, //
    [EnumDesc(Description = "使用")]
    Package_Gift_Use,   //

    [EnumDesc(Description = "背包 Item信息提示 升级")]
    Package_ItemInfoTips_Upgrade,
	#endregion

	#region 装备 UIType.EquipmentUpgrade
	[EnumDesc(Description = "装备界面-返回")]
	EquipmentUpgrade_Back = 601,
	[EnumDesc(Description = "装备界面-Tab强化")]
	EquipmentUpgrade_TabStren ,
	[EnumDesc(Description = "装备界面-Tab升星")]
	EquipmentUpgrade_TabStarUpgrade ,
	[EnumDesc(Description = "装备界面-Tab升级")]
	EquipmentUpgrade_TabUpgrade ,

	[EnumDesc(Description = "装备界面-强化列表项[不使用]")]
	EquipmentUpgrade_Stren_Item ,
	[EnumDesc(Description = "装备界面-升星列表项[不使用]")]
	EquipmentUpgrade_Star_Item ,
	[EnumDesc(Description = "装备界面-升级列表项")]
	EquipmentUpgrade_Upgrade_Item ,
	[EnumDesc(Description = "装备界面-强化右边图片按钮")]
	EquipmentUpgrade_Stren_RightIconBtn ,
	[EnumDesc(Description = "装备界面-升星右边图片按钮")]
	EquipmentUpgrade_Star_RightIconBtn ,
	[EnumDesc(Description = "装备界面-升级右边图片按钮")]
	EquipmentUpgrade_Upgrade_RightIconBtn ,
	[EnumDesc(Description = "装备界面-行嚷按钮")]
	EquipmentUpgrade_PackBtn ,
	[EnumDesc(Description = "装备界面-强化按钮")]
	EquipmentUpgrade_StrenBtn ,
	[EnumDesc(Description = "装备界面-升星按钮")]
	EquipmentUpgrade_StarBtn ,
	[EnumDesc(Description = "装备界面-升级按钮")]
	EquipmentUpgrade_UpgradeBtn ,

	[EnumDesc(Description = "装备界面强化部位-武器")]
	EquipmentUpgrade_Stren_Weapon ,
	[EnumDesc(Description = "装备界面强化部位-药水")]
	EquipmentUpgrade_Stren_Medicine ,
	[EnumDesc(Description = "装备界面强化部位-帽子")]
	EquipmentUpgrade_Stren_Heard ,
	[EnumDesc(Description = "装备界面强化部位-衣服")]
	EquipmentUpgrade_Stren_Body ,
	[EnumDesc(Description = "装备界面强化部位-鞋子")]
	EquipmentUpgrade_Stren_Shoes ,
	[EnumDesc(Description = "装备界面强化部位-戒指")]
	EquipmentUpgrade_Stren_Accessories ,

	[EnumDesc(Description = "装备界面升星部位-武器")]
	EquipmentUpgrade_Star_Weapon ,
	[EnumDesc(Description = "装备界面升星部位-药水")]
	EquipmentUpgrade_Star_Medicine ,
	[EnumDesc(Description = "装备界面升星部位-帽子")]
	EquipmentUpgrade_Star_Heard ,
	[EnumDesc(Description = "装备界面升星部位-衣服")]
	EquipmentUpgrade_Star_Body ,
	[EnumDesc(Description = "装备界面升星部位-鞋子")]
	EquipmentUpgrade_Star_Shoes ,
	[EnumDesc(Description = "装备界面升星部位-戒指")]
	EquipmentUpgrade_Star_Accessories ,
	#endregion

	#region 防守副本  Defense   UIType.Defence
    //EquipStrenManager 强化升星
    [EnumDesc(Description = "强化升星Tab强化")]
    EquipStrengthen_TabStren=651,
    [EnumDesc(Description = "强化升星Tab升星")]
    EquipStrengthen_TabStarUpgrade,
    [EnumDesc(Description = "强化升星买元宝")]
    EquipStrengthen_BuyIngot,
    [EnumDesc(Description = "强化升星买铜币")]
    EquipStrengthen_BuyMoney,
    [EnumDesc(Description = "强化升星强化按钮")]
    EquipStrengthen_Strength,
    [EnumDesc(Description = "强化升星返回")]
    EquipStrengthen_Back,
    [EnumDesc(Description = "强化升星背包")]
    EquipStrengthen_Package,
    [EnumDesc(Description = "强化升星装备项")]
    EquipStrengthen_Item,
    [EnumDesc(Description = "强化升星升星按钮")]
    EquipStrengthen_StarUp,
    [EnumDesc(Description = "强化升星列表项")]
    EquipStrengthen_EquipItem,
    [EnumDesc(Description = "强化升星快速升星")]
    EquipStrengthen_QuickUpgradeStar,
    [EnumDesc(Description = "强化升星快速强化")]
    EquipStrengthen_QuickStrengthen,   
    #endregion   
    #region 防守副本  Defense   UIType.Defence
    [EnumDesc(Description = "加元宝")]
    Defence_BuyIngot=701,  //
    [EnumDesc(Description = "加活力")]
    Defence_BuyActivity,    //
    [EnumDesc(Description = "挑战经验关")]
    Defence_Stage1,     //
    [EnumDesc(Description = "挑战铜币关")]
    Defence_Stage2,     //
    [EnumDesc(Description = "挑战元宝关")]
    Defence_Stage3,     //
    [EnumDesc(Description = "返回")]
    Defence_Back,       //
    [EnumDesc(Description = "开始挑战")]
    Defence_GotoFight,  //
    #endregion
    #region 炼妖界面  Siren   UIType.Siren
    [EnumDesc(Description = "触摸妖女点")]
    Siren_TouchPoint = 801,   //
    [EnumDesc(Description = "前一个")]
    Siren_Previour,     //
    [EnumDesc(Description = "后一个")]
    Siren_Next,         //
    [EnumDesc(Description = "返回")]
    Siren_Back,         //
    [EnumDesc(Description = "妖女参战")]
    Siren_Join,         //
    [EnumDesc(Description = "购买元宝")]
	Siren_BuyIngot,     //
    [EnumDesc(Description = "收服按钮")]
    Siren_SubdueSiren,     //   
	[EnumDesc(Description = "妖女突破")]
	Siren_Break,     //
	[EnumDesc(Description = "妖女突破界面-返回")]
	Siren_Break_Back,     //
	[EnumDesc(Description = "妖女突破界面-按钮")]
	Siren_Break_Btn,     //
    #endregion   
	/*#region 技能  Skill   UIType.Skill
	[EnumDesc(Description = "购买元宝")]
	Skill_BuyIngot=901, //
	[EnumDesc(Description = "购买铜币")]
	Skill_BuyMoney, //
	[EnumDesc(Description = "技能列表项")]
	Skill_ListItem, //
	[EnumDesc(Description = "技能装配项")]
	Skill_AssemblyItem,//
	[EnumDesc(Description = "查看打断等级")]
	Skill_ViewBreakLevDesc,//
	[EnumDesc(Description = "技能升级")]
	Skill_Upgrade,  //
	[EnumDesc(Description = "技能界面返回")]
	Skill_Back,  //
	#endregion*/

    #region 技能 jamfing Skill   UIType.Skill
    [EnumDesc(Description = "购买元宝")]
	Skill_UpgrdeBuyIngot=951, //
    [EnumDesc(Description = "购买铜币")]
	Skill_UpgrdeBuyMoney, //
    [EnumDesc(Description = "技能选项")]
	Skill_UpgrdeListItem, //
    [EnumDesc(Description = "技能升级按钮")]
	Skill_UpgrdeUpgrade,//
	[EnumDesc(Description = "技能强化按钮")]
	Skill_UpgrdeStrengthen,//
	[EnumDesc(Description = "技能进阶按钮")]
	Skill_UpgrdeAdvance,  //
	[EnumDesc(Description = "技能进阶物品按钮")]
	Skill_UpgrdeGoodsIcon,  //
    [EnumDesc(Description = "技能界面返回")]
	Skill_UpgrdeBack,  //
    #endregion

    #region 邮件  Mail   UIType.Mail
    [EnumDesc(Description = "收信")]
    Mail_ReceiveMail=1001,  //
    [EnumDesc(Description = "写信")]
    Mail_WriteMail,     //
    [EnumDesc(Description = "公告")]
    Mail_Notice,        //
    [EnumDesc(Description = "返回")]
    Mail_Back,          //
    [EnumDesc(Description = "一键删除")]
    Mail_DelByOneKey,   //
    [EnumDesc(Description = "一键提取")]
    Mail_CatchByOneKey, //
    [EnumDesc(Description = "邮件列表项")]
    Mail_MailItem,      //
    [EnumDesc(Description = "提取邮件")]
    Mail_CatchMail,      //
    [EnumDesc(Description = "删除邮件")]
    Mail_DeleteMail,      //
    [EnumDesc(Description = "发送邮件")]
    Mail_SendMail,      //
    #endregion
    #region 签到  SignIn   UIType.SignIn
    [EnumDesc(Description = "签到关闭")]
    SignIn_CloseIcon=1101,   //
    [EnumDesc(Description = "小箱")]
    SignIn_SBox,        //
    [EnumDesc(Description = "中箱")]
    SignIn_MBox,        //
    [EnumDesc(Description = "大箱")]
    SignIn_XBox,        //
    [EnumDesc(Description = "补浇1")]
    SignIn_ReWater1,    //
    [EnumDesc(Description = "补浇2")]
    SignIn_ReWater2,    //
    [EnumDesc(Description = "补浇3")]
    SignIn_ReWater3,    //
    [EnumDesc(Description = "补浇4")]
    SignIn_ReWater4,    //
    [EnumDesc(Description = "补浇5")]
    SignIn_ReWater5,    //
    [EnumDesc(Description = "补浇6")]
    SignIn_ReWater6,    //
    [EnumDesc(Description = "补浇7")]
    SignIn_ReWater7,    //
    [EnumDesc(Description = "浇水")]
    SignIn_Water,       //
    [EnumDesc(Description = "每日登录奖励确定")]
    SignIn_AwardConfirm,       //
    #endregion
    #region 无尽试炼 Trial UIType.Trial
    [EnumDesc(Description = "返回")]
    Trial_Back = 1201,    //
    [EnumDesc(Description = "购买体力")]
    Trial_BuyActivity ,    //
    [EnumDesc(Description = "购买元宝")]
    Trial_BuyIngot,    //
    [EnumDesc(Description = "开始挑战")]
    Trial_GotoFight,    //
    #endregion
    #region 首领讨伐  Crusade  UIType.Crusade
    [EnumDesc(Description = "返回")]
    Crusade_Back = 1301,  //
    [EnumDesc(Description = "购买体力")]
    Crusade_BuyActivity ,  //
    [EnumDesc(Description = "购买元宝")]
    Crusade_BuyIngot,  //
    [EnumDesc(Description = "首领讨伐弹出界面的列表项")]
    Crusade_EctypeItem,  //
    [EnumDesc(Description = "关卡掉落")]
    Crusade_EctypeDropOut ,  //
	[EnumDesc(Description = "创建队伍")]
    Crusade_GotoCrusade,  //
	[EnumDesc(Description = "队伍列表")]
    Crusade_FollowCrusade ,  //
	[EnumDesc(Description = "随机副本")]
	Crusade_RandomEctype ,  //
	[EnumDesc(Description = "快速加入")]
	Crusade_QuickJoin ,  //

    //[EnumDesc(Description = "发起讨伐-关卡掉落")]
    //Crusade_GotoCrusade_EctypeDropOut,  //
    //[EnumDesc(Description = "发起讨伐-查看副本介绍")]
    //Crusade_GotoCrusade_ViewInstruct,  //
    //[EnumDesc(Description = "发起讨伐-聊天")]
    //Crusade_GotoCrusade_Chat,  //
    //[EnumDesc(Description = "发起讨伐-邀请好友")]
    //Crusade_GotoCrusade_InviteFriend,  //
    //[EnumDesc(Description = "发起讨伐-快速招募")]
    //Crusade_GotoCrusade_QuickCollect,  //
    //[EnumDesc(Description = "发起讨伐-开始挑战")]
    //Crusade_GotoCrusade_Start,  //
    //[EnumDesc(Description = "发起讨伐-返回")]
    //Crusade_GotoCrusade_Back,  //
    //[EnumDesc(Description = "跟随讨伐-创建队伍")]
    //Crusade_FollowCrusade_CreateTeam,  //
    //[EnumDesc(Description = "跟随讨伐-快速加入")]
    //Crusade_FollowCrusade_QuickJoin,  //
    //[EnumDesc(Description = "跟随讨伐-返回")]
    //Crusade_FollowCrusade_Back,  //
    //[EnumDesc(Description = "跟随讨伐-聊天")]
    //Crusade_FollowCrusade_Chat,  //
    #endregion
    #region 通用按钮  CommonButton  UIType.CommonButton
    [EnumDesc(Description = "通用确认按钮")]
    CommonButton_Show_Ensure = 1401,  //
    [EnumDesc(Description = "通用取消按钮")]
    CommonButton_Show_Cancel ,
    [EnumDesc(Description = "副本消息框-Icon按钮")]
    CommonButton_EctyeShow_Icon,   
    #endregion
    #region 铸造界面 Forging 
    [EnumDesc(Description = "铸造界面-返回")]
    Forging_Back=1501,
    [EnumDesc(Description = "铸造界面-铸造")]
    Forging_Forging,
    [EnumDesc(Description = "铸造界面-铸造列表项")]
    Forging_ForgingItem,
	[EnumDesc(Description = "铸造界面-主按钮")]
	Forging_MainBtn,
	[EnumDesc(Description = "铸造界面-装备按钮")]
	Forging_EquipmentBtn,
	[EnumDesc(Description = "铸造界面-宝箱按钮")]
	Forging_GiftBoxBtn,
	[EnumDesc(Description = "铸造界面-材料按钮")]
	Forging_MaterialBtn,
    #endregion
    #region 宝树界面 Treasure
    [EnumDesc(Description = "宝树界面-购买铜币")]
    Treasure_BuyMoney = 1551,
    [EnumDesc(Description = "宝树界面-购买元宝")]
    Treasure_BuyIngot ,
    [EnumDesc(Description = "宝树界面-仙露")]
    Treasure_Amrita,
    [EnumDesc(Description = "宝树界面-摘取")]
    Treasure_Catch,
    [EnumDesc(Description = "宝树界面-消息")]
    Treasure_Message,
    [EnumDesc(Description = "宝树界面-消息关闭")]
    Treasure_MessageClose,
    [EnumDesc(Description = "宝树界面-果实1")]
    Treasure_Fruit1,
    [EnumDesc(Description = "宝树界面-果实2")]
    Treasure_Fruit2,
    [EnumDesc(Description = "宝树界面-果实3")]
    Treasure_Fruit3,
    [EnumDesc(Description = "宝树界面-果实4")]
    Treasure_Fruit4,
    [EnumDesc(Description = "宝树界面-果实5")]
    Treasure_Fruit5,
    [EnumDesc(Description = "宝树界面-果实6")]
    Treasure_Fruit6,
    [EnumDesc(Description = "宝树界面-果实7")]
    Treasure_Fruit7,
    [EnumDesc(Description = "宝树界面-果实8")]
    Treasure_Fruit8,
    [EnumDesc(Description = "宝树界面-返回")]
    Treasure_Back,
    #endregion
    #region 组队界面 Team
   	[EnumDesc(Description = "组队界面-购买元宝")]
    TeamInfo_BuyIngot=1601,
	[EnumDesc(Description = "组队界面-购买活力")]
    TeamInfo_BuyActivity,
	[EnumDesc(Description = "组队界面-聊天")]
    TeamInfo_Chat,
    [EnumDesc(Description = "组队界面-创建队伍")]
    TeamInfo_CreateTeam,
    [EnumDesc(Description = "组队界面-快速加入")]
    TeamInfo_QuickJoin,
	[EnumDesc(Description = "组队界面-更换区域")]
	TeamInfo_ChangeZone,
    [EnumDesc(Description = "组队界面-返回")]
    TeamInfo_Back,
    [EnumDesc(Description = "组队界面-创建队伍-返回")]
    TeamInfo_CreateTeam_Back,
    [EnumDesc(Description = "组队界面-创建队伍-副本列表项")]
    TeamInfo_CreateTeam_EctypeItem,
    [EnumDesc(Description = "组队界面-创建队伍-普通")]
    TeamInfo_CreateTeam_Normal,
    [EnumDesc(Description = "组队界面-创建队伍-宗师")]
    TeamInfo_CreateTeam_Hard,
    [EnumDesc(Description = "组队界面-创建队伍-更换区域")]    
    TeamInfo_CreateTeam_ChangeZone,
    [EnumDesc(Description = "组队界面-创建队伍-创建")]	
    TeamInfo_CreateTeam_Create,
    [EnumDesc(Description = "组队界面-更换区域-区域列表项")]
    TeamInfo_ChangeZone_ZoneItem,
    [EnumDesc(Description = "组队界面-更换区域-确定")]
    TeamInfo_ChangeZone_Confirm,
    [EnumDesc(Description = "组队界面-更换区域-取消")]
    TeamInfo_ChangeZone_Cancel,    
	[EnumDesc(Description = "组队界面-加入队伍-返回")]
    TeamInfo_JoinTeam_Back,
	[EnumDesc(Description = "组队界面-加入队伍-聊天")]
    TeamInfo_JoinTeam_Chat,
	[EnumDesc(Description = "组队界面-加入队伍-邀请好友")]
    TeamInfo_JoinTeam_Invite,
	[EnumDesc(Description = "组队界面-加入队伍-快速招募")]
    TeamInfo_JoinTeam_QuickInvite,
	[EnumDesc(Description = "组队界面-加入队伍-开始挑战")]
    TeamInfo_JoinTeam_GotoFight,
    #endregion

	#region 活动(三财贺喜)界面 Activity
	[EnumDesc(Description = "活动界面-活动选项")]
	Activity_ListItem = 1701,
	[EnumDesc(Description = "活动界面-领取奖励")]
	Activity_GetReward ,
	[EnumDesc(Description = "活动界面-返回按钮")]
	Activity_BtnBack ,
	#endregion

	#region 战斗失败界面 Activity
	[EnumDesc(Description = "战斗失败界面-返回按钮")]
	BattleFail_Close = 1751,
	[EnumDesc(Description = "战斗失败界面-装备")]
	BattleFail_Equip ,
	[EnumDesc(Description = "战斗失败界面-器魂")]
	BattleFail_Gem,
	[EnumDesc(Description = "战斗失败界面-妖姬")]
	BattleFail_Siren ,
	[EnumDesc(Description = "战斗失败界面-武功")]
	BattleFail_Skill ,
	[EnumDesc(Description = "战斗失败界面-商行")]
	BattleFail_Shop ,
	#endregion
	
    //#region Npc对话界面
    //NpcButton ,  //Npc主按钮
    //NpcReturn , //NPC返回按钮
    //#endregion
    //#region 副本战斗界面
    //EctypeSysSetting , //系统设置
    //EctypeHealth , //药水槽
    //EctypeSkill ,  //普通技能
    //EctypeSpecialSkill , //特殊技能
    //EctypeBattleGo,
    //EctypeShowYaoqiExp,
    //EctypeAddYaoqiValue,
    //EctypeCloseYaoqiExp,
    //EctypeCard,
    //#endregion
    //#region 摇杆控制
    //JoySticker ,    //摇杆控制
    //#endregion
    
    //#region 好友界面
    //FriendPage ,  //好友翻页
    //#endregion

    //#region 强化锻造界面
    //EquipStrenButton , //强化按钮
    //EquipStrenItemList , //强化列表
    //EquipStrenOperate , //强化操作
    //EquipStrenSelectArtifice , //选择炼化界面
    //EquipStrenOperateItem , //炼化列表项
    //EquipStrenMsg , //装备消息
    //#endregion
    //#region 时装界面
    //FashionPage , //时装翻页
    //FashionEquipBtn , //试空按钮
    //FashionItem ,   //时装列表项
    //#endregion
    //#region 技能秘籍界面
    //SkillEmpty ,  //无用
    //SkillMainAssembly , //技能装配列表
    //SkillMainItem ,  //技能列表
    //SkillMainViewSkill ,   //技能查看按钮
    //SkillMainUpgrade , //技能升级按钮
    //SkillMainItemed , //技能已装配列表
    //SkillMainTips ,  //技能项目的Tips
    //#endregion
    //#region 背包行囊界面
    //PackageContainerBoxSlot , //未装备背包格
    //PackageHeroBoxSlot , //英雄已装备背包格
    //PackagePage , //背包上下页
    //packageReset , //背包整理
    //PackageViewSkill , //背包列表查看
    //PackageEquipItem , //背包物品项
    //PackageEquipDescPanel , //背包界面道具描述面板
    //#endregion
   
    //#region 组队界面
    //TeamRoom ,  //组队房间
    //#endregion
   
    //#region 炼妖界面
    //SirenMainButton ,  //炼妖主按钮
    //SirenItemList ,  //妖女列表
    //#endregion
    //#region 宝树界面
    //TreasureTreesMainButton ,  //宝树主按钮
    //TreasureTreesItem ,  //宝树列表
    //#endregion

    //#region 宝箱界面
    //TreasureChestsHelp,  //宝箱帮助
    //TreasureChestsItem, //宝箱列表
    //#endregion

    //#region 商店界面
    //ShopInfoGoodItem ,  //商店列表项
    //ShopInfoGoodInfo , //商店项Detail面板
    //ShopInfoNextPage ,   //下一页
    //ShopInfoLastPage ,   //上一页
    //ShopInfoBuyTips ,  //购买Tips
    //#endregion
}
/// <summary>
/// 标记此特征表示在读表过程中忽略
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
public class EnumDescAttribute : Attribute
{
    public string Description;
}