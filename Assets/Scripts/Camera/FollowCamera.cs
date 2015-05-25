using UnityEngine;
using System.Collections;

/// <summary>
/// BattleUI/TownUI Scene 锟斤拷锟斤拷锟斤拷锟?
/// </summary>
public class FollowCamera : MonoBehaviour {

    public Transform m_target;
    public Transform m_oldTarget;

    //public float m_distance = 30.0f;
    //public float m_height = 30.0f;
    //public float m_heightDamping = 2.0f;
    //public float m_rotationDamping = 3.0f;
	
	private float m_sceneWidth;
	private float m_sceneHeight;
	
	public float m_distanceCacheX = 170;
	public float m_distanceCacheY = 170;
	
	//for camera shake
	//闇囧姩娆℃暟
	public int m_shakeTimes = 2;
	//闇囧姩琛板噺
	public float m_shakeDeclineParam = 0.99f;
	//闇囧姩鍒濋€熷害
	public float m_shakeInitSpeed = -2000.0f;
	//鐨瓔寮瑰姏绯绘暟
	public float m_kParam = 1500.0f;
	
	
	public int m_camStayTotalFrame = 5;
	private int m_stayFrame;
	private bool m_isStay;
	private bool m_stayPassed;
	private bool m_isShaking = false;
	
	//闀滃ご鍒濆浣嶇疆
    private float x = 200, y = 300, z = -200;
    private string xStr="200", yStr="300", zStr="-200";
	public Vector3 m_initDistanceFromPlayer = new Vector3(220, 350, -220);
	

	private Vector3 m_normalInitDistanceFromPlayer;
	
	//
	private bool m_isCameraShake = false;
	private int m_currentDoubleShakeTime = 0;
	private float m_shakeCurrentSpeed;
	private Vector3 m_currentDistanceFromPlayer;
	private Vector3 m_nextDistanceFromPlayer;
	private Vector3 m_initShakeDistanceFromPlayer;
	
	
	private float m_shakeNextSpeed;
	
	
	//
	private Vector3 m_smoothDistanceFromTarget;
	private Vector3 m_smoothMoveFrom;
	private Vector3 m_smoothMoveTo;
	
	private Vector3 m_distanceFromTarget;
	
	public float m_smoothMoveTime = 1.0f;

    private Animation m_shakeAnimation;
    public bool IsInSmoothMove{get;private set;}
    public bool m_posTest;

    //private bool m_IsNotNeedProtect = false;    //涓嶉渶瑕佹憚鍍忓ご淇濇姢
    private bool m_isMovingToFixedPos = false;
    private Vector3 m_fixedPos = Vector3.zero;
    private float m_fixedTime = 0;


	void Awake()
	{
		
        int curGameViewLevel = GameManager.Instance.m_gameSettings.GameViewLevel;
        m_initDistanceFromPlayer = CommonDefineManager.Instance.GetCameraDistanceFromPlayer();
        m_distanceCacheX = CommonDefineManager.Instance.CommonDefine.CameraBarrierDistanceList[curGameViewLevel];
        m_distanceCacheY = m_distanceCacheX;

        m_shakeAnimation = null;
        m_shakeAnimation = gameObject.GetComponent<Animation>();
	}

    public void SetInitDistanceFromPlayer(Transform target, Vector3 distance, bool isInSmoothMove)
    {

        IsInSmoothMove = isInSmoothMove;
        m_initDistanceFromPlayer = distance;
        int curGameViewLevel = GameManager.Instance.m_gameSettings.GameViewLevel;
        m_distanceCacheX = CommonDefineManager.Instance.CommonDefine.CameraBarrierDistanceList[curGameViewLevel];
        m_distanceCacheY = m_distanceCacheX;
        SetTarget(target);
    }

	[ContextMenu( "Shake Camera" )]
	public void ShakeCamera(int shakeTime, float shakeAttenuation, float shakeInitSpeed, float shakeElasticity)
	{
		if(m_isShaking)
		{
			return;	
		}
		m_shakeTimes = shakeTime;
		m_shakeDeclineParam = shakeAttenuation;
		m_shakeInitSpeed = shakeInitSpeed;
		m_kParam = shakeElasticity;
		
		
		
		m_isCameraShake = true;
		m_shakeCurrentSpeed = m_shakeInitSpeed;
		m_shakeNextSpeed = m_shakeCurrentSpeed;
		m_currentDoubleShakeTime = 0;
		m_currentDistanceFromPlayer = m_initShakeDistanceFromPlayer;
		m_normalInitDistanceFromPlayer = m_initShakeDistanceFromPlayer.normalized;
		
		m_stayFrame = 0;
		m_stayPassed = false;
		m_isStay = false;
		m_isShaking = true;
		
	}

