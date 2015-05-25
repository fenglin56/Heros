using UnityEngine;
using System.Collections;

/// <summary>
/// 特效生命周期脚本
/// </summary>
public class SpecialEffectsLifeCycle : MonoBehaviour {

    private float m_alphaLoseFactor;
    public float LifeTime;
    public float ScaleFactor;
    public Transform[] Elephents;

    private string m_colorName;

	// Use this for initialization
	void Start () {
        StartCoroutine(this.DestroyMySelf());
        m_alphaLoseFactor = Time.deltaTime / LifeTime;
        m_colorName = "_Color";
	}
    public void ResetLifeTime(float lifeTime)
    {
        this.LifeTime = lifeTime;
        m_alphaLoseFactor = Time.deltaTime / LifeTime;
        //StartCoroutine(this.DestroyMySelf());
    }
    public string ColorName
    {
        set { this.m_colorName = value; }
    }
	// Update is called once per frame
	void Update () {
        //try
        //{
            if (this.ScaleFactor != 0)
            {
                this.transform.localScale += this.transform.localScale * ScaleFactor * Time.deltaTime;
            }

            //foreach (var e in Elephents)
            //{
            //    if (e.renderer.material != null)
            //    {
            //        if (e.renderer.material.HasProperty(m_colorName))
            //        {
            //            Color originColor = e.renderer.material.GetColor(m_colorName);
            //            originColor.a = originColor.a - m_alphaLoseFactor;
            //            e.renderer.material.SetColor(m_colorName, originColor);
            //        }
            //    }
            //}
        //}
        //catch { }
	}
    private IEnumerator DestroyMySelf()
    {
        if (LifeTime > 0)
        {
            yield return new WaitForSeconds(LifeTime);
            Destroy(this.gameObject);
        }
    }
}
