using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class BloodBarManager : Controller, ISingletonLifeCycle
{
    private static BloodBarManager m_instance;
    private Dictionary<Int64, EnemyHealthBar> m_BloodBarCache = new Dictionary<Int64, EnemyHealthBar>();

    public BloodBarManager()
    {

    }
    public static BloodBarManager Instance
    {
        get 
        {
            if (m_instance == null)
            {
                m_instance = new BloodBarManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
    /// <summary>
    /// 在实体工厂创建实体的时候调用些方法，给实体添加血条
    /// </summary>
    /// <param name="uid"></param>
    /// <param name="target"></param>
    public void AttachBarToTarget(Int64 uid, Transform target,GameObject bloodBarPrefab)
    {
        var bloodGo = GameObjectPool.Instance.AcquireLocal(bloodBarPrefab,target.position, Quaternion.identity);        

        TypeID entityType;
        var monsterModel=EntityController.Instance.GetEntityModel(uid,out entityType);
        if(monsterModel!=null)
        {
            var monsterData=((SMsgPropCreateEntity_SC_Monster)monsterModel.EntityDataStruct).MonsterUnitValues;
            var enemyHealthBar = bloodGo.GetComponent<EnemyHealthBar>();
            if (enemyHealthBar != null)
            {
                enemyHealthBar.InitMaxValue(target, monsterData.UNIT_FIELD_MAXHP, monsterModel);
                this.m_BloodBarCache[uid] = enemyHealthBar;//.Add(uid, enemyHealthBar);
            }
        }
    }
    /// <summary>
    /// 根据实体ID销毁血条
    /// </summary>
    /// <param name="uid"></param>
    public void DestroyBarViaEntityUid(Int64 uid)
    {
        if (m_BloodBarCache.ContainsKey(uid))
        {
            var enemyHealthBar = m_BloodBarCache[uid];
            if (enemyHealthBar != null)
            {
                GameObjectPool.Instance.Release(enemyHealthBar.gameObject);
				
            }
            this.m_BloodBarCache.Remove(uid);
        }
    }
   
    protected override void RegisterEventHandler()
    {
       
    }

    public void Instantiate()
    {

    }

    public void LifeOver()
    {
        this.ClearEvent();
        m_instance = null;
    }
}
