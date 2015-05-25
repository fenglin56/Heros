using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerRoomDataManager : MonoBehaviour {

    private static PlayerRoomDataManager m_instance;
    public static PlayerRoomDataManager Instance { get { return m_instance; } }

    public PlayerSirenConfigDataBase playerSirenConfigDataBase;

    private List<PlayerSirenConfigData> PlayerSirenList = new List<PlayerSirenConfigData>();

    void Awake()
    {
        m_instance = this;
        InitPlayerSirenConfigData();
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    void InitPlayerSirenConfigData()
    {
        foreach (PlayerSirenConfigData data in playerSirenConfigDataBase._dataTable)
        {
            PlayerSirenList.Add(data);
        }
    }

    public List<PlayerSirenConfigData> GetPlayerSirenList()
    {
        return PlayerSirenList;
    }
}
