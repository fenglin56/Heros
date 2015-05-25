using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class BuffController:ISingletonLifeCycle
{
    struct BuffKey
    {
        public long UID;
        public uint Index;
    }
    //private Dictionary<ushort, SMsgActionWorldObjectAddBuff_SC> m_evtList = new Dictionary<ushort, SMsgActionWorldObjectAddBuff_SC>();
    private Dictionary<BuffKey, BuffEffectBase> m_buffEffectList = new Dictionary<BuffKey, BuffEffectBase>();
    
    //private bool m_handled = false;
    //private BuffEffectBase m_buffEffect = new BuffEffectBase();

    private Animation m_anim;
    private Action m_onOver;

    struct BuffEffectBase
    {
        public GameObject _effect;
        public ushort _buffID;
    };

    private static BuffController m_instance;

    public static BuffController Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BuffController();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    //void FireBuff(SMsgActionWorldObjectAddBuff_SC evt)
    //{
        
    //    //Transform target = PlayerManager.Instance.FindPlayer(evt.SMsgActionSCHead.uidEntity).transform;

    //    if (m_buffEffectList.ContainsKey(evt.dwIndex))
    //    {
    //        ReleaseBuff(evt.dwIndex);    
    //    }

    //    if (!m_handled)
    //    {
    //        m_handled = true;
    //        LaunchBuff(evt);    
    //    }
    //}

    public void LaunchBuff(SMsgActionWorldObjectAddBuff_SC evt)
    {

        TypeID entityType;
        EntityModel  entityTarget = EntityController.Instance.GetEntityModel(evt.SMsgActionSCHead.uidEntity, out entityType);
        if (entityTarget==null || entityTarget.GO == null)
            return;

        GameObject effectPrefab = BattleConfigManager.Instance.BuffConfigList[evt.dwBuffId]._buffEffectPrefab;
		
        if (null != effectPrefab)
        {
            Transform buffTarget = null;

            if (BattleConfigManager.Instance.BuffConfigList[evt.dwBuffId]._buffEffMount != "0")
            {
                entityTarget.GO.transform.RecursiveFindObject(BattleConfigManager.Instance.BuffConfigList[evt.dwBuffId]._buffEffMount, out buffTarget);
                if (buffTarget == null)
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"[buffTarget == null]");
                    buffTarget = entityTarget.GO.transform;
                }
            }
            else
            {
                buffTarget = entityTarget.GO.transform;
            }
            
			//add by lee : add sound
			if(BattleConfigManager.Instance.BuffConfigList[evt.dwBuffId]._buffSound != "0")
			{
				SoundManager.Instance.PlaySoundEffect(BattleConfigManager.Instance.BuffConfigList[evt.dwBuffId]._buffSound);
			}

            GameObject effectObj = GameObjectPool.Instance.AcquireLocal(effectPrefab, buffTarget.position, buffTarget.rotation);
            effectObj.transform.parent = buffTarget;
            effectObj.GetComponent<BuffEffectController>().Emit();
            BuffEffectBase buffEffect = new BuffEffectBase();
            buffEffect._effect = effectObj;
            buffEffect._buffID = evt.dwBuffId;

            BuffKey key = new BuffKey();
            key.UID = evt.SMsgActionSCHead.uidEntity;
            key.Index = evt.dwIndex;
            m_buffEffectList.Add(key, buffEffect);  
        }
        
        //PlaySoundEffect(evt.Param);
    }

    /// <summary>
    /// 动画和Buff触发函数
    /// </summary>
    [ContextMenu("BuffStart")]
    //public void BuffStart(SMsgActionWorldObjectAddBuff_SC evt)
    //{
        
    //    if (!m_evtList.ContainsKey(evt.dwBuffId))
    //    {
    //        m_evtList.Add(evt.dwBuffId, evt);
    //        this.m_handled = false;
    //        FireBuff(evt);
    //    }
    //    else if (m_evtList[evt.dwBuffId].dwLevel < evt.dwLevel)
    //    {
    //        m_evtList.Remove(evt.dwBuffId);
    //        m_buffEffectList[evt.dwIndex]._effect.GetComponent<BuffEffectController>().SelfRelease();
    //        m_evtList.Add(evt.dwBuffId, evt);
    //        this.m_handled = false;
    //        FireBuff(evt);
    //    }
    //}

    /// <summary>
    /// 删除Buff动画和特效
    /// </summary>
    /// <param name="index"></param>
    public void Remove(SMsgActionWorldObjectRemoveBuff_SC  removeItem)
    {
        BuffKey key = new BuffKey();
        key.UID = removeItem.SMsgActionSCHead.uidEntity;
        key.Index = removeItem.DwIndex;

        if (m_buffEffectList.ContainsKey(key))
        {
            //m_evtList.Remove(m_buffEffectList[index]._buffID);
            ReleaseBuff(key);
        }
        //m_handled = false;
    }


    void ReleaseBuff(BuffKey index)
    {
		//TraceUtil.Log( "Remove Buff UID" +  index.UID + "   index:" + index.Index);
		if( m_buffEffectList[index]._effect != null)
		{
        	m_buffEffectList[index]._effect.GetComponent<BuffEffectController>().SelfRelease();
		}
        m_buffEffectList.Remove(index);
    }

    void PlaySoundEffect(int index)
    {
        //SoundManager.Instance.PlaySoundEffect(SoundList[index]);
    }

    public void SetAnimation(Animation anim, Action act)
    {
        m_anim = anim;
        m_onOver = act;
    }

    public void OnDestroy()
    {
        m_instance = null;
        m_anim = null;
    }


    public void Instantiate()
    {
        
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}
