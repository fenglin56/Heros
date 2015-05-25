using UnityEngine;
using System.Collections;

public class PlayerRollStrengthManager : View 
{
    private static PlayerRollStrengthManager m_instance;
    public static PlayerRollStrengthManager Instance
    {
        get
        {
            return m_instance;
        }
    }

    private const int MaxStrengthValue = 3;  //最大体力值
    private const float RestoreIntervalTime = 5f;  //恢复时间
    private int m_rollStrengthValue = 0;
    private float m_restoreTime = 0;
    //重置体力(满体力)
    public void ResetRollStrength()
    {
        m_rollStrengthValue = MaxStrengthValue;
        SendUpdateRollValueEvent(m_rollStrengthValue);
    }
    //恢复一点体力
    private void RestoreOneRollStrength()
    {
        m_rollStrengthValue += 1;
        SendUpdateRollValueEvent(m_rollStrengthValue);
    }
    //消耗一点体力
    public void ConsumeOneRollStrength()
    {
        m_rollStrengthValue -= 1;
        SendUpdateRollValueEvent(m_rollStrengthValue);
    }
    //能否滚动
    public bool IsCanRoll()
    {
        if (m_rollStrengthValue <= 0)
        {
            SoundManager.Instance.PlaySoundEffect("sound0046");
            //RaiseEvent(EventTypeEnum.NoEnoughRollStrength.ToString(), null);
            return false;
        }
        return true;
    }
    //发送更新翻滚体力点消息
    public void SendUpdateRollValueEvent(int value)
    {
        SUpdateRollStrengthStruct updateRSStruct = new SUpdateRollStrengthStruct() { strengthValue = value };
        //RaiseEvent(EventTypeEnum.UpdateRollStrength.ToString(), updateRSStruct);
    }

    void Start () 
    {
        m_instance = this;
	}

    void OnDestroy()
    {
        m_instance = null;
    }
		

	void Update () 
    {
        if (m_rollStrengthValue < MaxStrengthValue)
        {
            m_restoreTime += Time.deltaTime;

            if (m_restoreTime >= RestoreIntervalTime)
            {
                m_restoreTime -= RestoreIntervalTime;
                this.RestoreOneRollStrength();

                if (m_rollStrengthValue == MaxStrengthValue)
                {
                    m_restoreTime = 0;
                }
            }
        }
	}

    protected override void RegisterEventHandler()
    {
        throw new System.NotImplementedException();
    }
}
