using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PlaySkillForUI : RoleBehaviour
{
    private List<SkillBase> m_skills;
    private int m_CurrentSkillID;

    new protected IEntityDataStruct m_roleDataModel;
    private long m_userID;
    protected bool m_isAnimPlayed;
    public bool IsSkillBeBreaked = false;
    private Vector3 m_skillFirePos;

    private SkillActionData m_actData;
    private SkinnedMeshRenderer PlayerModel;
    private List<Renderer> m_playerRendererDatas;
    private SkillBase m_skillBase;

    //private GameObject m_curBullet;
    //private GameObject m_curSkillEffect;

    void Awake()
    {
        this.CacheTransform();
        m_skills = GetComponents<SkillBase>().ToList();
        m_userID = PlayerManager.Instance.FindHeroDataModel().SMsg_Header.uidEntity;
    }

    //角色选择技能按钮，不改变当前技能，把选择的技能先缓存，在施放的时候才真正改变。
    public int m_nextSkillID;
    public SkillBase NextSkillBase {
        get
        {
            return this.m_skills.SingleOrDefault(P => P.SkillId == this.m_nextSkillID);
        }
    }

    public SkillSelectEffectController SkillSelectEffectController { get; set; }
    public SkillBase SelectedSkillBase { get; private set; }

    public void AddSkillBase(int skillID, bool isDefaultSkill)
    {
        if (!this.m_skills.Any(P => P.SkillId == skillID))
        {
            var newSkill = RoleGenerate.AttachSkill(gameObject, skillID);

            newSkill.AddActionDelegate(ActionHandler);
            newSkill.AddSkillOverDelegate(SkillOverHandler);
            newSkill.AddSkillBulletFireDelegate(FireSkillBullet);
            newSkill.AddSkillEffectFireDelegate(FireSkillActionEffect);

            newSkill.SetUserID = m_userID;
            this.m_skills.Add(newSkill);
            if (isDefaultSkill)
            {
                m_CurrentSkillID = skillID;
            }
        }
    }

    public Vector3 SkillFirePos
    {
        get { return m_skillFirePos; }
        set { m_skillFirePos = value; }
    }

    public void FireSkillBullet(int bulletId, bool useFirePos)
    {
        if (SelectedSkillBase == null)
        {
            TraceUtil.Log("选择的技能数据为空！");
            return;
        }

        if (useFirePos)
        {
            CreateBullet(bulletId, m_userID, ThisTransform, this.transform.position, CampType.CAMP_PLAYER, SelectedSkillBase.SkillData.m_affectTarget);
        }
        else
        {
            CreateBullet(bulletId, m_userID, ThisTransform);
        }
    }

    /// <summary>
    /// 施放技能调用创建子弹(无选择方向)
    /// </summary>
    /// <param name="bulletID">子弹索引id</param>
    /// <param name="EntityUID">施放者uid</param>
    /// <param name="heroTrans">施放者transform</param>
    private void CreateBullet(int bulletID, long EntityUID, Transform heroTrans)
    {
        this.CreateBullet(bulletID, EntityUID, heroTrans, null, CampType.CAMP_PLAYER, SelectedSkillBase.SkillData.m_affectTarget);
    }

    ///// <summary>
    ///// 返回 children transforms, 根据名称排列 .
    ///// </summary>
    //private Transform[] GetTransforms(GameObject parentGameObject)
    //{
    //    if (parentGameObject != null)
    //    {
    //        List<Component> components = new List<Component>(parentGameObject.GetComponentsInChildren(typeof(Transform)));
    //        List<Transform> transforms = components.ConvertAll(c => (Transform)c);

    //        transforms.Remove(parentGameObject.transform);
    //        transforms.Sort(delegate(Transform a, Transform b)
    //        {
    //            return a.name.CompareTo(b.name);
    //        });

    //        return transforms.ToArray();
    //    }

    //    return null;
    //}

    /// <summary>
    /// 施放技能调用创建子弹(有选择方向)
    /// </summary>
    /// <param name="bulletID">子弹索引id</param>
    /// <param name="EntityUID">施放者uid</param>
    /// <param name="heroTrans">施放者transform</param>
    /// <param name="targetPos">目标位置</param>
    public void CreateBullet(int bulletID, Int64 EntityUID, Transform heroTrans, Vector3? targetPos, CampType camp, int affectTarget)
    {
        /*
        if (bulletID == 0)
        {
            TraceUtil.Log("bulletID == 0");
            return;
        }

        BulletData bData = SkillDataManager.Instance.GetBulletData(bulletID);
        if (bData.m_mountType == 2)    //2的挂载类型表示子弹由服务器控制。
            return;

        if (bData == null)
        {
            TraceUtil.Log("找不到子弹配置信息");
            return;
        }
        BulletManager.Instance.CalendarIndex(EntityUID);
        if (bData.m_resource == null)
        {
            TraceUtil.Log("未配置子弹实体");
            return;
        }

        m_curBullet = (GameObject)Instantiate(bData.m_resource);

        Transform[] transList = m_curBullet.GetChildTransforms();
        foreach (Transform item in transList)
        {
            item.gameObject.layer = 25;
        }

        BulletBehaviour bulletBehaviour = m_curBullet.GetComponent<BulletBehaviour>();
        if (bulletBehaviour == null)
        {
            bulletBehaviour = m_curBullet.AddComponent<BulletBehaviour>();
        }
        bulletBehaviour.InitLocalBullet(bulletID, EntityUID, bData, camp, affectTarget, SelectedSkillBase.SkillData.m_skillId, -1);
        BulletManager.Instance.RegisteBullets(bulletBehaviour);

        #region 挂载类型子弹
        if (bData.m_mountType == 1)
        {
            m_curBullet.transform.position = heroTrans.transform.position;
            m_curBullet.transform.parent = heroTrans.transform;

            float lifeTime = bData.m_lifeTime / 1000f;
            bulletBehaviour.Fired(lifeTime, heroTrans);
            return;
        }

        #endregion

        //子弹运动轨迹(根据目标位置)
        float QuaterionY;
        float rad;
        Vector3 motionVector = Vector3.zero;
        Vector3 accelerationVector = Vector3.zero;

        if (targetPos == null)
        {
            QuaterionY = heroTrans.eulerAngles.y + bData.m_angle;
            float angel = 90 - QuaterionY;   //加上角度偏移

            rad = angel * Mathf.Deg2Rad;
            motionVector = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * bData.m_startSpeed;

        }
        else
        {
            float zVector = targetPos.Value.z - heroTrans.position.z;
            float xVector = targetPos.Value.x - heroTrans.position.x;
            rad = Mathf.Atan2(zVector, xVector);
            motionVector = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * bData.m_startSpeed;
            //子弹自身角度
            QuaterionY = 90 - rad * Mathf.Rad2Deg;
        }
        if (bData.m_acceleration != 0)
        {
            accelerationVector = motionVector * (bData.m_acceleration / bData.m_startSpeed);
        }
        float parseLifeTime = bData.m_lifeTime / 1000f;

        //子弹初始位置
        Vector3 startPos;
        if (targetPos == null)
        {
            startPos = heroTrans.position + heroTrans.TransformDirection(bData.m_initPos.y, 0, bData.m_initPos.x);
        }
        else
        {
            //\
            startPos = targetPos.Value + heroTrans.TransformDirection(bData.m_initPos.y, 0, bData.m_initPos.x);
        }

        TraceUtil.Log("parseLifeTime:" + parseLifeTime);
        bulletBehaviour.Fired(startPos, QuaterionY, motionVector, accelerationVector, parseLifeTime);
        */

    }

    public void ActionHandler(SkillActionData actData)
    {
        m_isAnimPlayed = false;
        m_actData = (SkillActionData)actData.Clone();
        ChangeDisplayState(m_actData.m_moveType);

        if (animation.GetClip(m_actData.m_animationId) == null)
        {
            TraceUtil.Log("当前角色不包含" + m_actData.m_animationId + "动画名称");
            return;
        }
        var t = Time.deltaTime;
        var s = m_actData.m_startSpeed * t + m_actData.m_acceleration * Mathf.Pow(t, 2) * 0.5f;

        m_actData.m_startSpeed += Mathf.FloorToInt(m_actData.m_acceleration * t);

        if (!m_isAnimPlayed || animation[m_actData.m_animationId].wrapMode == WrapMode.Loop)
        {
            animation.Play(m_actData.m_animationId);
            m_isAnimPlayed = true;
        }

    }

    public void BreakSkill()
    {
        if (m_skillBase != null)
        {
            m_skillBase.BreakSkill();
            m_skillBase.RemoveActionDelegate(ActionHandler);
            m_skillBase.DeleteSkillOverDelegate(SkillOverHandler);
            //m_skillBase.RemoveSkillEffectFireDelegate(FireSkillActionEffect);
            //m_skillBase.RemoveSkillBulletFireDelegate(FireSkillBullet);
            
            ChangeDisplayState(1);
        }
        /*

        if (m_curSkillEffect != null)
        {
            GameObjectPool.Instance.Release(m_curSkillEffect);
        }
        if (m_curBullet != null)
        {
            DestroyImmediate(m_curBullet);
        }
        */
    }

    /// <summary>
    /// 根据技能动作类型，隐身或显身
    /// </summary>
    /// <param name="moveType"></param>
    public void ChangeDisplayState(int moveType)
    {
        RefreshRenderCach();
        if (moveType == 0)
        {
            m_playerRendererDatas.ApplyAllItem(P => { if (P.enabled) P.enabled = false; });
        }
        else
        {
            m_playerRendererDatas.ApplyAllItem(P =>
            {
                //TraceUtil.Log(P.name + ": "+ P.enabled);
                if (!P.enabled) P.enabled = true;
            });
        }
    }

    /// <summary>
    /// 刷新玩家自身与装备的Renderer列表
    /// </summary>
    public void RefreshRenderCach()
    {
        List<PlayDataStruct<Renderer>> partRenderer;
        List<PlayDataStruct<SkinnedMeshRenderer>> mainRenderer;
        transform.RecursiveGetComponent<Renderer>("Renderer", out partRenderer);
        transform.RecursiveGetComponent<SkinnedMeshRenderer>("SkinnedMeshRenderer", out mainRenderer);

        PlayerModel = transform.GetComponentInChildren<SkinnedMeshRenderer>();

        if (this.m_playerRendererDatas == null)
        {
            this.m_playerRendererDatas = new List<Renderer>();
        }
        this.m_playerRendererDatas.Clear();

        this.m_playerRendererDatas.AddRange(partRenderer.Select(P => P.AnimComponent));
        this.m_playerRendererDatas.AddRange(mainRenderer.Select(P => (Renderer)P.AnimComponent));
    }

    public void RemoveSkillBase()
    {
        this.m_skills.ApplyAllItem(skill =>
        {
            if (skill.SkillId != m_CurrentSkillID)
            {
                skill.RemoveSkillBulletFireDelegate(FireSkillBullet);
                skill.RemoveSkillEffectFireDelegate(FireSkillActionEffect);
                skill.RemoveActionDelegate(ActionHandler);
                skill.AddSkillOverDelegate(SkillOverHandler);
                Component.Destroy(skill);
            }
        });
        this.m_skills.RemoveAll(P => P.SkillId != m_CurrentSkillID);
    }

    public void GetSkillByAniName(string aniString)
    {
        this.SelectedSkillBase = m_skills.SingleOrDefault(P => P.AniStr == aniString);
    }

    public void SkillOverHandler()
    {
        animation.Play("BIdle");
        ChangeDisplayState(1);
    }

    public void AddSkillBase(int skillID)
    {
        this.AddSkillBase(skillID, false);
    }


    public void GetSkillBySkillID(int? skillID)
    {
        if (skillID.HasValue)
        {
            m_CurrentSkillID = skillID.Value;
            this.SelectedSkillBase = m_skills.SingleOrDefault(P => P.SkillId == skillID);
        }
        else
        {
            this.SelectedSkillBase = null;
        }
    }


    public void PlaySkill(int skillID)
    {
        m_skillBase = m_skills.SingleOrDefault(P => P.SkillId == skillID);
        m_skillBase.AddActionDelegate(ActionHandler);
        m_skillBase.AddSkillOverDelegate(SkillOverHandler);
        //m_skillBase.AddSkillBulletFireDelegate(FireSkillBullet);
        //m_skillBase.AddSkillEffectFireDelegate(FireSkillActionEffect);
        GetSkillBySkillID(skillID);
        if (m_skillBase != null)
        {
            m_skillBase.PlaySkill();
        }
    }


    private void FireSkillActionEffect(int actionId, int skillID)
    {
        CreateActionEffect(actionId, skillID, m_userID);
    }

    public void CreateActionEffect(int actionID, int skillID, long entityUID)
    {
        //TraceUtil.Log("########actionID#" + actionID);
        //TraceUtil.Log("#########skillID#" + skillID);
        SkillActionData bData = SkillDataManager.Instance.GetSkillActionData(actionID);

        if (bData == null)
        {
            return;
        }
        /*
        if (bData.m_effect_resource == null)
        {
            return;
        }

        Vector3 startPos = transform.TransformPoint(bData.m_effect_start_pos.y, transform.localPosition.y, bData.m_effect_start_pos.x);  //配置表中的X对应3D中的Z，y对应3D中和X
        float rotationY = transform.eulerAngles.y + bData.m_effect_start_angel;


        m_curSkillEffect = GameObjectPool.Instance.AcquireLocal(bData.m_effect_resource, startPos, Quaternion.Euler(0, rotationY, 0));


        m_curSkillEffect.layer = 25;
        int num = m_curSkillEffect.transform.childCount;
        for (int i = 0; i < num; i++)
        {
            m_curSkillEffect.transform.GetChild(i).gameObject.layer = 25;
        }

        ActionEffectBehaviour actionEffectBehaviour = m_curSkillEffect.GetComponent<ActionEffectBehaviour>();
        if (actionEffectBehaviour == null)
        {
            actionEffectBehaviour = m_curSkillEffect.AddComponent<ActionEffectBehaviour>();
        }
        actionEffectBehaviour.InitDataConfig(bData, entityUID);
        */
    }

    public List<SkillBase> GetPlayerSkills()
    {
        return this.m_skills;
    }


    public override ObjectType GetFHObjType()
    {
        return ObjectType.Hero;
    }

    protected override void RegisterEventHandler()
    {
        return;
    }
}
