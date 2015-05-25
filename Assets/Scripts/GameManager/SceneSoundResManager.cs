using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// BattleUI/TownUI Scene SceneSoundResManager 
/// </summary>
public class SceneSoundResManager : MonoBehaviour {

    private static SceneSoundResManager _instance = null;
    
    public static SceneSoundResManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(SceneSoundResManager)) as SceneSoundResManager;
                if(_instance != null)
                {
                    _instance.Init();
                }
            }
            return _instance;
        }
    }

    public SoundClipList m_soundSfxList;
    private Dictionary<string, SoundClip> m_soundSfcDic;

    void Awake()
    {
        _instance = this;
        Init();
    }


    void OnDestroy()
    {
        _instance = null;
    }
    
    void Init()
    {
        if(null == m_soundSfcDic)
        {
            m_soundSfcDic = new Dictionary<string, SoundClip>();
            foreach(SoundClip clip in m_soundSfxList._soundList)
            {
                if(m_soundSfcDic.ContainsKey(clip._name))
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"same key : " + clip._name);
                }
                else
                {
                    m_soundSfcDic.Add(clip._name, clip);
                }
            }
        }
    }

    public SoundClip GetSceneSFXClip(string name)
    {
        SoundClip clip = null;
        m_soundSfcDic.TryGetValue(name,  out clip);
        return clip;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


}
