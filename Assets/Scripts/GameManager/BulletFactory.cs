using UnityEngine;
using System.Collections;
using System.Linq;
using System;

/// <summary>
/// BattleUI Scene BattleDataManager  子弹工厂
/// </summary>
public class BulletFactory : MonoBehaviour 
{		
    //\测试数据
    public GameObject CircleGO;
    public GameObject SquareGO;
    public GameObject FanGO;

    public bool isShowTestBullet = true;  //是否显示子弹击打范围    

    public bool isShowSquareBullet = true;  //显示方形子弹
    public bool isShowFanBullet = true; //显示扇形子弹
    public float StartBulletFireTime=0;

    private static BulletFactory m_instance;
    public static BulletFactory Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType(typeof(BulletFactory)) as BulletFactory;
            }
            return m_instance;
        }
    }
    void OnDestroy()
    {
        m_instance = null;
    }
    void Awake()
    {
        m_instance = this;
    }

    

    public void Register(IEntityDataStruct entityDStruct)
    {               
        CreateBullet(entityDStruct);
    }

    /// <summary>
    /// 施放技能调用创建子弹(无选择方向)
    /// </summary>
    /// <param name="bulletID">子弹索引id</param>
    /// <param name="EntityUID">施放者uid</param>
    /// <param name="heroTrans">施放者transform</param>
    
    public void CreateBullet(int bulletID, Int64 EntityUID, Transform heroTrans, int skillId, long TargetId)
    {        
        ////TraceUtil.Log("子弹id: " + bulletID);
        this.CreateBullet(bulletID, EntityUID, heroTrans, null, skillId, TargetId);
    }
    
    /// <summary>
    /// 施放技能调用创建子弹(有选择方向)
    /// </summary>
    /// <param name="bulletID">子弹索引id</param>
    /// <param name="EntityUID">施放者uid</param>
    /// <param name="heroTrans">施放者transform</param>
    /// <param name="targetPos">目标位置</param>
    public void CreateBullet(int bulletID, Int64 EntityUID, Transform heroTrans, Vector3? targetPos, int skillId, long TargetId)
    {
        if (bulletID == 0)
        {
            //TraceUtil.Log("bulletID == 0");
            return;            
        }

        BulletData bData = SkillDataManager.Instance.GetBulletData(bulletID);
        if (bData.m_mountType == 2 && GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)    //2的挂载类型表示子弹由服务器控制。
            return;
		if(GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER && bData.m_lifeTime != 0 && bData.m_overParam == 2)	//多人模式下，此类型子弹只有后台发送消息时才进行创建
			return;

        if (bData == null)
        {
            TraceUtil.Log( SystemModel.Common, TraceLevel.Error,"子弹:"+bulletID.ToString()+ " 错误");
			TraceUtil.Log( SystemModel.NotFoundInTheDictionary, TraceLevel.Error,"子弹:"+bulletID.ToString()+ " 错误");
            return;
        }
        //前端无论是否生成子弹，子弹序号也需要计算。才会与服务端同步。这个位置不要调整
        BulletManager.Instance.CalendarIndex(EntityUID);
        if (bData.m_bulletResPath == "0")
        {
            //TraceUtil.Log("未配置子弹实体");
            return;
        }
        GameObject bulletPrefab = MapResManager.Instance.GetMapEffectPrefab(bData.m_bulletResPath);
        if(null == bulletPrefab)
        {
            Debug.LogError("Bullet res not found in map res : " + bData.m_bulletId);
        }

        GameObject bullet = (GameObject)Instantiate(bulletPrefab);
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        if (bulletBehaviour == null)
        {
            bulletBehaviour = bullet.AddComponent<BulletBehaviour>();
        }
        bulletBehaviour.InitLocalBullet(bulletID, EntityUID,bData, skillId, TargetId);
        BulletManager.Instance.RegisteBullets(bulletBehaviour);

        #region 挂载类型子弹
        if (bData.m_mountType == 1)
        {
            bullet.transform.position = heroTrans.transform.position;
            bullet.transform.parent = heroTrans.transform;

            float lifeTime = bData.m_lifeTime / 1000f;
            bulletBehaviour.Fired(lifeTime, heroTrans);
            return;
        }

        #endregion
        //跟踪子弹类型
        if (bData.m_mountType == 5)
        {
            float lifeTime = bData.m_lifeTime / 1000f;
            TypeID type;
            var targetData = EntityController.Instance.GetEntityModel(TargetId, out type);
			if(targetData != null)
			{
                Vector3 createPoint = heroTrans.TransformPoint(bData.m_initPos.y, 0, bData.m_initPos.x);
                bulletBehaviour.FollowFired(lifeTime, createPoint, targetData.GO.transform, bData.m_startSpeed);
            }
            return;
        }


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
            QuaterionY = 90 - rad * Mathf.Rad2Deg ;
        }
        if(bData.m_acceleration != 0)
        {
            accelerationVector = motionVector * (bData.m_acceleration / bData.m_startSpeed);
        }        
        float parseLifeTime = bData.m_lifeTime / 1000f;

        //子弹初始位置
        Vector3 startPos;
        if (targetPos == null)
        {
            startPos = /*heroTrans.position +*/ heroTrans.TransformPoint(bData.m_initPos.y, 0, bData.m_initPos.x);
        }
        else
        {            
            //startPos = targetPos.Value + heroTrans.TransformPoint(bData.m_initPos.y, 0, bData.m_initPos.x);
            startPos = targetPos.Value + heroTrans.TransformPoint(bData.m_initPos.y, 0, bData.m_initPos.x) - heroTrans.position;
        }

        //\test
        //this.CreateFan(bData, startPos, QuaterionY);
        //TraceUtil.Log("parseLifeTime:" + parseLifeTime);
		if(bData.m_mountType == 2)
		{
			if(TargetId != -1)
			{
				TypeID type;
				var targetData = EntityController.Instance.GetEntityModel(TargetId, out type);
				if(targetData != null)
				{
					Vector3 vecToTarget =  targetData.GO.transform.position - startPos;
					
					motionVector = 1000.0f * vecToTarget/bData.m_lifeTime;
				}
			}
		}
        else if (bData.m_mountType == 3)//相对目标型
        {
            TypeID type;
            var targetPlayer = EntityController.Instance.GetEntityModel(TargetId, out type);
            if (targetPlayer != null)
            {                
                startPos = targetPlayer.GO.transform.position + new Vector3(bData.m_initPos.x, 0, -bData.m_initPos.y);
                startPos.y = 0;
            }
        }
        else if (bData.m_mountType == 4)//绝对位置型
        {
            QuaterionY = bData.m_angle + 90;
            float angel = 90 - QuaterionY;   //加上角度偏移            

            rad = angel * Mathf.Deg2Rad;
            motionVector = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * bData.m_startSpeed;
            startPos = new Vector3(bData.m_initPos.x, 0, -bData.m_initPos.y);
        }
        
        
        bulletBehaviour.Fired(startPos, QuaterionY, motionVector, accelerationVector, parseLifeTime);
    }

    /// <summary>
    /// 创建目标范围型的子弹
    /// </summary>
    /// <param name="bulletID"></param>
    /// <param name="bulletPos"></param>
    public void CreateBullet(int bulletID, Int64 EntityUID, Transform heroTrans, Vector3 targetPos, CampType camp, int affectType, int skillId, long targetID)
    {        
        if (bulletID == 0)
        {
            return;
        }        
        BulletData bData = SkillDataManager.Instance.GetBulletData(bulletID);        
		if (bData.m_mountType == 2 && GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER)    //2的挂载类型表示子弹由服务器控制。2 rcj
            return;  
		if(GameManager.Instance.CurrentGameMode == GameMode.MULTI_PLAYER && bData.m_lifeTime != 0 && bData.m_overParam == 2)	//多人模式下，此类型子弹只有后台发送消息时才进行创建
			return;

        if (bData == null)
        {
            ////TraceUtil.Log("找不到子弹配置信息");
            return;
        }
        //前端无论是否生成子弹，子弹序号也需要计算。才会与服务端同步。这个位置不要调整
        BulletManager.Instance.CalendarIndex(EntityUID);
        if (bData.m_bulletResPath == "0")
        {
            ////TraceUtil.Log("未配置子弹实体");
            return;
        }
        GameObject bulletPrefab = MapResManager.Instance.GetMapEffectPrefab(bData.m_bulletResPath);

        GameObject bullet = (GameObject)Instantiate(bulletPrefab);
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        if (bulletBehaviour == null)
        {
            bulletBehaviour = bullet.AddComponent<BulletBehaviour>();
        }
        bulletBehaviour.InitLocalBullet(bulletID, EntityUID,bData, skillId, targetID);
        float lifeTime = bData.m_lifeTime / 1000f;
        //TraceUtil.Log("lifeTime:" + lifeTime);
        

        Vector3 distance = targetPos - heroTrans.position;
        Vector3 heroReferencePos = /*heroTrans.position + */ heroTrans.TransformPoint(bData.m_initPos.y, 0, bData.m_initPos.x);
        Vector3 bulletPos = heroReferencePos + distance;
        
        bullet.transform.position = bulletPos;
		bulletBehaviour.Fired(lifeTime);
        BulletManager.Instance.RegisteBullets(bulletBehaviour);
		
    }

    #region 测试用
    public void BulletPosTest(sMsgButtlePos bulletPos)
    {
        Vector3 pos = Vector3.zero.GetFromServer(bulletPos.posX, bulletPos.posY)+Vector3.up;
        Quaternion quaternion = SquareGO.transform.rotation;
        float rad = Mathf.Atan2(-1 * bulletPos.dirY, bulletPos.dirX);
        float QuaterionY = 90 - rad * Mathf.Rad2Deg;
        GameObject TestGO = (GameObject)UnityEngine.Object.Instantiate(SquareGO, pos, Quaternion.Euler(SquareGO.transform.eulerAngles.x, QuaterionY, SquareGO.transform.eulerAngles.z));

        TestGO.transform.localScale = TestGO.transform.lossyScale * 10f;
    }
    public void TestTransformFun(Vector3 heroPos, Vector3 targetPos, int initPosX, int initPosY)
    {
        GameObject bulletGO = CircleGO;
        Vector3 startPos ;
        float quaterionY;
        CommonTools.TransformPosAndAngle(heroPos, targetPos, initPosX, initPosY, out startPos, out quaterionY);

        GameObject TestGO = (GameObject)UnityEngine.Object.Instantiate(bulletGO, startPos, Quaternion.Euler(bulletGO.transform.eulerAngles.x, quaterionY, bulletGO.transform.eulerAngles.z));

        TestGO.transform.localScale = new Vector3(TestGO.transform.lossyScale.x * 30, TestGO.transform.lossyScale.y * 30, TestGO.transform.lossyScale.z);        
    }
    public void TestTransformFun(Transform heroTrans, int initPosX, int initPosY)
    {
        GameObject bulletGO = CircleGO;
        Vector3 startPos;
        float quaterionY;
        CommonTools.TransformPosAndAngle(heroTrans, initPosX, initPosY, out startPos, out quaterionY);

        GameObject TestGO = (GameObject)UnityEngine.Object.Instantiate(bulletGO, startPos, Quaternion.Euler(bulletGO.transform.eulerAngles.x, quaterionY, bulletGO.transform.eulerAngles.z));

        TestGO.transform.localScale = new Vector3(TestGO.transform.lossyScale.x * 30, TestGO.transform.lossyScale.y * 30, TestGO.transform.lossyScale.z);
    }
    public bool CreateFan(BulletData bulletData, Vector3 pos, float eulerY)
    {
        //\如果不是扇形
        if (bulletData.m_shapeParam1 != 3)
            return false;

        if (isShowFanBullet)
        {
            GameObject fanGO = (GameObject)UnityEngine.Object.Instantiate(FanGO);
            float rad = bulletData.m_shapeParam2 / 10f;
            fanGO.transform.localScale = new Vector3(rad, rad, rad);
            fanGO.transform.position = pos + Vector3.up;
            fanGO.transform.rotation = Quaternion.Euler(0, eulerY, 0);
            var funBehaviour = fanGO.GetComponent<FanController>();
            if (funBehaviour != null)
            {
                funBehaviour.m_Angel = bulletData.m_shapeParam3;
            }
            fanGO.AddComponent<DestroySelf>();
        }
        
        return true;
    }
    #endregion

    public void TestBullet(IEntityDataStruct entityDStruct)
    {
        if (!isShowTestBullet)
        {
            return;
        }        
        SMsgPropCreateEntity_SC_Bullet bulletSC = (SMsgPropCreateEntity_SC_Bullet)entityDStruct;

        #region 子弹
        //Log.Instance.StartLog();
        //Log.Instance.AddLog("66666", "0", bulletSC.PosX.ToString(), bulletSC.PosY.ToString(), bulletSC.DirX.ToString(), bulletSC.DirY.ToString());
        //Log.Instance.AppendLine();
        #endregion
        //TraceUtil.Log("子弹id=" + bulletSC.BaseValue.OBJECT_FIELD_ENTRY_ID);
        //TraceUtil.Log("子弹类型=" + bulletSC.BaseValue.OBJECT_FIELD_TYPE);
        BulletData bData = SkillDataManager.Instance.GetBulletData(bulletSC.BaseValue.OBJECT_FIELD_ENTRY_ID);        

        GameObject bulletGO = CircleGO;

        bool isCircle = true;
        if (bData.m_shapeParam1 == 2)
        {
            //矩形
            bulletGO = SquareGO;
            isCircle = false;
        }
        
        Vector3 localPos = Vector3.zero.GetFromServer(bulletSC.PosX, bulletSC.PosY);
        localPos = new Vector3(localPos.x, bulletGO.transform.position.y, localPos.z);
        Quaternion quaternion = bulletGO.transform.rotation;
        float rad = Mathf.Atan2(-1*bulletSC.DirY, bulletSC.DirX);
        float QuaterionY = 90 - rad * Mathf.Rad2Deg;

        //\本地图片反了
        QuaterionY += 180;

        //\test
        if (this.CreateFan(bData, localPos, QuaterionY))
        {
            return;
        }
         

        if (isShowSquareBullet)
        {

            GameObject TestGO = (GameObject)UnityEngine.Object.Instantiate(bulletGO, localPos, Quaternion.Euler(quaternion.eulerAngles.x, QuaterionY, quaternion.eulerAngles.z));


            if (isCircle)
            {
                TestGO.transform.localScale = new Vector3(TestGO.transform.lossyScale.x * bData.m_shapeParam2, TestGO.transform.lossyScale.y * bData.m_shapeParam2, TestGO.transform.lossyScale.z);
            }
            else
            {
                TestGO.transform.localScale = new Vector3(TestGO.transform.lossyScale.x * bData.m_shapeParam3, TestGO.transform.lossyScale.y * bData.m_shapeParam2, TestGO.transform.lossyScale.z);
            }
            

        }    
    }


    /// <summary>
    /// 网络通知调用创建子弹
    /// </summary>
    /// <param name="entityDStruct"></param>
    private void CreateBullet(IEntityDataStruct entityDStruct)
    {
        SMsgPropCreateEntity_SC_Bullet bulletSC = (SMsgPropCreateEntity_SC_Bullet)entityDStruct;
        BulletData bData = SkillDataManager.Instance.GetBulletData(bulletSC.BaseValue.OBJECT_FIELD_ENTRY_ID);

        //TraceUtil.Log("网络通知调用创建子弹");

        if (bData == null)
        {
            TraceUtil.Log("找不到子弹配置信息");
            return;
        }
        var bulletPrefabStr = bData.m_bulletResPath;
        if (bulletPrefabStr == "0")
        {
            //TraceUtil.Log("未配置子弹实体");
            return;
        }
        GameObject bulletPrefab = MapResManager.Instance.GetMapEffectPrefab(bulletPrefabStr);

        Vector3 localPos = Vector3.zero.GetFromServer(bulletSC.PosX, bulletSC.PosY);
        localPos = new Vector3(localPos.x, bulletPrefab.transform.position.y,localPos.z);

        GameObject bulletGO = (GameObject)UnityEngine.Object.Instantiate(bulletPrefab, localPos, bulletPrefab.transform.rotation);        

        //bulletGO.transform.localScale = new Vector3(bulletGO.transform.lossyScale.x * bData.m_shapeParam2, bulletGO.transform.lossyScale.y * bData.m_shapeParam3, bulletGO.transform.lossyScale.z);        
        
        BulletBehaviour bulletBehaviour = bulletGO.GetComponent<BulletBehaviour>();
        if (bulletBehaviour == null)
        {
            bulletBehaviour = bulletGO.AddComponent<BulletBehaviour>();
        }
        bulletBehaviour.InitBulletFromServer(bulletSC.BaseValue.OBJECT_FIELD_ENTRY_ID, bulletSC.CasterUID, bData, bulletSC.TargetId);

        //子弹运动轨迹(根据角度)
        Vector3 motionVector = Vector3.zero;
        Vector3 accelerationVector = Vector3.zero;

        Vector3 motionNormalize = new Vector3(bulletSC.DirX, 0, -1 * bulletSC.DirY);
        motionNormalize.Normalize();
        var speed = bData.m_startSpeed;
        float parseLifeTime = bData.m_lifeTime / 1000f;

        if (bData.m_mountType == 2)  //必达型子弹，使用服务器速度
        {
            
            Vector3 targetPos = Vector3.zero.GetFromServer(bulletSC.TargetX, bulletSC.TargetY);

            speed = Vector3.Distance(localPos, targetPos) / parseLifeTime;// bulletSC.Speed / 10;  //厘米转分米
        }
        motionVector = motionNormalize * speed;
        float rad = Mathf.Atan2(-1 * bulletSC.DirY, bulletSC.DirX);                
        float QuaterionY = 90 - rad * Mathf.Rad2Deg;

        
        if (bData.m_acceleration != 0)
        {
            accelerationVector = motionVector * (bData.m_acceleration / bData.m_startSpeed);
        }
        

        Quaternion initQuaternion = bulletBehaviour.transform.rotation;
        bulletBehaviour.transform.rotation = Quaternion.Euler(initQuaternion.eulerAngles.x, QuaterionY, initQuaternion.eulerAngles.z);
        //Vector3 bulletReferencePos = /*bulletBehaviour.transform.position+*/ .transform.TransformPoint(bData.m_initPos.y, 0, bData.m_initPos.x);
        bulletBehaviour.Fired(localPos, QuaterionY, motionVector, accelerationVector, parseLifeTime);

        EntityModel bulletModel = new EntityModel();
        bulletModel.GO = bulletGO;
        bulletModel.Behaviour = bulletBehaviour;
        bulletModel.EntityDataStruct = entityDStruct;
        EntityController.Instance.RegisteEntity(bulletSC.GUID, bulletModel);

        #region 打印测试
        //Log.Instance.AddOtherLog(

        #endregion
    }

    #region 预留方法 传入角度参数创建子弹
	
	/*
    /// <summary>
    /// 施放技能调用创建子弹(被告知角度的情况)
    /// </summary>
    /// <param name="bulletID"></param>
    /// <param name="EntityUID"></param>
    /// <param name="heroTrans"></param>
    /// <param name="angle"></param>
    public void CreateBullet(int bulletID, Int64 EntityUID, Transform heroTrans, int angle)
    {
        BulletData bData = SkillDataManager.Instance.GetBulletData(bulletID);
        if (bData == null)
        {
            //TraceUtil.Log("找不到子弹配置信息");
            return;
        }
        if (bData.m_resourcePrefab == null)
        {
            //TraceUtil.Log("未配置子弹实体");
            return;
        }
        GameObject bullet = (GameObject)Instantiate(bData.m_resourcePrefab);
        BulletBehaviour bulletBehaviour = bullet.GetComponent<BulletBehaviour>();
        if (bulletBehaviour == null)
        {
            bulletBehaviour = bullet.AddComponent<BulletBehaviour>();
        }
        bulletBehaviour.InitBullet(bulletID, EntityUID,bData);
        BulletManager.Instance.RegisteBullets(bulletBehaviour);

        //子弹运动轨迹(根据角度)
        Vector3 motionVector = Vector3.zero;
        Vector3 accelerationVector = Vector3.zero;

        float rad = angle * Mathf.Deg2Rad;
        motionVector = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)) * bData.m_startSpeed;
        float QuaterionY = 90 - rad * Mathf.Rad2Deg;

        if (bData.m_acceleration != 0)
        {
            accelerationVector = motionVector * (bData.m_acceleration / bData.m_startSpeed);
        }
        float parseLifeTime = bData.m_lifeTime / 1000f;
        bulletBehaviour.Fired(heroTrans.position, QuaterionY, motionVector, accelerationVector, parseLifeTime);
    }
    */
    #endregion
}
