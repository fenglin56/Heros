using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum SkillFireSelectState
{
	STATE_WAIT_FOR_SELECT_TARGET = 0,
	STATE_SELECT_TARGET,
	STATE_NONE,
}


/// <summary>
///  主动技能选择目标
/// </summary>
public class InitiativeSkillSelectedState : PlayerState
{
    private SkillBase m_skillBase;
	private SkillConfigData m_skillData;
	private PlayerBehaviour m_playerBehaviour;
	private int m_skillDirType;
	private bool m_fireSelected;
	private Vector3 m_touchPoint;
	private bool m_canFireRangeSkill;
	
	private SkillFireSelectState m_state = SkillFireSelectState.STATE_NONE;
    public InitiativeSkillSelectedState()
    {
        m_stateID = StateID.PlayerInitialtiveSkillSelect;
		
		
    }
    public override void Reason()
    {
        if (m_PlayerBehaviour.IsCopy) return;
        if (IsStateReady)
        {
            if (!IsSkillBeBreaked)
            {
                switch (m_skillDirType)
                {
                    case 1:        //方向
                        {
                            if (m_fireSelected)
                            {
                                //m_playerBehaviour.ChangeForward(m_touchPoint);
                                OnChangeTransition(Transition.PlayerFireInitiativeSkill);
                            }
                        }
                        break;
                    case 2:       //圆形范围
                        {
                            if (m_fireSelected)
                            {
                                if (!m_canFireRangeSkill)
                                {
                                    float distance = Vector3.Distance(m_playerBehaviour.ThisTransform.position, m_touchPoint);
                                    if (distance <= m_skillData.m_launchRange[0])
                                    {
                                        //m_playerBehaviour.ChangeForward(m_touchPoint);
                                        m_canFireRangeSkill = true;                                        
                                        var fireState = m_playerBehaviour.FSMSystem.FindState(global::StateID.PlayerInitiativeSkill) as PlayerInitiativeSkillFireState;
                                        fireState.m_touchPoint = m_touchPoint;
                                        OnChangeTransition(Transition.PlayerFireInitiativeSkill);
                                    }

                                }
                            }
                        }
                        break;
                    case 3:       //扇形范围
                        {
                            if (m_fireSelected)
                            {
                                m_playerBehaviour.ChangeForward(m_touchPoint);
                                OnChangeTransition(Transition.PlayerFireInitiativeSkill);
                            }
                        }
                        break;

                    default:
                        break;

                }
            }
            else
            {
                //取消技能选择
                this.m_PlayerBehaviour.LeaveInitiativeSkillSelectedState();
                OnChangeTransition(Transition.PlayerToIdle);
            }
        }
    }

