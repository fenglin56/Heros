using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct StroyLineKey
{
    public int VocationID;
	public int ConditionID;
    public int EctypeID;
}

public class StroyLineConfigManager : MonoBehaviour {

    public StroyLineDataBase StroyLineConfigFile;
    private Dictionary<StroyLineKey, StroyLineConfigData> m_stroyLineConfigList = new Dictionary<StroyLineKey, StroyLineConfigData>();
    private Dictionary<StroyLineKey, StroyLineConfigData> m_stroyLineConfigListBak = new Dictionary<StroyLineKey, StroyLineConfigData>();

    public CameraGroupDataBase CameraGroupConfigFile;
    private Dictionary<int, CameraGroupConfigData> m_cameraGroupConfigList = new Dictionary<int, CameraGroupConfigData>();
    private Dictionary<int, CameraGroupConfigData> m_cameraGroupConfigListBak = new Dictionary<int, CameraGroupConfigData>();

    public StroyLineDialogDataBase StroyDialogConfigFile;
    private Dictionary<int, StroyDialogConfigData> m_stroyDialogConfigList = new Dictionary<int, StroyDialogConfigData>();

    public StroyActionDataBase StroyActionConfigFile;
    private Dictionary<int, StroyActionConfigData> m_stroyActionConfigList = new Dictionary<int, StroyActionConfigData>();
    private Dictionary<int, StroyActionConfigData> m_stroyActionConfigListBak = new Dictionary<int, StroyActionConfigData>();

    public StroyCameraDataBase StroyCameraConfigFile;
    private Dictionary<int, StroyCameraConfigData> m_stroyCameraConfigList = new Dictionary<int, StroyCameraConfigData>();
    private Dictionary<int, StroyCameraConfigData> m_stroyCameraConfigListBak = new Dictionary<int, StroyCameraConfigData>();

    private static StroyLineConfigManager m_instance;
    public static StroyLineConfigManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    // Use this for initialization
	void Awake () {
        m_instance = this;

        InitStroyLineConfig();
        InitCameraGroupConfig();
        InitDialogConfig();
        InitActionConfig();
        InitCameraConfig();
	}

    void OnDestroy()
    {
        m_instance = null;
    }