    public void ShakeCamera(string shakeAniName)
    {
        if(null != m_shakeAnimation)
        {
            m_shakeAnimation.Play(shakeAniName);
            if (m_isMovingToFixedPos && gameObject.activeInHierarchy)
            {
                StartCoroutine("LateStopShake", m_shakeAnimation[shakeAniName].length);
            }            
        }
    }

    IEnumerator LateStopShake(float time)
    {
        yield return new WaitForSeconds(time);
        StartCoroutine(MoveToFixedPos(m_fixedPos, m_fixedTime));
    }

	public void StopShake()
	{
		m_isCameraShake = false;	
		m_currentDoubleShakeTime = 0;
		m_isShaking = false;        
	}
	
	void FindTargetPoint(Transform target)
	{
		
		target.RecursiveFindObject("CameraTarget", out m_target);
		if(null == m_target )
		{
			m_target = target;	
		}
	}
	
	public void SetTarget(Transform target)
	{
		//m_target = target;	
		FindTargetPoint(target);
		
		//init camera pos and rotation
		m_distanceFromTarget = m_initDistanceFromPlayer;
        Vector3 pos = m_target.position;
        pos += m_initDistanceFromPlayer;

        transform.position = pos;
        //transform.LookAt(m_target,Vector3.up);
        transform.LookAt(m_target);
		
		//setup the scene parameter
		int SceneConfigId = (int)GameManager.Instance.GetCurSceneMapID;
		SceneConfigData sData = EctypeConfigManager.Instance.SceneConfigList[SceneConfigId];
		m_sceneWidth = sData._mapWidth;
		m_sceneHeight = sData._mapHeight;
		
		//init Shake Params
		m_normalInitDistanceFromPlayer = m_initDistanceFromPlayer.normalized;
	}

