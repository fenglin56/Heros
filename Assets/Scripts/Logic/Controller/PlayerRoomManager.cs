using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerRoomManager : ISingletonLifeCycle
{
    public enum Status
    {
        Normal,
    }

    private static PlayerRoomManager m_instance;
    public static PlayerRoomManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new PlayerRoomManager();
                SingletonManager.Instance.Add(m_instance);
            }
            return m_instance;
        }
    }

    private SMSGEctypeALLSeatInfo_SC m_RoomSeatInfo = new SMSGEctypeALLSeatInfo_SC();
    private SMsgEctypePracice_YaoNvUpdate_CSC m_YaoNvUpdateInfo = new SMsgEctypePracice_YaoNvUpdate_CSC();
	private SMsgActionXiuLianInfo_SC m_XiuLianInfo = new SMsgActionXiuLianInfo_SC();


    public void UpdateRoomSeatInfo(SMSGEctypeALLSeatInfo_SC info)
    {
        m_RoomSeatInfo = info;
        NotifyManager.RaiseEvent(EventTypeEnum.UpdateRoomSeatInfo.ToString(), null);
    }

    public void UpdateSeatInfo(SMSGEctypeSeatInfo_SC info)
    {
        /*
        uint curHerosNum = m_RoomSeatInfo.dwPlayerNum;
        if (info.dwPlayerNum > curHerosNum)//进入
        {
            m_RoomSeatInfo.SeatInfoList.Add(info.sSeatInfo);
        }
        else//离开
        {
            m_RoomSeatInfo.SeatInfoList.Remove(info.sSeatInfo);                    
        }
        m_RoomSeatInfo.dwPlayerNum = curHerosNum;
        */
    }

    public void UpdateYaoNvUpdateInfo(SMsgEctypePracice_YaoNvUpdate_CSC info)
    {
        m_YaoNvUpdateInfo = info;
    }

	public void UpdateXiuLianInfo(SMsgActionXiuLianInfo_SC info)
	{
		m_XiuLianInfo = info;
	}

    public SMSGEctypeALLSeatInfo_SC GetRoomSeatInfo()
    {
        return m_RoomSeatInfo;
    }

    public SMsgEctypePracice_YaoNvUpdate_CSC GetYaoNvUpdateInfo()
    {
        return m_YaoNvUpdateInfo;
    }

	public SMsgActionXiuLianInfo_SC GetXiuLianInfo()
	{
		return m_XiuLianInfo;
	}

    public void Instantiate()
    {
    }

    public void LifeOver()
    {
        m_instance = null;
    }
}