    public override void Act()
    {
        if (IsStateReady)
        {
            UpdateState();
			if(!m_canFireRangeSkill && m_fireSelected)
			{
				float dt = Time.deltaTime;
				if(m_skillDirType == 2)
				{
					m_playerBehaviour.ChangeForward(m_touchPoint);
				}
				m_roleAnimationComponent.CrossFade("Walk02");
                var motion = this.m_playerBehaviour.ThisTransform.TransformDirection(Vector3.forward) * this.m_PlayerBehaviour.WalkSpeed * Time.deltaTime;
                //if (!SceneDataManager.Instance.IsPositionInBlock(this.m_PlayerBehaviour.ThisTransform.position + motion))
                //{
                //    m_PlayerBehaviour.HeroCharactorController.Move(motion);
                //}  
                this.m_playerBehaviour.MoveTo(motion);
				//m_playerBehaviour.HeroCharactorController.Move(this.m_playerBehaviour.ThisTransform.TransformDirection(Vector3.forward) * this.m_PlayerBehaviour.WalkSpeed*Time.deltaTime);
			}
            
        }
    }   
    public void ChangeState(Vector3 touchPosition, TouchPhase touchPhase)
    {
        switch (m_state)
        {
            case SkillFireSelectState.STATE_WAIT_FOR_SELECT_TARGET:
                if (touchPhase==TouchPhase.Began)
                {
                    m_state = SkillFireSelectState.STATE_SELECT_TARGET;
                    if (m_skillDirType == 2)
                    {
                        ChangeCirclePosByExactPos(touchPosition);
                    }
                }
                break;
            case SkillFireSelectState.STATE_SELECT_TARGET:
                if (touchPhase == TouchPhase.Ended)
                {

                        m_state = SkillFireSelectState.STATE_NONE;
                        Vector3 touchPos = touchPosition;
                        m_fireSelected = true;
                        m_touchPoint = touchPos;
                        m_playerBehaviour.ChangeForward(m_touchPoint);
                        //m_playerBehaviour.SkillFirePos = point;
                        if (m_skillDirType == 2)
                        {

                            m_playerBehaviour.SkillFirePos = ChangeCirclePosByExactPos(m_touchPoint);
                            m_touchPoint = m_playerBehaviour.SkillFirePos;
                            HideAllEffect();
                        }

                        break;
                    }
                    else
                    {

                        Vector3 movePos = touchPosition;
                        if (m_skillDirType == 1)
                        {
                            RotatePlayerByExactPos(movePos);
                        }
                        else if (m_skillDirType == 2)
                        {
                            ChangeCirclePosByExactPos(movePos);
                        }

                    }
                break;
            default:
                break;

        }
    }
    private void UpdateState()
    {
        switch (m_state)
        {
            case SkillFireSelectState.STATE_WAIT_FOR_SELECT_TARGET:
                {
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
                    if (Input.GetMouseButtonDown(0))
#else
            if(Input.touches.Length > 0 
                    && Input.touches[0].phase == TouchPhase.Began)
						
#endif
                    {
                        m_state = SkillFireSelectState.STATE_SELECT_TARGET;
                        if (m_skillDirType == 2)
                        {
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
                            Vector3 touchPosition = Input.mousePosition;
#else
                    Vector3 touchPosition = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
#endif
                            var uiRay = UICamera.currentCamera.ScreenPointToRay(touchPosition);
                            RaycastHit uiRaycastHit;
                            if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
                            {
                                return;
                            }
                            ChangeCirclePosByTouchPos(touchPosition);
                        }
                    }
                }
                break;
            case SkillFireSelectState.STATE_SELECT_TARGET:
                {
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
                    if (Input.GetMouseButtonUp(0))
#else
            if(Input.touches.Length > 0 
                    && Input.touches[0].phase == TouchPhase.Ended)	
#endif
                    {

#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
                        Vector3 touchPosition = Input.mousePosition;
#else
                    Vector3 touchPosition = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
#endif
                        var uiRay = UICamera.currentCamera.ScreenPointToRay(touchPosition);
                        RaycastHit uiRaycastHit;
                        if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
                        {
                            return;
                        }
                        m_state = SkillFireSelectState.STATE_NONE;
                        Vector3 touchPos = touchPosition;
                        m_fireSelected = true;
                        m_touchPoint = touchPos;

                        //m_playerBehaviour.SkillFirePos = point;
                        if (m_skillDirType == 2)
                        {

                            m_playerBehaviour.SkillFirePos = ChangeCirclePosByTouchPos(m_touchPoint);
                            m_touchPoint = m_playerBehaviour.SkillFirePos;
                            HideAllEffect();
                        }
						else if(m_skillDirType == 1)
						{
							RotatePlayerByTouchPos(m_touchPoint);
						}
	
                        break;
                    }
                    else
                    {

						bool touchExit = false;

#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
						touchExit = true;
#else
						if(Input.touches.Length > 0 
				   && Input.touches[0].phase == TouchPhase.Moved)	
						{
							touchExit = true;
						}
#endif


					if(touchExit)
					{
#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
                        Vector3 touchPosition = Input.mousePosition;
#else
                    Vector3 touchPosition = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
#endif

                        Vector3 movePos = touchPosition;
                        var uiRay = UICamera.currentCamera.ScreenPointToRay(touchPosition);
                        RaycastHit uiRaycastHit;
                        if (Physics.Raycast(uiRay, out uiRaycastHit, 100))
                        {
                            return;
                        }
                        if (m_skillDirType == 1)
                        {
                            RotatePlayerByTouchPos(movePos);
                        }
                        else if (m_skillDirType == 2)
                        {
                            ChangeCirclePosByTouchPos(movePos);
                        }

					}

                    }
                }
                break;
            default:
                break;

        }

    }
    void RotatePlayerByTouchPos(Vector3 touchPos)
    {
        /*
        Vector3 playerPosInScreen = Camera.mainCamera.WorldToViewportPoint(m_playerBehaviour.ThisTransform.position);
        Vector3 dir = touchPos - playerPosInScreen;
        Vector3 transDir = Camera.mainCamera.transform.TransformDirection(dir);
        transDir.y = 0;
        m_playerBehaviour.ChangeForward(transDir + m_playerBehaviour.ThisTransform.position);
        */
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
        {
            Vector3 dir = hit.point - m_playerBehaviour.ThisTransform.position;
            dir.y = 0;
            m_playerBehaviour.ChangeForward(dir + m_playerBehaviour.ThisTransform.position);
        }
    }
    Vector3 ChangeCirclePosByTouchPos(Vector3 touchPos)
    {
        Vector3 result = Vector3.zero;
        Ray ray = Camera.main.ScreenPointToRay(touchPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << 8))
        {
            Vector3 pos = hit.point;
            pos.y = 0.1f;
            m_playerBehaviour.SkillSelectEffectController.SetCirclePos(pos);
            result = hit.point;
        }
        return result;
    }
