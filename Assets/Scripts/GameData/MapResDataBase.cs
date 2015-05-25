using UnityEngine;
using System.Collections;

[System.Serializable]
public class MonsterObjectGroup
{
    public int m_monsterId;
    public GameObject m_monsterPrefab;
    public GameObject m_monsterBloodEffectPrefab;
}

[System.Serializable]
public class EffectObjGroup
{
    public string m_effectPath;
    public GameObject m_effectPrefab;
}


public class MapResDataBase : ScriptableObject
{
    public MonsterObjectGroup[] m_monsters;
    public EffectObjGroup[] m_effects;
    public SoundClip[] m_sfxClips;
}
