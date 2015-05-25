using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StroyNpcBehaviour :View {

    private List<StroyActionConfigData> m_actionData = new List<StroyActionConfigData>();
    private bool m_isPlayAnim = false;
    private bool m_isMove = false;
    private float m_elapseTime;
	private float m_moveElapseTime;
    private float m_moveTime;
    private int animIndex = 0;
	private int moveIndex = 0;
    private float m_totalActionTime = 0;
    private int m_npcID;

	// Use this for initialization
    void Start()
    {
        this.RegisterEventHandler();
    }

    // Update is called once per frame
    void Update()
    {
        float timeDelta = Time.deltaTime;

		if(m_isPlayAnim)
		{
            if (m_actionData.Count > 0)
            {
				string animName = m_actionData[animIndex]._ActionName;
                if (m_actionData[animIndex]._ActionType == 3)
                {
                    StroyLineDataManager.Instance.DelNpcGo(m_npcID);
                }

                if (animation.GetClip(animName) == null)
                {
                    TraceUtil.Log("当前角色未挂载" + animName + "的动画，请检查！");
                    //return;
                }
                else
                {

                    if (m_elapseTime < m_actionData[animIndex]._Duration/1000)
    				{
    					m_elapseTime += timeDelta;
                        //animation.CrossFade(animName, 0.3f);
                        animation.Play(animName);
                        if (m_actionData.Count == 1)
                        {
                            animation.wrapMode = WrapMode.Loop;
                        }
    				}
    				else
    				{
                        m_actionData.RemoveAt(animIndex);
                        m_elapseTime = 0f;
    				}
                }
			}
			else
			{
				m_isPlayAnim = false;
			}
		}
		
		if(m_isMove)
		{
			if(m_actionData.Count > 0)
			{
				m_moveElapseTime += timeDelta;
				if(m_moveElapseTime <= m_actionData[moveIndex]._Duration/1000)
				{
					//jamfing TODO 这里有问题，1)最终速度只能再取一个变量来存，而不能用_Speed,否则改表了；2)因为为减速运动，Mathf.FloorToInt(-0.1)值为-1，有误差，应该直接去除Mathf.FloorToInt,直接用float;//
					m_moveTime = m_actionData[moveIndex]._Speed * timeDelta + m_actionData[moveIndex]._Acceleration * Mathf.Pow(timeDelta, 2) * 0.5f;
                   	m_actionData[moveIndex]._Speed += Mathf.FloorToInt(m_actionData[moveIndex]._Acceleration * timeDelta);
                    Vector3 direction = Quaternion.Euler(0, (float)m_actionData[moveIndex]._StartAngle, 0) * Vector3.forward;
                    transform.rotation = Quaternion.Euler(0, m_actionData[moveIndex]._ModelAngle, 0);
					if(transform.GetComponent<CharacterController>() != null)
                    	transform.GetComponent<CharacterController>().Move(direction * m_moveTime);
					else
						gameObject.AddComponent<CharacterController>().Move(direction * m_moveTime);
				}
				else
				{
					//moveIndex += 1;
                    m_moveElapseTime = 0f;
				}
			}
			else
			{
				m_isMove = false;
			}
		}
    }

    public void SetActionData(StroyAction actionData)
    {
        m_actionData.Clear();
        m_isPlayAnim = false;
        m_isMove = false;
        moveIndex = 0;
        animIndex = 0;
        m_elapseTime = 0;
        m_moveTime = 0;
        m_totalActionTime = 0;
        m_npcID = actionData.NpcID;

        foreach(var item in actionData.ActionList)
        {
			if(item._ActionType != 1)
            {
                m_actionData.Add(item);
                m_totalActionTime += item._Duration / 1000;
            }
        }

        if (m_actionData.Count > 0)
        {
            m_isPlayAnim = true;
            m_isMove = true;
            StroyLineManager.Instance.ActionMostTime(m_totalActionTime);
        }     
        else
            StroyLineManager.Instance.ActionMostTime(0f);
    }

    protected override void RegisterEventHandler()
    {
        return;
    }


    public void StroyUIHandle(INotifyArgs notifyArgs)
    {
        //m_totalActionTime = 0f;
    }

    void OnDestroy()
    {
        //RemoveEventHandler(EventTypeEnum.StroyUIEnd.ToString(), StroyUIHandle);
    }
}