    private void InitCameraConfig()
    {
        if (null == StroyCameraConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"StroyLineConfigData没有指定摄像机的配置文件！");
        }
        else
        {
            foreach (StroyCameraConfigData element in StroyCameraConfigFile._dataTable)
            {
                m_stroyCameraConfigList.Add(element._CameraID, element);
                m_stroyCameraConfigListBak.Add(element._CameraID, element.Clone());
            }
        }
    }

    private void InitActionConfig()
    {
        if (null == StroyActionConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"StroyLineConfigData没有指定剧情动作的配置文件！");
        }
        else
        {
            foreach (StroyActionConfigData element in StroyActionConfigFile._dataTable)
            {
                m_stroyActionConfigList.Add(element._ActionID, element);
                m_stroyActionConfigListBak.Add(element._ActionID, element.Clone());
            }
        }
    }

    private void InitDialogConfig()
    {
        if (null == CameraGroupConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"StroyLineConfigData没有指定剧情对话的配置文件！");
        }
        else
        {
            foreach (StroyDialogConfigData element in StroyDialogConfigFile._dataTable)
            {
                m_stroyDialogConfigList.Add(element._DialogID, element);
            }
        }
    }

    private void InitCameraGroupConfig()
    {
        if (null == CameraGroupConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"StroyLineConfigData没有指定镜头组的配置文件！");
        }
        else
        {
            foreach (CameraGroupConfigData element in CameraGroupConfigFile._dataTable)
            {
                m_cameraGroupConfigList.Add(element._CameraGroupID, element);
                m_cameraGroupConfigListBak.Add(element._CameraGroupID, element.Clone());
            }
        }
    }

    private void InitStroyLineConfig()
    {
        if (null == StroyLineConfigFile)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"StroyLineConfigData没有指定剧情的配置文件！");
        }
        else
        {
            foreach (StroyLineConfigData element in StroyLineConfigFile._dataTable)
            {
				m_stroyLineConfigList.Add(new StroyLineKey { VocationID = element._TriggerVocation, ConditionID = element._TriggerCondition, EctypeID = element._EctypeID }, element);
				m_stroyLineConfigListBak.Add(new StroyLineKey { VocationID = element._TriggerVocation, ConditionID = element._TriggerCondition,EctypeID = element._EctypeID }, element.Clone());
            }
        }
    }

    public Dictionary<StroyLineKey, StroyLineConfigData> GetStroyLineConfig
    {
        get { return m_stroyLineConfigList; }
    }

    public Dictionary<int, CameraGroupConfigData> GetCameraGroupConfig
    {
        get { return m_cameraGroupConfigList; }
    }

    public Dictionary<int, StroyDialogConfigData> GetStroyDialogConfig
    {
        get { return m_stroyDialogConfigList; }
    }

    public Dictionary<int, StroyActionConfigData> GetStroyActionConfig
    {
        get { return m_stroyActionConfigList; }
    }

    public Dictionary<int, StroyCameraConfigData> GetCameraConfig
    {
        get { return m_stroyCameraConfigList; }
    }

    public void RestroeAllList()
    {
        foreach (KeyValuePair<StroyLineKey, StroyLineConfigData> item in m_stroyLineConfigListBak)
        {
            m_stroyLineConfigList[item.Key]._BgMusic = item.Value._BgMusic;
            m_stroyLineConfigList[item.Key]._EctypeID = item.Value._EctypeID;
            m_stroyLineConfigList[item.Key]._SceneMapID = item.Value._SceneMapID;
            m_stroyLineConfigList[item.Key]._TriggerCondition = item.Value._TriggerCondition;
        }

        foreach (KeyValuePair<int, StroyActionConfigData> item in m_stroyActionConfigListBak)
        {
            m_stroyActionConfigList[item.Key]._Acceleration = item.Value._Acceleration;
            m_stroyActionConfigList[item.Key]._ActionName = item.Value._ActionName;
            m_stroyActionConfigList[item.Key]._ActionType = item.Value._ActionType;
            m_stroyActionConfigList[item.Key]._Duration = item.Value._Duration;
            m_stroyActionConfigList[item.Key]._EffectGo = item.Value._EffectGo;
            m_stroyActionConfigList[item.Key]._EffectLoopTimes = item.Value._EffectLoopTimes;
			m_stroyActionConfigList[item.Key]._EffectPosition = item.Value._EffectPosition;
			m_stroyActionConfigList[item.Key]._EffectStartAngle = item.Value._EffectStartAngle;
            m_stroyActionConfigList[item.Key]._EffectStartTime = item.Value._EffectStartTime;
            m_stroyActionConfigList[item.Key]._SoundName = item.Value._SoundName;
            m_stroyActionConfigList[item.Key]._SoundTime = item.Value._SoundTime;
            m_stroyActionConfigList[item.Key]._Speed = item.Value._Speed;
            m_stroyActionConfigList[item.Key]._StartAngle = item.Value._StartAngle;
            m_stroyActionConfigList[item.Key]._StartPosition = item.Value._StartPosition;
        }

        foreach (KeyValuePair<int, CameraGroupConfigData> item in m_cameraGroupConfigListBak)
        {
            m_cameraGroupConfigList[item.Key]._CameraID = item.Value._CameraID;
            m_cameraGroupConfigList[item.Key]._ActionList = item.Value._ActionList;

            for (int i = 0; i < item.Value._ActionList.Length; i++)
            {
                //m_cameraGroupConfigList[item.Key]._ActionList[i].AnimID.Clear();
                m_cameraGroupConfigList[item.Key]._ActionList[i].AnimID = item.Value._ActionList[i].AnimID;
            }
            
            
            ////m_cameraGroupConfigList[item.Key]._ActionType = item.Value._ActionType;
            ////m_cameraGroupConfigList[item.Key]._Duration = item.Value._Duration;
            ////m_cameraGroupConfigList[item.Key]._EffectGo = item.Value._EffectGo;
            ////m_cameraGroupConfigList[item.Key]._EffectLoopTimes = item.Value._EffectLoopTimes;
            ////m_cameraGroupConfigList[item.Key]._EffectPosition = item.Value._EffectPosition;
        }

        foreach (KeyValuePair<int, StroyCameraConfigData> item in m_stroyCameraConfigListBak)
        {
            m_stroyCameraConfigList[item.Key]._ActionTime = item.Value._ActionTime;
            m_stroyCameraConfigList[item.Key]._TargetID = item.Value._TargetID;
            m_stroyCameraConfigList[item.Key]._TargetPos = item.Value._TargetPos;
            m_stroyCameraConfigList[item.Key]._TargetType = item.Value._TargetType;
            for (int i = 0; i < item.Value._Params.Length; i++)
            {
                m_stroyCameraConfigList[item.Key]._Params[i]._EquA = item.Value._Params[i]._EquA;
                m_stroyCameraConfigList[item.Key]._Params[i]._EquB = item.Value._Params[i]._EquB;
                m_stroyCameraConfigList[item.Key]._Params[i]._EquC = item.Value._Params[i]._EquC;
                m_stroyCameraConfigList[item.Key]._Params[i]._EquD = item.Value._Params[i]._EquD;
            }
        }
    }
}
