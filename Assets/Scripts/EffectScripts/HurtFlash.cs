using UnityEngine;
using System.Collections;
using System.Linq;

public class HurtFlash : MonoBehaviour {
    public Renderer[] NormalRender;
	public Renderer[] SplitRender;

    public Color NoDisruptColor;
    public Color DisruptColor;

    public Color HordeColor = Color.blue;
    public Color NoHordeColor = Color.black;

	private Material[] m_mat;
    private Material[] m_hordeMat;
	private float m_flashTotalTime;
	private float m_flashTime = 0;

    private float m_disruptTime = 0;
    private bool m_isDisrupt = false;

    private float m_hordeTime = 0;
    private bool m_isHorde = false;

	public void OnAttack(bool isNormal,float time)
	{
        var renders = isNormal ? NormalRender: SplitRender;
        m_mat = (from item in renders select item.material).ToArray();
		m_flashTotalTime = time;
		m_flashTime = time;
	}

    public void OnDisrupt(bool isDisrupt)
    {
        m_mat = (from item in NormalRender select item.material).ToArray();

        m_isDisrupt = isDisrupt;

        if (isDisrupt)
        {
            m_mat.ApplyAllItem(P => { if (P != null)P.SetColor("_DisruptColor", DisruptColor); });
        }
        else
        {
            m_mat.ApplyAllItem(P => { if (P != null)P.SetColor("_DisruptColor", NoDisruptColor); });
        }

        m_disruptTime = 0;
    }

    public void OnHorde(bool isHorde)
    {
        m_hordeMat = (from item in NormalRender select item.material).ToArray();
        
        m_isHorde = isHorde;
        
        if (isHorde)
        {
            m_hordeMat.ApplyAllItem(P => { if (P != null)P.SetColor("_HordeColor", HordeColor); });
        }
        else
        {
            m_hordeMat.ApplyAllItem(P => { if (P != null)P.SetColor("_HordeColor", NoHordeColor); });
        }
        
        m_hordeTime = 0;
    }

	// Use this for initialization
	void Start ()
	{
        m_mat = new Material[NormalRender.Length];
	}
	
	// Update is called once per frame
	void Update () 
	{
        float dt = Time.deltaTime;

		if(m_flashTime > 0.0f)
		{
			float step = (m_flashTotalTime - m_flashTime)/m_flashTotalTime;

            m_mat.ApplyAllItem(P => { if (P != null)P.SetFloat("_HurtStep", step); });
			m_flashTime -= dt;
		}


        if (m_isDisrupt)
        {
            m_disruptTime = m_disruptTime - Mathf.FloorToInt(m_disruptTime);
            m_mat.ApplyAllItem(P => { if (P != null)P.SetFloat("_DisruptStep", m_disruptTime); });
            m_disruptTime += Time.deltaTime;
        }

        if(m_isHorde)
        {
            m_hordeTime = m_hordeTime - Mathf.FloorToInt(m_hordeTime);
            m_hordeMat.ApplyAllItem(P => { if (P != null)P.SetFloat("_HordeStep", m_hordeTime); });
            m_hordeTime += Time.deltaTime;
        }
		
	}
	
	[ContextMenu( "OnAttack 1s" ) ]
    public void OnDisrupt()
	{
        OnDisrupt(true);
	}
	
	
}
