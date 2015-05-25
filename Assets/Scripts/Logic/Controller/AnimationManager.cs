using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

//
/// <summary>
/// 动画管理器，读取角色状态，触发事件
/// </summary>
public class AnimationManager : Controller, ISingletonLifeCycle
{
    private Dictionary<Int64, List<AnimationClip>> m_roleAnimatinMap = new Dictionary<long, List<AnimationClip>>();
    private static AnimationManager m_instance;
    public static AnimationManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new AnimationManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }
    /// <summary>
    /// 角色动画注入接口
    /// </summary>
    /// <param name="uid">角色ID</param>
    /// <param name="animation">动画组件</param>
    public void RegisteAnimMapInfo(Int64 uid,Animation animation)
    {
        m_roleAnimatinMap.Clear();
        if (animation != null)
        {
            var clips = SplitAnimation(animation);
            if (this.m_roleAnimatinMap.ContainsKey(uid))
            {
                this.m_roleAnimatinMap[uid] =clips;
            }
            else
            {
                this.m_roleAnimatinMap.Add(uid,clips);
            }
        }
    }
    /// <summary>
    /// 触发动画，由角色每帧调用，并监听相关事件
    /// </summary>
    /// <param name="uid"></param>
    public void InvokeAnimation(Int64 uid)
    {

    }
    /// <summary>
    /// 附加动画事件
    /// </summary>
    /// <param name="clip">目标动画片段</param>
    private void AttachAnimEvent(AnimationClip clip)
    {
        //AnimationEvent animEvent=new AnimationEvent();
        //clip.
        //UnityEditor.AnimationUtility.
        //animEvent.
        //clip.AddEvent(null);
    }
    /// <summary>
    /// 分解动画片段。
    /// </summary>
    /// <param name="animation"></param>
    /// <returns></returns>
    private List<AnimationClip> SplitAnimation(Animation animation)
    {
        List<AnimationClip> animationClips = new List<AnimationClip>();
        foreach (AnimationState state in animation)
        {
            animationClips.Add(state.clip);
        }
        return animationClips;
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
public class HeroAnimaitonStatus
{
    public PlayerActionStatus ActionStatus;
    public GameManager.GameState GameState
    {
        get { return GameManager.Instance.CurrentState; }
    }
}
public enum PlayerActionStatus
{
    None,
    Idle,
    Running,
    Attacking,
    FireSkill,
}
