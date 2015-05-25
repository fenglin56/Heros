
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum StroyLineType
{
    None,
    EctypeStart,
    EctypeEnd
};

public class StroyLineDataManager {

    private Dictionary<int, StroyNpcBehaviour> m_NpcGoList = new Dictionary<int, StroyNpcBehaviour>();
    private StroyLineType m_stroyLineType = StroyLineType.None;
    private uint m_passEctypeId = 0;
    private int m_curStroyMapID;
    private string m_curMapBgMusic;
    private List<int> m_curCameraGroupID;
    private int m_curCameraClipID;
    private static SMSGEctypeData_SC m_curSelectEctypeData;
	//当前剧情key值(编辑器和正常游戏共用)
	[HideInInspector]
	public StroyLineKey curStroyLineKey;
    private static StroyLineDataManager m_instance;
    public static StroyLineDataManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new StroyLineDataManager();
            }
            return m_instance;
        }
    }

    /// <summary>
    /// 进否进入剧情[貌似没有使用]
    /// </summary>
    /// <param name="onSelectEctypeData">当前所选择的副本</param>
    public void CurSelectEctype(SMSGEctypeData_SC onSelectEctypeData)
    {
        m_curSelectEctypeData = onSelectEctypeData;

        int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;

        EctypeSelectConfigData item = null;
		//todo：onSelectEctypeData协议已经取消难度dwEctypeID和byDiff难度，如果使用需要从新更改
        //EctypeConfigManager.Instance.EctypeSelectConfigList.TryGetValue((int)onSelectEctypeData.dwEctypeID, out item);
        if (item == null)
            return;

		int _ectypeid = 0;// = item._vectContainer[(int)onSelectEctypeData.byDiff - 1];
		StroyLineKey key = new StroyLineKey{ VocationID = vocation,ConditionID = 0, EctypeID = _ectypeid };


        if (StroyLineConfigManager.Instance.GetStroyLineConfig.ContainsKey(key) && _ectypeid > m_passEctypeId)
        {
            StroyLineConfigData stroyLineConfig = StroyLineConfigManager.Instance.GetStroyLineConfig[key];

            m_curStroyMapID = stroyLineConfig._SceneMapID;
            m_curMapBgMusic = stroyLineConfig._BgMusic;
            m_curCameraGroupID = stroyLineConfig._CameraGroup;

            //1=进入触发；2=完成触发
            if (StroyLineConfigManager.Instance.GetStroyLineConfig[key]._TriggerCondition == 1)
                m_stroyLineType = StroyLineType.EctypeStart;
            else if (StroyLineConfigManager.Instance.GetStroyLineConfig[key]._TriggerCondition == 2)
                m_stroyLineType = StroyLineType.EctypeEnd;
            //else
            //    m_stroyLineType = StroyLineType.None;
        }
        //else
        //{
        //    m_stroyLineType = StroyLineType.None;
        //}
    }
	//创建
    public void TutorialScene()
    {
		m_curStroyMapID = GameManager.Instance.GetStoryConfigData._SceneMapID;
		m_curMapBgMusic = GameManager.Instance.GetStoryConfigData._BgMusic;
		m_curCameraGroupID = GameManager.Instance.GetStoryConfigData._CameraGroup;
		//1=进入触发；2=完成触发
		if (GameManager.Instance.GetStoryConfigData._TriggerCondition == 1)
			m_stroyLineType = StroyLineType.EctypeStart;
		else if (GameManager.Instance.GetStoryConfigData._TriggerCondition == 2)
			m_stroyLineType = StroyLineType.EctypeEnd;
		else
			m_stroyLineType = StroyLineType.None;
		//StroyLineManager.Instance.ResetCameraGroup();
        /*int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
        int ectypeid = CommonDefineManager.Instance.CommonDefine.TUTORIAL_ECTYPE_ID;

        //int _ectypeid = item._vectContainer[(int)onSelectEctypeData.byDiff - 1];
		StroyLineKey key = new StroyLineKey { VocationID = vocation,ConditionID = 1, EctypeID = ectypeid };
        if (StroyLineConfigManager.Instance.GetStroyLineConfig.ContainsKey(key))
        {
            StroyLineConfigData stroyLineConfig = StroyLineConfigManager.Instance.GetStroyLineConfig[key];

            m_curStroyMapID = stroyLineConfig._SceneMapID;
            m_curMapBgMusic = stroyLineConfig._BgMusic;
            m_curCameraGroupID = stroyLineConfig._CameraGroup;
			//1=进入触发；2=完成触发
			if (StroyLineConfigManager.Instance.GetStroyLineConfig[key]._TriggerCondition == 1)
				m_stroyLineType = StroyLineType.EctypeStart;
			else if (StroyLineConfigManager.Instance.GetStroyLineConfig[key]._TriggerCondition == 2)
				m_stroyLineType = StroyLineType.EctypeEnd;
			else
				m_stroyLineType = StroyLineType.None;
        }*/
    }

    /// <summary>
    /// 剧情编辑器用
    /// </summary>
    public void SetCurEctypeID(StroyLineKey key)
    {
		curStroyLineKey = key;
        TraceUtil.Log(key.EctypeID + "   " + key.VocationID);
        m_curStroyMapID = StroyLineConfigManager.Instance.GetStroyLineConfig[key]._SceneMapID;
        m_curMapBgMusic = StroyLineConfigManager.Instance.GetStroyLineConfig[key]._BgMusic;
        m_curCameraGroupID = StroyLineConfigManager.Instance.GetStroyLineConfig[key]._CameraGroup;
    }

    public void GoToBattle(int ectypeID, byte difficulty)
    {
        SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
		{
			//TODO:进入技能协议有更改，去掉副本iD和难度,需要从新修改
//            uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//            dwEctypeId = ectypeID - 1,
//            byDifficulty = difficulty,
        };
        if(EctGuideManager.Instance.IsEctypeGuide)
        //if (NewbieGuideManager_V2.Instance.IsEctypeGuide)
        {
            NetServiceManager.Instance.EctypeService.SendEctypeGuideCreate(sMSGEctypeRequestCreate_CS);
        }
        else
        {
            NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
        }
    }


    public void ChangeScene()
    {
		//jamfing
		GameManager.Instance.DealStroyOverRequestServer();
        /*if (m_stroyLineType == StroyLineType.None)
        {
            int dwEctypeId = CommonDefineManager.Instance.CommonDefine.TUTORIAL_ECTYPE_ID;
            int byDifficulty = EctypeConfigManager.Instance.EctypeContainerConfigList[dwEctypeId].lDifficulty;

			SMSGEctypeRequestCreate_CS sMSGEctypeRequestCreate_CS = new SMSGEctypeRequestCreate_CS()
			{
				//TODO:进入技能协议有更改，去掉副本iD和难度,需要从新修改
//                uidEntity = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity,
//                dwEctypeId = dwEctypeId - 1,
//                byDifficulty = (byte)byDifficulty,
        	};
            NetServiceManager.Instance.EctypeService.SendEctypeRequest(sMSGEctypeRequestCreate_CS);
        }
        else if (m_stroyLineType == StroyLineType.EctypeStart)
		{
			//todo：onSelectEctypeData协议已经取消难度dwEctypeID和byDiff难度，如果使用需要从新更改
            //GoToBattle((int)m_curSelectEctypeData.dwEctypeID, m_curSelectEctypeData.byDiff);
        }
        else
        {
            NetServiceManager.Instance.EctypeService.SendEctypeChallengeComplete_CS();
        }*/
    }

    /// <summary>
    /// 剧情系统类型
    /// </summary>
    public StroyLineType GetStroyType
    {
        get { return m_stroyLineType; }
    }

    /// <summary>
    /// 获取剧情场景名称
    /// </summary>
    public string GetStroySceneName()
    {
        if (EctypeConfigManager.Instance.SceneConfigList.ContainsKey(m_curStroyMapID))
        {
            return EctypeConfigManager.Instance.SceneConfigList[m_curStroyMapID]._szSceneName;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 获取剧情的背景音乐
    /// </summary>
    public string GetMapBgMusic
    {
        get { return m_curMapBgMusic; }
    }
    /// <summary>
    /// 获取镜头组的ID
    /// </summary>
    public List<int> GetCameraGroupID
    {
        get { return m_curCameraGroupID; }
    }

    public bool IsStartCameraMask { get; set; }
    public bool IsEndCameraMask { get; set; }
    /// <summary>
    /// 设置当前镜头片段ID
    /// </summary>
    public int SetCameraClipID
    {
        set { m_curCameraClipID = value; }
    }
    /// <summary>
    /// 获得当前对话ID
    /// </summary>
    public int[] GetDialogGroupID
    {
        get { return StroyLineConfigManager.Instance.GetCameraGroupConfig[m_curCameraClipID]._DialogGroupID; }
    }

    public Dictionary<int, StroyNpcBehaviour> GetNpcList
    {
        get { return m_NpcGoList; }
    }

    public void DelNpcGo(int npcID)
    {
        if (m_NpcGoList.ContainsKey(npcID))
        {
            GameObject.Destroy(m_NpcGoList[npcID].gameObject);
            m_NpcGoList.Remove(npcID);
        }
    }

    /// <summary>
    /// 获取当前镜头组
    /// </summary>
    /// <param name="cameraGroupID">镜头组ID</param>
    /// <returns>镜头组</returns>
    public CameraGroupConfigData GetCurCameraGroup(int cameraGroupID)
    {
        if (StroyLineConfigManager.Instance.GetCameraGroupConfig.ContainsKey(cameraGroupID))
        {
            return StroyLineConfigManager.Instance.GetCameraGroupConfig[cameraGroupID];
        }
        else
        {
            Debug.LogWarning("配置表中不包括ID为" + cameraGroupID + "的镜头组");
            return null;
        }
    }

    /// <summary>
    /// 设置当前玩家的最高通关的副本ID
    /// </summary>
    /// <param name="ectypeid"></param>
    public void SetPassEctypeID(uint ectypeid)
    {
        if (ectypeid == 0)
            m_passEctypeId = 100;

        m_passEctypeId = ectypeid;
    }
}
