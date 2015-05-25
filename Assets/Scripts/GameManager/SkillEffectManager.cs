using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public struct SkillEffectKey
{
    public Int64 UID;
    public int SkillID;

    public SkillEffectKey(Int64 uid, int skillId)
    {
        this.UID = uid;
        this.SkillID = skillId;
    }
};

public class SkillEffectManager {

    private Dictionary<SkillEffectKey, List<GameObject>> m_effectList = new Dictionary<SkillEffectKey, List<GameObject>>();
    private static SkillEffectManager m_instance;
   

    public static SkillEffectManager Instance
    {
        get { 
            if (m_instance == null)
            {
                m_instance = new SkillEffectManager();
            }

            return m_instance;
        }
    }

    public void AddEffect(SkillEffectKey _effectKey, GameObject effect)
    {
        if (!m_effectList.ContainsKey(_effectKey))
        {
            m_effectList.Add(_effectKey, new List<GameObject>());
        }
        if (effect != null)
        {
            m_effectList[_effectKey].Add(effect);
        }
    }

    public void RemoveEffect(SkillEffectKey _effectKey)
    {
        if (m_effectList.ContainsKey(_effectKey))
		{            
            m_effectList[_effectKey].ApplyAllItem(P =>
            {
                if (P != null)
                {
                    var actionEffectBehaviour = P.GetComponent<ActionEffectBehaviour>();
                    if (actionEffectBehaviour != null)
                    {
                        actionEffectBehaviour.StopByBreak();
                    }
                    GameObjectPool.Instance.Release(P);
                }
			});
            m_effectList.Remove(_effectKey);
		}
    }
}
