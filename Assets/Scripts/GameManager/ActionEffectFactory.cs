using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// EnterPoint Scene GameManager  动作特效工厂(怪物或角色发动攻击时)
/// </summary>
public class ActionEffectFactory : MonoBehaviour
{
    //public 
    private static ActionEffectFactory m_instance;
    public static ActionEffectFactory Instance
    {
        get
        {
            return m_instance;
        }
    }
    void Awake()
    {
        m_instance = this;       
    }
    public void CreateActionEffect(int actionID, int skillID, Int64 entityUID, Transform heroTrans)
    {
        this.CreateActionEffect(actionID, skillID, entityUID, heroTrans, heroTrans.position + heroTrans.TransformDirection(0, 0, 1f));
    }
        
    public void CreateActionEffect(int actionID, int skillID, Int64 entityUID, Transform heroTrans, Vector3 targetPos)
    {
        SkillActionData bData = SkillDataManager.Instance.GetSkillActionData(actionID);
        if (bData == null)
        {
            ////TraceUtil.Log("找不到动作特效配置信息");
            return;
        }
        if (bData.m_effectPath == "0")
        {
            ////TraceUtil.Log("未配置动作特效实体");
            return;
        }
        Vector3 startPos = heroTrans.TransformPoint(bData.m_effect_start_pos.y, heroTrans.localPosition.y, bData.m_effect_start_pos.x);  //配置表中的X对应3D中的Z，y对应3D中和X
        float rotationY = heroTrans.eulerAngles.y + bData.m_effect_start_angel;
        GameObject actionEffectPrefab = MapResManager.Instance.GetMapEffectPrefab(bData.m_effectPath);

        GameObject actionEffect = GameObjectPool.Instance.AcquireLocal(actionEffectPrefab, startPos, Quaternion.Euler(0, rotationY, 0)); 

        SkillEffectManager.Instance.AddEffect(new SkillEffectKey(entityUID, skillID), actionEffect);

        ActionEffectBehaviour actionEffectBehaviour = actionEffect.GetComponent<ActionEffectBehaviour>();
        if (actionEffectBehaviour == null)
        {
            actionEffectBehaviour = actionEffect.AddComponent<ActionEffectBehaviour>();
        }
        actionEffectBehaviour.InitDataConfig(bData, entityUID);
    }
}
