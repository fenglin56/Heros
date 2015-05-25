using UnityEngine;
using System.Collections;

/// <summary>
/// 副本资源管理，挂在BattleUI场景下。
/// </summary>
public class BattleResManager : MonoBehaviour {

    public GameObject BossStatusPanelPrefab;
	public GameObject SecondBossStatusPanelPrefab;//第二boss血条
    //血条（普通，Boss，精英）
    public GameObject BloobBarPrefab_normal;
    public GameObject BloobBarPrefab_boss;
    public GameObject BloobBarPrefab;

    public GameObject MonsterDialogPrefab;
    private static BattleResManager m_instance = null;
    public static BattleResManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType(typeof(BattleResManager)) as BattleResManager;
            }
            return m_instance;
        }
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    void Awake()
    {
        m_instance = this;
    }
}
