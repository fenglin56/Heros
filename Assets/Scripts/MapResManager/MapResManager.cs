using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapResManager : MonoBehaviour {

    private static MapResManager _instance = null;
    
    public static MapResManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType(typeof(MapResManager)) as MapResManager;
                if(_instance != null)
                {
                    _instance.Init();
                }
            }
            
            return _instance;
        }
    }



    public MapResDataBase m_mapMonsterRes;
    public PlayerEffectDataBase m_playerEffectDataBase;


    private Dictionary<int, GameObject> m_mapMonstersDic;

    private Dictionary<int, GameObject> m_monsterBloodEffectDic;


    private Dictionary<string, GameObject> m_mapEffectsDic;

    private Dictionary<string, SoundClip> m_mapSfxDic;

    void Awake()
    {
        _instance = this;
        Init();
    }

    void OnDestroy()
    {
        m_mapMonstersDic.Clear();
        m_mapEffectsDic.Clear();
        m_monsterBloodEffectDic.Clear();

        m_mapMonstersDic = null;
        m_mapEffectsDic = null;
        m_monsterBloodEffectDic = null;
        _instance = null;
    }

    void Init()
    {
        if(null == m_mapMonsterRes)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Missing Map DataBase");

        }
        if(null == m_mapMonstersDic)
        {
            m_mapMonstersDic = new Dictionary<int, GameObject>();
            foreach(MonsterObjectGroup mog in m_mapMonsterRes.m_monsters)
            {
                m_mapMonstersDic.Add(mog.m_monsterId, mog.m_monsterPrefab);
            }
        }
        if(null == m_monsterBloodEffectDic)
        {
            m_monsterBloodEffectDic = new Dictionary<int, GameObject>();
            foreach(MonsterObjectGroup mog in m_mapMonsterRes.m_monsters)
            {
                m_monsterBloodEffectDic.Add(mog.m_monsterId, mog.m_monsterBloodEffectPrefab);
            }
        }

        if(null == m_mapEffectsDic)
        {
            m_mapEffectsDic = new Dictionary<string, GameObject>();
            foreach(EffectObjGroup eog in m_mapMonsterRes.m_effects)
            {
                if(!m_mapEffectsDic.ContainsKey(eog.m_effectPath))
                {
                    m_mapEffectsDic.Add(eog.m_effectPath, eog.m_effectPrefab);
                }
            }
            if(m_playerEffectDataBase != null)
            {
                foreach(EffectObjGroup eog in m_playerEffectDataBase.m_playerReses)
                {
                    if(!m_mapEffectsDic.ContainsKey(eog.m_effectPath))
                    {
                        m_mapEffectsDic.Add(eog.m_effectPath, eog.m_effectPrefab);
                    }
                }
            }
        }

        if(null == m_mapSfxDic)
        {
            m_mapSfxDic = new Dictionary<string, SoundClip>();
            foreach(SoundClip clip in m_mapMonsterRes.m_sfxClips)
            {
                if(!m_mapSfxDic.ContainsKey(clip._name))
                {
                    m_mapSfxDic.Add(clip._name, clip);
                }
            }
            if(m_playerEffectDataBase != null)
            {
                foreach(SoundClip clip in m_playerEffectDataBase.m_playerSfxs)
                {
                    if(!m_mapSfxDic.ContainsKey(clip._name))
                    {
                        m_mapSfxDic.Add(clip._name, clip);
                    }
                }
            }
        }
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public GameObject GetMapMonsterPrefab(int monsterId)
    {
        GameObject monsterPrefab = null;
        if(!m_mapMonstersDic.TryGetValue(monsterId, out monsterPrefab))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Don't Find Monster in map Res, Monster Id:" + monsterId);
        }

        return monsterPrefab;

    }

    public GameObject GetMapMonsterBloodEffectPrefab(int monsterId)
    {
        GameObject monsterBloodEffectPrefab = null;
        if(!m_monsterBloodEffectDic.TryGetValue(monsterId, out monsterBloodEffectPrefab))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Don't Find Monster in map Res, Monster Id:" + monsterId);
        }
        
        return monsterBloodEffectPrefab;

    }

    public GameObject GetMapEffectPrefab(string path)
    {
        GameObject effectPrefab = null;
        if(!m_mapEffectsDic.TryGetValue(path, out effectPrefab))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Don't Find Effect in map Res, Effect Path;" + path);
        }
        return effectPrefab;
    }

    public SoundClip GetMapSfxSoundClip(string name)
    {
        SoundClip clip = null;
        m_mapSfxDic.TryGetValue(name, out clip);
        return clip;
    }
}