    /// <summary>
    /// 璁剧疆鍥哄畾闀滃ご
    /// </summary>
    public void SetFixed(Vector3 fixedPos, float time)
    {
        if (m_target != null)
        {
            m_oldTarget = m_target;
            m_target = null;
        }
        m_fixedPos = fixedPos;
        m_fixedTime = time;

        StopAllCoroutines();
        StartCoroutine(MoveToFixedPos(fixedPos, time));
    }
    public void ResetSetTarget()
    {
        if (m_oldTarget != null)
        {
            SetTarget(m_oldTarget);
        }
    }
    IEnumerator MoveToFixedPos(Vector3 endPos, float time)
    {
        m_isMovingToFixedPos = true;
        float i = 0;
        float rate = 1.0f / time;
        Vector3 startPos = transform.position;
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;            
            transform.position = Vector3.Lerp(startPos, endPos, i);
            //record
            m_fixedTime = (1 - i) * time;            
            yield return null;
        }
        m_isMovingToFixedPos = false;
        IsInSmoothMove = false;
    }

    private Vector3 OldPos;

    public void BeginMoveToPosAndGoBack(Vector3 endPos, float moveTime, float stayTime, float backTime, bool blockPlayerToIdle)
    {
        EntityModel heroEntityModel = PlayerManager.Instance.FindHeroEntityModel();
        if(heroEntityModel != null && heroEntityModel.Behaviour != null)
        {
            PlayerBehaviour pb = (PlayerBehaviour)heroEntityModel.Behaviour;
            if(pb.FSMSystem.CurrentStateID == StateID.PlayerInitiativeSkill
               && pb.SelectedSkillBase.SkillData.m_IsSirenSkill)
            {
                return;
            }
        }
		if(blockPlayerToIdle)
		{
			UI.Battle.BattleUIManager.Instance.ShowStoryCover(true);
		}
		//return;
        if(m_target != null)
        {
            m_oldTarget = m_target;
            m_target = null;           
        }
        OldPos = transform.position;
        StopAllCoroutines();
        StartCoroutine(MoveToPosAndGoBack(endPos, moveTime, stayTime, backTime, blockPlayerToIdle));
    }


    IEnumerator MoveToPosAndGoBack(Vector3 endPos, float moveTime, float stayTime, float backTime, bool blockPlayerToIdle)
    {
        IsInSmoothMove = true;
        BattleManager.Instance.BlockPlayerToIdle = blockPlayerToIdle;

        float firstMoveTime = moveTime;
        if(firstMoveTime < 0.0001f)
        {
            firstMoveTime = 0.0001f;
        }

        //first move camera to endPos
        float i = 0;
        float rate = 1.0f / firstMoveTime;
        Vector3 startPos = transform.position;
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;            
            transform.position = Vector3.Lerp(startPos, endPos, i);
            //record
            //m_fixedTime = (1 - i) * firstMoveTime;

            yield return null;
        }
        transform.position = endPos;

        if(stayTime < 0.0001f)
        {
            IsInSmoothMove = false;
            BattleManager.Instance.BlockPlayerToIdle = false;
			UI.Battle.BattleUIManager.Instance.ShowStoryCover(false);
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(stayTime);

            i = 0;
            rate = 1.0f / backTime;
            m_target = m_oldTarget;
            while (i < 1.0)
            {
                i += Time.deltaTime * rate;            
                transform.position = Vector3.Lerp(endPos, OldPos, i);
                //record
                //m_fixedTime = (1 - i) * backTime;
                //transform.LookAt(m_target);
                yield return null;
            }
            transform.LookAt(m_target);
            transform.position = startPos;
            SetTarget(m_target);
            BattleManager.Instance.BlockPlayerToIdle = false;
			UI.Battle.BattleUIManager.Instance.ShowStoryCover(false);
        }
        IsInSmoothMove = false;
        


    }


	public void SmoothMoveTargetOriginal(Transform target)
	{
		SetSmoothMoveTarget(target, m_initDistanceFromPlayer);
		
	}
	
	public void SetSmoothMoveTarget(Transform target, Vector3 newDisTance)
	{	
		//m_target = target;
		FindTargetPoint(target);
		m_distanceFromTarget = m_smoothDistanceFromTarget = newDisTance;
		m_smoothMoveFrom = this.transform.position; 
		m_smoothMoveTo = target.transform.position + m_smoothDistanceFromTarget;
		StopCoroutine("SmoothMove");
		StartCoroutine("SmoothMove");
	}
    public void SetSmoothMoveTarget(Vector3 targetPos, Vector3 newDisTance)
    {
        m_target = null;
        m_distanceFromTarget = m_smoothDistanceFromTarget = newDisTance;
        m_smoothMoveFrom = this.transform.position;
        m_smoothMoveTo = targetPos + m_smoothDistanceFromTarget;
        StartCoroutine(SmoothMoveToTargetPos(targetPos));
    }
    IEnumerator SmoothMoveToTargetPos(Vector3 targetPos)
    {
        float i = 0;
        IsInSmoothMove = true;
        float rate = 1.0f / m_smoothMoveTime;
        while (i < 1f)
        {
            i += Time.deltaTime * rate;
            m_smoothMoveTo = targetPos + m_smoothDistanceFromTarget;
            transform.position = Vector3.Lerp(m_smoothMoveFrom, m_smoothMoveTo, i);
            transform.LookAt(targetPos);
            yield return null;
        }

        transform.position = m_smoothMoveTo;
        transform.LookAt(m_target);
        IsInSmoothMove = false;

    }
	IEnumerator SmoothMove()
	{

		float i = 0;
		IsInSmoothMove = true;
		float rate = 1.0f / m_smoothMoveTime;
		while (i < 1f)
        {
            i += Time.deltaTime * rate;
			m_smoothMoveTo = m_target.transform.position + m_smoothDistanceFromTarget;
            transform.position = Vector3.Lerp(m_smoothMoveFrom, m_smoothMoveTo, i);
			transform.LookAt(m_target);
            yield return null;
        }
		
		transform.position = m_smoothMoveTo;
		transform.LookAt(m_target);
		IsInSmoothMove = false;
		
	}

    void LateUpdate()
    {
		/*
		if(Input.GetKeyDown(KeyCode.S))
		{
			//ShakeCamera();	
		}
		*/
        if (m_posTest)
        {
            SetTarget(m_target);
            return;
        }

		if(IsInSmoothMove)
		{
			return;	
		}
		
		
		//float dt = Time.deltaTime;
        if (!m_target)
		{
            return;
		}
        
		if(!m_isCameraShake)
		{
		  	//float currentHeight = m_target.position.y + m_initDistanceFromPlayer.y;
			Vector3 targetPos =new Vector3(m_target.position.x,m_target.localPosition.y,m_target.position.z);
			if(targetPos.x > m_sceneWidth - m_distanceCacheX)
			{
				targetPos.x = m_sceneWidth - m_distanceCacheX;
			}
			else if(targetPos.x < m_distanceCacheX)
			{
				targetPos.x = m_distanceCacheX;
			}
			
			if(targetPos.z > -m_distanceCacheY)
			{
				targetPos.z = -m_distanceCacheY;	
			}
			else if(targetPos.z < -(m_sceneHeight - m_distanceCacheY))
			{
				targetPos.z = -(m_sceneHeight - m_distanceCacheY);
			}
			
		    targetPos += m_distanceFromTarget;
			
		    transform.position = targetPos;
			m_initShakeDistanceFromPlayer = targetPos - m_target.position;
		}
		else
		{
			if(m_isStay)
			{
				m_stayFrame += 1;
				if(m_stayFrame > m_camStayTotalFrame)
				{
					m_stayPassed = true;
					m_isStay = false;
				}
				return;
			}
			float fixedDt = 0.0167f;
			
			float currentDisFromOrigin = m_initShakeDistanceFromPlayer.magnitude - m_currentDistanceFromPlayer.magnitude;
			
			m_shakeNextSpeed = m_shakeCurrentSpeed*m_shakeDeclineParam + 
				m_kParam*(currentDisFromOrigin)*fixedDt;
			
			m_nextDistanceFromPlayer = m_currentDistanceFromPlayer + m_shakeNextSpeed*m_normalInitDistanceFromPlayer*fixedDt;
			float nextDisFromOrigin = m_initShakeDistanceFromPlayer.magnitude - m_nextDistanceFromPlayer.magnitude;
			
			Vector3 TargetPos = m_target.position;
			TargetPos += m_currentDistanceFromPlayer;
			transform.position = TargetPos;
			
			if(nextDisFromOrigin * currentDisFromOrigin <= 0 )
			{
				if(!m_stayPassed)
				{
					m_isStay = true;	
				}
				m_currentDoubleShakeTime += 1;	
			}
			m_currentDistanceFromPlayer = m_nextDistanceFromPlayer;
			m_shakeCurrentSpeed = m_shakeNextSpeed;
			
			if(m_currentDoubleShakeTime/2 >= m_shakeTimes)
			{
				StopShake();	
			}
		}
    }

    void OnGUI()
    {

        if (GameManager.Instance.IsShowCameraAdjust)
        {
            Rect posL1 = new Rect(100, 300, 100, 40);
            Rect posL2 = new Rect(100, 360, 100, 40);
            Rect posL3 = new Rect(100, 420, 100, 40);
            //Rect posS1 = new Rect(200, 300, 500, 10);
            //Rect posS2 = new Rect(200, 360, 500, 10);
            //Rect posS3 = new Rect(200, 420, 500, 10);

            if (GUI.Button(new Rect(200, 250, 100, 50), "Close"))
            {
                GameManager.Instance.IsShowCameraAdjust = false;
            }
            if (GUI.Button(new Rect(100, 250, 100, 50), "Reset"))
            {
                x = 200;
                y = 300;
                z = -200;

                xStr = x.ToString();
                yStr = y.ToString();
                zStr = z.ToString();
            }

            xStr=GUI.TextField(posL1, xStr);
            yStr=GUI.TextField(posL2, yStr);
            zStr=GUI.TextField(posL3, zStr);

            float.TryParse(xStr, out x);
            float.TryParse(yStr, out y);
            float.TryParse(zStr, out z);

            //x = GUI.HorizontalSlider(posS1, x, 0, 5000);
            //y = GUI.HorizontalSlider(posS2, y, 0, 5000);
            //z = GUI.HorizontalSlider(posS3, z, -5000, 0);



            m_distanceFromTarget = new Vector3(x, y, z);
        }
    }
}
