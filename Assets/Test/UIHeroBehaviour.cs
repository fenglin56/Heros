using UnityEngine;
using System.Collections;
using System.Linq;

public class UIHeroBehaviour : View {

    private SkillBase[] m_skills;
    private FSMSystem m_FSMSystem;

	// Use this for initialization
	void Start () {
        m_skills = GetComponents<SkillBase>();
       // InitFSM();
	}   
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 初始化状态机,添加种状态
    /// </summary>
    private void InitFSM()
    {
        m_FSMSystem = new FSMSystem(this);

        PlayerIdleState idleState = new PlayerIdleState();
        idleState.AddTransition(Transition.PlayerToIdle, StateID.PlayerIdle);
        m_FSMSystem.AddState(idleState);
        PlayerRunState runState = new PlayerRunState();
        runState.AddTransition(Transition.PlayerToTarget, StateID.PlayerRun);
        m_FSMSystem.AddState(runState);
        PlayerBeAttackedState beAttackedState = new PlayerBeAttackedState();
        beAttackedState.AddTransition(Transition.PlayerBeAttacked, StateID.PlayerBeAttacked);
        m_FSMSystem.AddState(beAttackedState);
        PlayerNormalSkillFireState normalSkillFireState = new PlayerNormalSkillFireState();
        normalSkillFireState.AddTransition(Transition.PlayerFireNormalSkill, StateID.PlayerNormalSkill);
        m_FSMSystem.AddState(normalSkillFireState);
        PlayerInitiativeSkillFireState initiativeSkillFireState = new PlayerInitiativeSkillFireState();
        initiativeSkillFireState.AddTransition(Transition.PlayerFireInitiativeSkill, StateID.PlayerInitiativeSkill);
        m_FSMSystem.AddState(initiativeSkillFireState);
    }
    public SkillBase GetSkillByAniName(string aniString)
    {
        return m_skills.SingleOrDefault(P => P.AniStr == aniString);
    }
    public SkillBase GetSkillByAniName(int skillID)
    {
        return m_skills.SingleOrDefault(P => P.SkillId == skillID);
    }

    protected override void RegisterEventHandler()
    {

    }
}
