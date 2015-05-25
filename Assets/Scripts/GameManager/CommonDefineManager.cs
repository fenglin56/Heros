using UnityEngine;
using System.Collections;
using System.Linq;

public class CommonDefineManager : MonoBehaviour {
	
    public IllegalCharacterDataBase IllegalCharacterConfig;
    public IllegalNameDataBase IllegalNameConfig;
    public CommonDefineDataBase CommonDefineFile;
    public TraceConfigDataBase TraceConfigDataBase;
	public PropKeyConfigDataBase PropKeyConfigDataBase;

    private CommonDefineData m_commonDefineData;

    private static CommonDefineManager m_instance;
    public static CommonDefineManager Instance
    {
        get {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<CommonDefineManager>();
            }
            return m_instance; 
        }
    }
    void Awake()
    {
        m_instance = this;

        if (CommonDefineFile != null)
            m_commonDefineData = CommonDefineFile._dataTable;
    }
	
    public CommonDefineData CommonDefine
    {
        get { return m_commonDefineData; }
    }

    public Vector3 GetCameraDistanceFromPlayer()
    {
        int level = GameManager.Instance.m_gameSettings.GameViewLevel;
        if(GameManager.Instance.CurrentState == GameManager.GameState.GAME_STATE_TOWN)
        {
            return m_commonDefineData.CameraDistanceTownList[level];
        }
        else
        {
            return m_commonDefineData.CameraDistanceList[level];
        }


    }

	/// <summary>
	/// 根据二型结算id查找属性index
	/// </summary>
	/// <returns>属性index</returns>
	/// <param name="settleID">二型结算id</param>
	public int GetPropKey(int settleID)
	{
		int propKey = -1;
		var propData = PropKeyConfigDataBase._dataTable.SingleOrDefault(p=>p.nSettleID == settleID);
		if(propData!=null)
		{
			propKey = propData.nPropID;
		}
		return propKey;
	}

    //[ContextMenu("生成Trace对象")]
    //public void GeneratorTraceObject()
    //{
    //    var database = ScriptableObject.CreateInstance<TraceConfigDataBase>();
    //    //database.TraceConfigDataTable = new TraceConfigData[];
    //    //for (int i = 0; i < levelIds.Length; i++)
    //    //{
    //    //    if (0 == i || 1 == i) continue;
    //    //    var configData = new IllegalCharacterData();
    //    //    configData.Index = j;
    //    //    configData.IllegalCharacter = Convert.ToString(sheet["BanWords"][i]);
    //    //    database.IllegalCharacterDataTable[j++] = configData;
    //    //}
    //    UnityEditor.AssetDatabase.CreateAsset(database, "Assets/Data/TraceConfigDataBase.asset");
    //}
}
