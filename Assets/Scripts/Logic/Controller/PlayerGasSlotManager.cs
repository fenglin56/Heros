using UnityEngine;
using System.Collections;

/// <summary>
/// EnterPoint Scene GameManager  玩家气槽管理
/// </summary>
public class PlayerGasSlotManager : View {

    //private const int MaxAirSlotValue = 3;  //最大体力值
    private const float RestoreIntervalTime = 5f;  //恢复时间
    private int m_airSlotValue = 0;
    private float m_restoreTime = 0;

    private static PlayerGasSlotManager m_instance;
    public static PlayerGasSlotManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    public int GetAirSlotValue
    {
        get { return m_airSlotValue; }
    }

    //public int MaxEnergySlot { set;}

    //重置蓄气(满体力)
    public void UpdateRollStrength(int consume)
    {
        m_airSlotValue = consume;//MaxAirSlotValue;
        SendUpdateRollValueEvent(m_airSlotValue);
    }
    //恢复一点体力
    public void AddOneAirSlot()
    {
        m_airSlotValue += 1;
        SendUpdateRollValueEvent(m_airSlotValue);
    }
    //消耗一点体力
    public void ConsumeOneRollAirSlot(int consume)
    {
		if(m_airSlotValue <= 0)
			return;
		
        m_airSlotValue -= consume;
        SendUpdateRollValueEvent(m_airSlotValue);
    }
    //能否滚动
    public bool IsCanRoll()
    {
        if (m_airSlotValue <= 0)
        {
            SoundManager.Instance.PlaySoundEffect("sound0046");
            RaiseEvent(EventTypeEnum.NoEnoughRollAir.ToString(), null);
            return false;
        }
        return true;
    }
    //发送更新翻滚体力点消息
    public void SendUpdateRollValueEvent(int value)
    {
        SUpdateRollStrengthStruct updateRSStruct = new SUpdateRollStrengthStruct() { strengthValue = value };
        RaiseEvent(EventTypeEnum.UpdateRollAirSlot.ToString(), updateRSStruct);
    }

	
	void Start()
	{
		m_instance = this;
	}
	
	void OnDestroy()
	{
		m_instance = null;
	}

    protected override void RegisterEventHandler()
    {
        return;
    }
}
