using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

//using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class SkillDataManager : MonoBehaviour {
	
	public SkillConfigDataBase m_skillConfigDataBase;
	private Dictionary<int, SkillConfigData> m_skillConfigDataDictionary = new Dictionary<int, SkillConfigData>();
	
	public SkillActionDataBase m_skillActionDataBase;
	private Dictionary<int, SkillActionData> m_skillActionDataDictionary = new Dictionary<int, SkillActionData>();
	
	public BulletDataBase m_bulletDataBase;
	private Dictionary<int, BulletData> m_bulletDataDictionary = new Dictionary<int, BulletData>();
	
	public BulletImpactDataBase m_bulletImpactDataBase;
	private Dictionary<int, BulletImpactData> m_bulletImpactDataDictionary = new Dictionary<int, BulletImpactData>();

    public SirenSkillDataBase m_sirenSkillDataBase;
    private Dictionary<int, SirenSkillData> m_sirenSkillDataDict = new Dictionary<int, SirenSkillData>();

    public SkillCameraConfigDataBase m_skillCameraConfigDataBase;
    private Dictionary<int, SkillCameraData> m_skillCameraDataDictionary = new Dictionary<int, SkillCameraData>();


	private static SkillDataManager m_instance;
    public static SkillDataManager Instance
    {
        get
        {
            return m_instance;
        }
    }
	
	void Load()
	{
        foreach (BulletData data in m_bulletDataBase._dataTable)
        {
            m_bulletDataDictionary[data.m_bulletId] = data;
        }
        foreach (SkillActionData data in m_skillActionDataBase._dataTable)
        {
            m_skillActionDataDictionary[data.m_actionId] = data;
        }
		foreach(SkillConfigData data in m_skillConfigDataBase._dataTable)
		{
            //根据技能的Action配置决定该技能是否需要进行锁定目标计算
            data.IsLockTarget = data.m_actionId.Any(P => m_skillActionDataDictionary[P].m_ani_followtype == 1);
			m_skillConfigDataDictionary[data.m_skillId] = data;
		}
		
		foreach(BulletImpactData data in m_bulletImpactDataBase._dataTable)
		{
			m_bulletImpactDataDictionary[data.m_id] = data;	
			
		}
        foreach(SirenSkillData data in m_sirenSkillDataBase._dataTable)
        {
            m_sirenSkillDataDict[data._vocation] = data;
        }

        foreach (var item in m_skillCameraConfigDataBase._dataTable)
        {
            m_skillCameraDataDictionary[item._CameraID] = item;
        }
	}
	/*
	void LoadExtraData()
	{
		string dataPath = Application.dataPath;
		string skillConfigDataPath = System.IO.Path.Combine(dataPath, "SkillConfig.xml");
		if(File.Exists(skillConfigDataPath))
		{
			TextReader tr = new StreamReader(skillConfigDataPath);
		    string text = tr.ReadToEnd();
		
		    if (text == null)
		    {
		        TraceUtil.Log("Extra skill config file not exist");
		        return;
		    }
			else
			{
				XmlSpreadSheetReader.ReadSheet(text);
		        XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
		        string[] keys = XmlSpreadSheetReader.Keys;
		
		        object[] levelIds = sheet[keys[0]];
		
		        
				 
		        for (int i = 2; i < levelIds.Length; i++)
		        {	
					TraceUtil.Log("skill id :" + levelIds[i]);
					int skillId = Convert.ToInt32(sheet["skill_id"][i]);
					m_skillConfigDataDictionary[skillId].m_name = Convert.ToString(sheet["name"][i]);
				}
				
			}
			
		}
		
		
	}
	*/
	
	void Awake()
	{

		m_instance = this;	
		Load();

		//LoadExtraData();
	}

    void OnDestroy()
    {
        m_instance = null;
    }
	// Use this for initialization
	void Start () 
	{
	
	}
	
	public SkillConfigData GetSkillConfigData(int skillId)
	{
		SkillConfigData data = null;
		m_skillConfigDataDictionary.TryGetValue(skillId, out data);
		return data;
	}
	
	public SkillConfigData GetNormalAttackSkillId(int vocation)
	{
		foreach(SkillConfigData data in m_skillConfigDataDictionary.Values)
		{
			if(data.m_vocation == vocation)	
			{
				return data;	
			}
		}
		return null;
	}
	
	public SkillActionData GetSkillActionData(int actionId)
	{
		SkillActionData data = null;
		m_skillActionDataDictionary.TryGetValue(actionId, out data);
		return data;
	}
	
	public BulletData GetBulletData(int bulletId)
	{
		BulletData data = null;
		m_bulletDataDictionary.TryGetValue(bulletId, out data);
		return data;
	}
	
	public BulletImpactData GetBulletImpactData(int bulletImpactId)
	{
		BulletImpactData data = null;
		m_bulletImpactDataDictionary.TryGetValue(bulletImpactId, out data);
		if(data == null)
		{
			if(bulletImpactId != 0)
			{
				TraceUtil.Log(SystemModel.Common, TraceLevel.Error,"子弹结算:"+bulletImpactId.ToString()+" 错误");		             
				TraceUtil.Log(SystemModel.NotFoundInTheDictionary, TraceLevel.Error,"子弹结算:"+bulletImpactId.ToString()+" 错误");		             
			}
		}
		return data;
	}

    public SirenSkillData GetSirenSkillData(int vocation)
    {
        SirenSkillData data = null;
        m_sirenSkillDataDict.TryGetValue(vocation, out data);
        return data;
    }

    public SkillCameraData GetSkillCameraData(int cameraId)
    {
        if (m_skillCameraDataDictionary.ContainsKey(cameraId))
            return m_skillCameraDataDictionary[cameraId];
        else
            return null;
    }

}