#region 旧的点击处理
//    private Vector3? GetPlayerTouchPoint(out TouchPhase touchPhase)
//    {
//        Vector3? inputPoint = null;
//        touchPhase = TouchPhase.Canceled;
//        switch (m_state)
//        {
//            case SkillFireSelectState.STATE_WAIT_FOR_SELECT_TARGET:
//                {
//#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
//                    if (Input.GetMouseButtonDown(0))
//#else
//            if(Input.touches.Length > 0 
//                    && Input.touches[0].phase == TouchPhase.Began)
						
//#endif
//                    {
//                        m_state = SkillFireSelectState.STATE_SELECT_TARGET;
//                        if (m_skillDirType == 2)
//                        {
//#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
//                            inputPoint = Input.mousePosition;
//#else
//                    inputPoint = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
//#endif
//                        }
//                        touchPhase = TouchPhase.Began;
//                    }
//                }
//                break;
//            case SkillFireSelectState.STATE_SELECT_TARGET:
//                {
//#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
//                    if (Input.GetMouseButtonUp(0))
//#else
//            if(Input.touches.Length > 0 
//                    && Input.touches[0].phase == TouchPhase.Ended)	
//#endif
//                    {

//#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
//                        inputPoint = Input.mousePosition;
//#else
//                    inputPoint = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
//#endif
//                        touchPhase = TouchPhase.Ended;
//                        break;
//                    }
//                    else
//                    {
//#if (!UNITY_IPHONE && !UNITY_ANDROID) || UNITY_EDITOR
//                        inputPoint = Input.mousePosition;
//#else
//                    inputPoint = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, 0);
//#endif
//                        touchPhase = TouchPhase.Began;
//                    }
//                }
//                break;
//            default:
//                break;

//        }

//        return inputPoint;
//    }

#endregion


    void RotatePlayerByExactPos(Vector3 pos)
    {
        Vector3 dir = pos - m_playerBehaviour.ThisTransform.position;
        dir.y = 0;
        m_playerBehaviour.ChangeForward(dir + m_playerBehaviour.ThisTransform.position);
    }

    Vector3 ChangeCirclePosByExactPos(Vector3 pos)
    {
        m_playerBehaviour.SkillSelectEffectController.SetCirclePos(pos);
        return pos;
    }
    public override void DoBeforeEntering()
    {
		m_state = SkillFireSelectState.STATE_WAIT_FOR_SELECT_TARGET;
		m_canFireRangeSkill = false;
		m_fireSelected = false;
        IsSkillBeBreaked = false;
		m_playerBehaviour = (PlayerBehaviour)(m_roleBehaviour);
        m_skillBase = m_playerBehaviour.NextSkillBase;
        if (m_skillBase != null)
        {
            m_skillData = SkillDataManager.Instance.GetSkillConfigData(m_skillBase.SkillId);
            m_skillDirType = m_skillData.m_directionParam;

            HideAllEffect();

            if(GameManager.Instance.m_gameSettings.DoubleClickSkill)
            {
                switch (m_skillDirType)
                {
                    case 1:
                    {
                        ShowDirectionEffect();
                    }
                        break;
                    case 2:
                    {
                        ShowRangeEffect();
                    }
                        break;
                    case 3:
                    {
                        ShowFanEffect();
                    }
                        break;
                    default:
                        break;
                        
                }

            }


            base.DoBeforeEntering();
            this.m_playerBehaviour.ExecuteInitiativeSkillSelectedState = true;
        }
    }
    public override void DoBeforeLeaving()
    {
        if (this.m_playerBehaviour != null)
        {
            this.m_playerBehaviour.ExecuteInitiativeSkillSelectedState = false;
            HideAllEffect();
        }
		m_canFireRangeSkill = false;
		m_fireSelected = false;		
        base.DoBeforeLeaving();        
    }
	
	//显示方向选择效果
	private void ShowDirectionEffect()
	{
		if(null != m_playerBehaviour.SkillSelectEffectController)
		{
			m_playerBehaviour.SkillSelectEffectController.ShowArrow();
		}
	}
	
	//显示扇形效果
	private void ShowFanEffect()
	{
		if(null != m_playerBehaviour.SkillSelectEffectController)
		{
			m_playerBehaviour.SkillSelectEffectController.ShowFan(m_skillData.m_launchRange[0]/10, m_skillData.m_launchRange[1]);
		}
	}
	
	//范围效果
	private void ShowRangeEffect()
	{
		if(null != m_playerBehaviour.SkillSelectEffectController)
		{
			m_playerBehaviour.SkillSelectEffectController.ShowCircle(1, m_PlayerBehaviour.ThisTransform);
		}
	}
	
	//隐藏所有特效
	private void HideAllEffect()
	{
		if(null != m_playerBehaviour.SkillSelectEffectController)
		{
			m_playerBehaviour.SkillSelectEffectController.HideAll();
		}
	}   
	public void OnTouch(Vector3 point)
	{
		//m_touchPoint = point;
		//m_fireSelected = true;
       
        //m_playerBehaviour.SkillFirePos = point;
	}
}
