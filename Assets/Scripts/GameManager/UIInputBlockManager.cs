using UnityEngine;
using System.Collections;

public class UIInputBlockManager: MonoBehaviour {


    private const float DEFAULT_BLOCK_TIME = 0.3f;


    private static UIInputBlockManager m_instance;
    public static UIInputBlockManager Instance
    {
        get
        {
            if(null == m_instance)
            {
                m_instance = FindObjectOfType(typeof(UIInputBlockManager)) as UIInputBlockManager;
            }
            return m_instance;
        }
    }

    private bool m_blocked = false;
    public bool Blocked
    {
        get { return m_blocked; }
    }


    void Awake()
    {
        m_instance = this;
    }

    public bool CheckInputBlocked()
    {
        if(m_blocked)
        {
            return true;
        }
        else
        {
            SetBlockDefault();
            return false;
        }

    }




	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetBlockDefault()
    {
        SetBlockForSeconds(DEFAULT_BLOCK_TIME);
    }

    public void SetBlockForSeconds(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(StartBlockInput(duration));
    }

    public void EnableInputImmediately()
    {
        StopAllCoroutines();
        m_blocked = false;
    }

    IEnumerator StartBlockInput(float duration)
    {
        m_blocked = true;
        yield return new WaitForSeconds(duration);
        m_blocked = false;
    }

}
