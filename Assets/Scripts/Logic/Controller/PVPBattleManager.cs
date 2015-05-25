using UnityEngine;
using System.Collections;

public class PVPBattleManager : ISingletonLifeCycle
{
    private static PVPBattleManager m_instance;
    public static PVPBattleManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new PVPBattleManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    public enum PVPBattleState
    {
        ready,
        battle,
        settle,
    }
    private PVPBattleState m_currentState = PVPBattleState.ready;
    public PVPBattleState CurrentState { get { return m_currentState; } }

    //pvp对手信息
    private SEctypePvpPlayer m_pvpPlayerData;
    public void SavePVPPlayerData(SEctypePvpPlayer data)
    {
        m_pvpPlayerData = data;
        m_isPVPBattle = true;
        this.SetPVPState(PVPBattleState.ready);
    }    
    public SEctypePvpPlayer GetPVPPlayerData()
    {
        return m_pvpPlayerData;
    }
    public void ClearPVPPlayerData()
    {
        m_pvpPlayerData = new SEctypePvpPlayer();
        m_isPVPBattle = false;        
    }

    //是否pvp战斗
    private bool m_isPVPBattle;
    public bool IsPVPBattle { get { return m_isPVPBattle; } set { m_isPVPBattle = value; } }        

    //玩家排名
    private int m_myRankingIndex;
    public int MyRankingIndex { get { return m_myRankingIndex; } }
    public void SetMyRankingIndex(int value)
    {
        m_myRankingIndex = value;
    }

    //pvp战斗状态
    public bool IsCanMove()
    {
        return m_isPVPBattle == false || m_currentState == PVPBattleState.battle;
    }
    public void SetPVPState(PVPBattleState state)
    {
        m_currentState = state;
    }

    public void Instantiate()
    {
    }

    public void LifeOver()
    {        
        m_instance = null;
    }
}
