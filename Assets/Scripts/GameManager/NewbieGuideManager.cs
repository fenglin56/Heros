using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewbieGuideManager {

    //private List<GuideConfigData> m_curLevelList = new List<GuideConfigData>();
    //private static int m_curGuideLevel = 0;
    //private static bool m_isComplateEctype = false;
    //private bool m_isGuideState = false;
    //private uint m_passEctypeId = 0;
    //private int m_curGuideLevelCount = 0;
    //private bool m_isEctypeGuide = false;
    //private uint m_curSelectEctypeID;

    //private static NewbieGuideManager m_instance;
    //public static NewbieGuideManager Instance
    //{
    //    get
    //    {
    //        if (m_instance == null)
    //        {
    //            m_instance = new NewbieGuideManager();
    //        }
    //        return m_instance;
    //    }
    //}

    ///// <summary>
    ///// 进否进行副本新手引导
    ///// </summary>
    ///// <param name="onSelectEctypeData">当前所选择的副本</param>
    //public void CurSelectEctype(SMSGEctypeData_SC onSelectEctypeData)
    //{
    //    EctypeSelectConfigData item = EctypeConfigManager.Instance.EctypeSelectConfigList[(int)onSelectEctypeData.dwEctypeID];
    //    uint _ectypeid = (uint)item._vectContainer[(int)onSelectEctypeData.byDiff - 1];
    //    m_curSelectEctypeID = _ectypeid;

    //    if (_ectypeid > m_passEctypeId && GetEctypeGuideData.Count > 0)
    //    {
    //        m_isEctypeGuide = true;  //进行副本新手引导,
    //    }
    //    else
    //    {
    //        m_isEctypeGuide = false;
    //    }

    //}

    //public List<EctypeGuideConfigData> GetEctypeGuideData
    //{
    //    get { return GuideConfigManager.Instance.GetCurEctypeIDDataList(m_curSelectEctypeID); }
    //}


    ///// <summary>
    ///// 是否进行副本新手引导
    ///// </summary>
    //public bool IsEctypeGuide
    //{
    //    get { return m_isEctypeGuide; }
    //}

    ///// <summary>
    ///// 设置引导状态
    ///// </summary>
    //public void SetGuideState(bool start)
    //{
    //    if (start)
    //    {
    //        //NetServiceManager.Instance.EctypeService.SendEctypeClearance();
    //        //GuideBtnManager.Instance.DisableButtonList();
    //        m_isComplateEctype = true;
    //        IsGuideFinish = true;
    //    }

    //    m_isGuideState = start;		
    //}

    ///// <summary>
    ///// 是否引導完成
    ///// </summary>
    //public bool IsGuideFinish{ get; set; }

    ///// <summary>
    ///// 获得引导状态
    ///// </summary>
    //public bool GetGuideState
    //{
    //    get { return m_isGuideState; }
    //}

    ///// <summary>
    ///// 是否屏蔽屏幕点击
    ///// </summary>
    ////public bool IsDisableClick { get; set; }

    ///// <summary>
    ///// 是否完成副本战斗
    ///// </summary>
    //public bool IsCompleteEctype 
    //{ 
    //    set { m_isComplateEctype = value; }
    //    get { return m_isComplateEctype;} 
    //}

    //public List<GuideConfigData> GetCurLevelList
    //{
    //    get { return m_curLevelList = GuideConfigManager.Instance.GetCurLevelDataList(m_curGuideLevel); }
    //}

    ///// <summary>
    ///// 引导等级
    ///// </summary>
    //public int GuideLevel
    //{
    //    set { m_curGuideLevel = value; }
    //    get { return m_curGuideLevel; }
    //}

    ///// <summary>
    ///// 设置当前玩家的最高通关的副本ID
    ///// </summary>
    ///// <param name="ectypeid"></param>
    //public void SetPassEctypeID(uint ectypeid)
    //{
    //    if (ectypeid == 0)
    //        ectypeid = 100;
		
    //    m_passEctypeId = ectypeid;
    //    m_curGuideLevelCount = GuideConfigManager.Instance.GetCurEctypeDataList(m_passEctypeId).Count;            
    //}

    ///// <summary>
    ///// 获取当前引导等级的引导步骤数量
    ///// </summary>
    //public int GetCurLevelCount()
    //{
    //    return m_curGuideLevelCount;                //
    //}

    ///// <summary>
    ///// 递减当前等级引导数量
    ///// </summary>
    //public void DegressionLevelCount()
    //{
    //    m_curGuideLevelCount -= 1;
    //    NextGuideLevel();
    //}

    ///// <summary>
    ///// 转向下一个引导等级，并向服务提交引导等级数据
    ///// </summary>
    //public void NextGuideLevel()
    //{
    //    m_curLevelList.Clear();
    //    m_curGuideLevel += 1;

    //    NetServiceManager.Instance.InteractService.SendNewbieGuide(new SMsgInteract_NewbieGuide_SCS
    //    {
    //        dwActorId = (uint)PlayerManager.Instance.FindHeroDataModel().ActorID,
    //        wGuideIndex = (ushort)NewbieGuideManager.Instance.GuideLevel,
    //    });
    //}

}
