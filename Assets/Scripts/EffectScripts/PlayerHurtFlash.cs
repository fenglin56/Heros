using UnityEngine;
using System.Collections;
using System.Linq;

public class PlayerHurtFlash : MonoBehaviour {

	// Use this for initialization
	public Renderer[] NormalRender;
	private Material[] m_mat;
	private float m_flashTotalTime;
	private float m_flashTime = 0;
	
	//private float m_burstFlashTotalTime;
	private float m_burstFlashTime = 0;
	private bool m_isBurst = false;


    private float m_reliveEffectTime = 0;
    private bool m_isReliveEffect = false;

    public float min = 1f;
    public float max = 3f;
    public Color reliveColor = Color.white;
  
	
	public void OnAttack(float time)
	{
        m_mat = (from item in NormalRender select item.material).ToArray();
		m_flashTotalTime = time;
		m_flashTime = time;
	}
    
	
	public void OnBurst(bool isBurst)
	{
        if (NormalRender == null)
            InitRenderMat();

		m_mat = (from item in NormalRender select item.material).ToArray();
		m_isBurst = isBurst;
		m_burstFlashTime = 0;

        if (!m_isBurst)
        {
            m_mat.ApplyAllItem(P => { if (P != null)P.SetColor("_BurstColor", Color.black); });
        }
        else
        {
            m_mat.ApplyAllItem(P => { if (P != null)P.SetColor("_BurstColor", Color.yellow); });
        }
	}


    public void OnRelive(float duration)
    {
        if (NormalRender == null)
            InitRenderMat();
        
        m_mat = (from item in NormalRender select item.material).ToArray();
        m_reliveEffectTime = duration;
        m_isReliveEffect = true;
        m_mat.ApplyAllItem(P => { if (P != null)P.SetColor("_RimLightColor", reliveColor); });


    }
	
	// Use this for initialization
	void Start ()
	{
        InitRenderMat();
	}

    void InitRenderMat()
    {
        NormalRender = this.transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        m_mat = new Material[NormalRender.Length];
    }
	
	// Update is called once per frame
	void Update () 
	{	

		if(m_flashTime > 0.0f)
		{
			float dt = Time.deltaTime;
			float step = (m_flashTotalTime - m_flashTime)/m_flashTotalTime;
            m_mat.ApplyAllItem(P => { if (P != null)P.SetFloat("_HurtStep", step); });
			m_flashTime -= dt;
		}
		
		if(m_isBurst)
		{
            m_burstFlashTime = m_burstFlashTime - Mathf.FloorToInt(m_burstFlashTime);
            m_mat.ApplyAllItem(P => { if (P != null)P.SetFloat("_BurstStep", m_burstFlashTime); });
			m_burstFlashTime += Time.deltaTime;
		}

        if(m_isReliveEffect)
        {
            m_reliveEffectTime -= Time.deltaTime;

            float step = m_reliveEffectTime;
            step = step - Mathf.FloorToInt(step);
            int a = (int)step;

            float rimPower = 0;
            if(a % 2 == 1)
            {
                rimPower =  Mathf.Lerp(min, max, step);
            }
            else
            {
                rimPower = Mathf.Lerp(max, min, step);
            }
            m_mat.ApplyAllItem(P => { if (P != null)P.SetFloat("_RimPower", rimPower); });

            if(m_reliveEffectTime < 0)
            {
                m_isReliveEffect = false;
                m_mat.ApplyAllItem(P => { if (P != null)P.SetColor("_RimLightColor", new Color(0, 0, 0, 0)); });
            }
        }
	}
	
	[ContextMenu( "OnAttack 1s" ) ]
    public void OnAttack1s( )
	{
		OnAttack(1.0f);
	}

    [ContextMenu("OnBurst")]
    public void OnBurstTrue( )
    {
        OnBurst(true);
    }

    [ContextMenu("OnRelive")]
    public void OnRelive5()
    {
        OnRelive(5);
    }
}
