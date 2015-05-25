using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class BulletManager : Controller, IEntityManager, ISingletonLifeCycle
{
    private List<EntityModel> m_bulletList = new List<EntityModel>();
    private List<EntityModel> m_localBulletList = new List<EntityModel>();

    private List<Caster> m_CasterList = new List<Caster>(); //子弹释放者列表
    private bool m_destroyBullet;    //是否响应网络消息删除子弹
    private static BulletManager m_instance;
    public static BulletManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new BulletManager();
                SingletonManager.Instance.Add(m_instance);
                EntityController.Instance.RegisteManager(TypeID.TYPEID_BULLET, m_instance);                
            }
            return m_instance;
        }
    }


    protected override void RegisterEventHandler()
    {
        m_destroyBullet = true;
        //进入结算界面，不再处理网络的子弹消息。让本地子弹自生自灭
        AddEventHandler(EventTypeEnum.EctypeSettleAccount.ToString(), StopProcessNetworkMsg);
        AddEventHandler(EventTypeEnum.SceneChange.ToString(), SceneChangeHandle);
        AddEventHandler(EventTypeEnum.EntityDie.ToString(), EntityDieHandle);
    }

    void EntityDieHandle(INotifyArgs e)
    {
        SMsgActionDie_SC sMsgActionDie_SC = (SMsgActionDie_SC)e;
        this.DestroyAllBullets(sMsgActionDie_SC.uidEntity);
    }
    private void SceneChangeHandle(INotifyArgs e)
    {
        m_destroyBullet = true;
        //销毁网络子弹
        this.m_bulletList.ApplyAllItem(targetEntity =>
        {
            //TraceUtil.Log("销毁子弹:" + targetEntity.GO.name);
            targetEntity.DestroyEntity();
        });
        m_bulletList.Clear();
        //销毁本地子弹
        this.m_localBulletList.ApplyAllItem(targetEntity =>
        {
            //TraceUtil.Log("销毁子弹:" + targetEntity.GO.name);
            targetEntity.DestroyEntity();
        });
        m_localBulletList.Clear();
    }
    void StopProcessNetworkMsg(INotifyArgs inotifyArgs)
    {
        m_destroyBullet = false;
    }
    public EntityModel GetEntityMode(long uid)
    {        
        throw new System.NotImplementedException();
    }

    public void RegisteEntity(EntityModel bulletData)
    {
        if (m_bulletList.Exists(P => P.EntityDataStruct != null && P.EntityDataStruct.SMsg_Header.uidEntity == bulletData.EntityDataStruct.SMsg_Header.uidEntity))
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"收到服务器重复创建同ID的实体！");
        }
        else
        {
            m_bulletList.Add(bulletData);
        }
    }

    /// <summary>
    /// 网络销毁(实体销毁)
    /// </summary>
    /// <param name="uid"></param>
    public void UnRegisteEntity(Int64 uid)
    {
        if (!m_destroyBullet)
            return;
        var targetEntity = this.m_bulletList.SingleOrDefault(P => P.EntityDataStruct.SMsg_Header.uidEntity == uid);
        if (targetEntity != null)
        {
            m_bulletList.Remove(targetEntity);

            targetEntity.DestroyEntity();

            //TraceUtil.Log("子弹实体销毁");
        }        
    }

    /// <summary>
    /// 子弹销毁(网络战斗销毁和本地销毁)
    /// </summary>
    /// <param name="index"></param>
    /// <param name="casterID"></param>
    public void UnRegisteEntity(ulong index, Int64 casterID)
    {
        if (!m_destroyBullet)
            return;
        //TraceUtil.Log("子弹销毁 index" + index+"  "+Time.realtimeSinceStartup);
        //var targetEntity = this.m_bulletList.SingleOrDefault(p => ((BulletBehaviour)p.Behaviour).BulletIndex == index && ((BulletBehaviour)p.Behaviour).FormEntityID == casterID);
        var CasterList = this.m_localBulletList.Where(p => ((BulletBehaviour)p.Behaviour).FormEntityID == casterID).ToList();
        var targetEntity = CasterList.Where(p => ((BulletBehaviour)p.Behaviour).BulletIndex == index);
        targetEntity.ApplyAllItem(p =>
            {
                m_localBulletList.Remove(p);
                p.DestroyEntity();
                //TraceUtil.Log("子弹战斗销毁" + index);
            });
        //if (targetEntity != null)
        //{
        //    m_localBulletList.Remove(targetEntity);
        //    targetEntity.DestroyEntity();
        //}
        //else
        //{
        //    //TraceUtil.Log("找不到子弹 bulletList.Length = " + m_bulletList.Count);
        //}

    }

    /// <summary>
    /// 同步子弹数量
    /// </summary>
    /// <param name="index">子弹序号</param>
    public void SynchronousEntity(ulong index)
    {
        SynchronousEntity(index, -1);
    }
    /// <summary>
    /// 同步子弹
    /// </summary>
    /// <param name="index">子弹序号</param>
    /// <param name="entityUID">施放者id</param>
    public void SynchronousEntity(ulong index, Int64 entityUID)
    {
        //子弹打断有销毁信息，这里不用作子弹销毁处理

        //把多余的子弹成组并删除
        //var casterList = m_localBulletList.Where(p => ((BulletBehaviour)p.Behaviour).FormEntityID == entityUID);

        //判断子弹序号是提前还是滞后
        //UInt64 bulletIndex = BulletManager.Instance.ReadIndex(entityUID);
        //if (bulletIndex > index)
        //{

        //}

        
        //var bullets = casterList.Where(p => ((BulletBehaviour)p.Behaviour).BulletIndex > index ).ToList();

        //int length = bullets.Count;
        //for (int i = 0; i < length; i++)
        //{
        //    m_localBulletList.Remove(bullets[i]);
        //    bullets[i].DestroyEntity();            
        //}       
        //TraceUtil.Log("同步子弹=>" + index + " entityUID: " + entityUID);
        //同步序号
        var caster = m_CasterList.SingleOrDefault(p => p.CasterID == entityUID);
        if (caster != null)
        {
            caster.BulletIndex = index + 1;
        }
        
    }

    /// <summary>
    /// 本地注册
    /// </summary>
    /// <param name="bulletBehaviour"></param>
    public void RegisteBullets(BulletBehaviour bulletBehaviour)
    {
        EntityModel bulletModel = new EntityModel();
        bulletModel.GO = bulletBehaviour.gameObject;
        bulletModel.Behaviour = bulletBehaviour;

        m_localBulletList.Add(bulletModel);
    }
	
	public void UnRegisteLocalBullets(BulletBehaviour bulletBehaviour)
	{
		foreach(EntityModel bullet in m_localBulletList)
		{
			if(bullet.Behaviour == bulletBehaviour)
			{
				m_localBulletList.Remove(bullet);
				bullet.DestroyEntity();
			}
		}
	}

    /// <summary>
    /// 技能打断子弹销毁
    /// </summary>
    /// <param name="casterID">发射者ID</param>
    public void TryDestroyBreakBullets(Int64 casterID)
    {
        var bulletArray = m_localBulletList.Where(p => ((BulletBehaviour)p.Behaviour).FormEntityID == casterID).ToArray();
        int length = bulletArray.Length;
        for (int i = 0; i < length; i++)
        {
            if (((BulletBehaviour)bulletArray[i].Behaviour).IsBreakType())
            {
                m_localBulletList.Remove(bulletArray[i]);
                bulletArray[i].DestroyEntity();
            }            
        }
    }

    private void DestroyAllBullets(Int64 casterID)
    {
        var bulletArray = m_localBulletList.Where(p => ((BulletBehaviour)p.Behaviour).FormEntityID == casterID &&
		                                          ((BulletBehaviour)p.Behaviour).BulletData.m_breakType != 2 ).ToArray();
        int length = bulletArray.Length;
        for (int i = 0; i < length; i++)
        {
            m_localBulletList.Remove(bulletArray[i]);
            bulletArray[i].DestroyEntity();
        }
    }

    //public List<EntityModel> GetBulletBehaviourList()
    //{
    //    return m_bulletList;
    //}

    /// <summary>
    /// 计算子弹序号
    /// </summary>
    /// <param name="entityID"></param>
    /// <returns></returns>
    public ulong CalendarIndex(Int64 entityID)
    {
        //lock (m_CasterList)
        //{
            var caster = m_CasterList.SingleOrDefault(p => p.CasterID == entityID);
            if (caster == null)
            {
                caster = new Caster(entityID);
                m_CasterList.Add(caster);
            }
            return caster.BulletIndex;
        //}
    }
    /// <summary>
    /// 读取子弹序号
    /// </summary>
    /// <param name="entityID"></param>
    /// <returns></returns>
    public ulong ReadIndex(Int64 entityID)
    {
        //lock (m_CasterList)
        //{
            var caster = m_CasterList.SingleOrDefault(p => p.CasterID == entityID);
            if (caster == null)
            {
                caster = new Caster(entityID);
                m_CasterList.Add(caster);
            }
            return caster.ReadIndex;
        //}
    }
    //子弹释放者类
    public class Caster
    {
        public Int64 CasterID;
        private ulong mBulletIndex = 1; //服务器那边是从1开始算计
        public Caster(Int64 CasterID)
        {
            this.CasterID = CasterID;
        }
        public ulong BulletIndex
        {
            set { mBulletIndex = value; }
            get
            {
                ulong index = mBulletIndex++;
                //mBulletIndex++;
                //TraceUtil.Log("Calc bullet : " + index);
                return index;
            }
        }
        public ulong ReadIndex
        {
            get
            {
                ulong index = mBulletIndex - 1;
                return index;
            }
        }
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
