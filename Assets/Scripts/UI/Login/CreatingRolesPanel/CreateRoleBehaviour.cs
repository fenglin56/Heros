using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum RoleState
{
    Empty,
    Init,
    Click,
    Show,
    Back
}

public class CreateRoleBehaviour : View {

    public Animation AnimComponet;
    private CreateRoleUIData m_curRoleData = null;
    public CreateRoleUIData SetRoleData { set { m_curRoleData = value; } }

    private RoleState m_roleState = RoleState.Empty;
    private bool isPlayAnim = false;
    private string animName;
    private float animTime = 0;
    private int m_index = 0;
    private List<string> m_curAnimList = new List<string>();
    private bool m_lockRole;//锁定角色不能被点击。用于解决状态瞬移问题
    void Awake()
    {
        RegisterEventHandler();
    }

    protected override void RegisterEventHandler()
    {
        AddEventHandler(EventTypeEnum.OnTouchInvoke.ToString(), OnTouchDown);
        CreateRoleUI_v4.Instance.OnDoAction += PlayAnim;
    }

    //void OnGUI()
    //{
    //    if (GUILayout.Button("Init"))
    //    {
    //        AnimComponet.CrossFade("LoginPoses01");
    //    }
    //    if (GUILayout.Button("GotoFront"))
    //    {
    //        AnimComponet.CrossFade("LoginPoses02");
    //    }
    //    if (GUILayout.Button("Back"))
    //    {
    //        AnimComponet.CrossFade("LoginPoses04");
    //    }
    //    if (GUILayout.Button("Show"))
    //    {
    //        AnimComponet.CrossFade("LoginPoses03");
    //    }
    //}
    void PlayAnim(RoleState state, int vocation)
    {
        if (m_curRoleData._VocationID != vocation)
        {
            if (m_roleState == RoleState.Show)
            {
                m_roleState = RoleState.Back;
            }
            else if (m_roleState == RoleState.Click)
            {
                StartCoroutine(LockRoleClick());
                m_roleState = RoleState.Init;
            }
            else
            {
                //if(m_curRoleData._VocationID==1)
                //Debug.Log("状态1：" + m_curRoleData._VocationID + "  " + m_roleState);
                return;
            }
             
        }
        else
            m_roleState = state;

        m_curAnimList.Clear();
		m_index = 0;
		animTime = 0;

        switch (m_roleState)
        {
            case RoleState.Init:
                m_curAnimList.Add(m_curRoleData._InitAnim);
                break;
            case RoleState.Click:
                for (int i = 0; i < m_curRoleData._AnimList.Length; i++)
                {
                    m_curAnimList.Add(m_curRoleData._AnimList[i]);
                }
                break;
            case RoleState.Show:
                m_curAnimList.Add(m_curRoleData._StopAnim);
                break;
            case RoleState.Back:
                m_curAnimList.Add(m_curRoleData._BackAnim);
                break;
        }
	        
    }
    private IEnumerator LockRoleClick()
    {
        m_lockRole = true;
        yield return new WaitForSeconds(1);
        m_lockRole = false;
    }
    public void OnTouchDown(INotifyArgs e)
    {
        TouchInvoke touchInvoke = (TouchInvoke)e;
        if (touchInvoke.TouchGO == gameObject && m_roleState == RoleState.Init && !m_lockRole)  //只有角色处理Init状态才响应点击选择
        {
             UIEventManager.Instance.TriggerUIEvent(UIEventType.SelectRole, m_curRoleData._VocationID);
        }
    }


    void Update()
    {
        if (m_roleState == RoleState.Init)
        {
            if (!AnimComponet.IsPlaying(m_curRoleData._InitAnim))
            {
                //if (m_curRoleData._VocationID == 1)
                //    Debug.Log(m_roleState + " To RoleState.Init " + m_curRoleData._InitAnim);
                AnimComponet.CrossFade(m_curRoleData._InitAnim);
            }
        }
        else if (m_roleState == RoleState.Show)
        {
            if (!AnimComponet.IsPlaying(m_curRoleData._StopAnim))
            {
                //if (m_curRoleData._VocationID == 1)
                //    Debug.Log(m_roleState + " To RoleState.Show " + m_curRoleData._StopAnim);
                AnimComponet.CrossFade(m_curRoleData._StopAnim);
            }
        }
        else
        {
            animTime += Time.deltaTime;
            if (m_index < m_curAnimList.Count)
            {
                //AnimComponet[m_curAnimList[m_index]].weight = 0.3f;
                if (AnimComponet.animation[m_curAnimList[m_index]].length > animTime)
                {
                    if (!AnimComponet.IsPlaying(m_curAnimList[m_index]))
                    {
                        //Debug.Log(m_roleState + " " + m_curAnimList[m_index]);
                        AnimComponet.CrossFade(m_curAnimList[m_index]);
                    }
                }
                else
                {
                    animTime = 0;
                    m_index++;
                }
            }
            else
            {
                m_curAnimList.Clear();
                m_index = 0;
                if (m_roleState == RoleState.Click)
                {
                    m_roleState = RoleState.Show;
                }
                else if (m_roleState == RoleState.Back)
                {
                    m_roleState = RoleState.Init;
                }
                //else
                //{
                    //if (m_curRoleData._VocationID == 1)
                    //Debug.Log("状态2：" + m_curRoleData._VocationID + "  " + m_roleState);
                //}

            }
        }

    }

    void Destroy()
    {
        RemoveAllEvent();
        CreateRoleUI_v4.Instance.OnDoAction -= PlayAnim;
    }

}
