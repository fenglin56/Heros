using UnityEngine;
using System.Collections;

public class BeautiesPanel : MonoBehaviour {

    public UISprite Clothes;

    float Speed = 100;
    float BackTime = 1;

    void Awake()
    {
        Speed = CommonDefineManager.Instance.CommonDefine.LoadingTransparent;
        BackTime = CommonDefineManager.Instance.CommonDefine.LoadingTransparentReturn/1000f;
    }
    

    public void OnDrag(Vector2 drs)
    {
        float dragDistance = Vector2.Distance(drs,Vector2.zero)*0.000005f*Speed;
        //TraceUtil.Log("OnDrag:"+drs+","+dragDistance);
        if (Clothes.alpha > 0)
        {
            Clothes.alpha -= dragDistance;
            //TraceUtil.Log("SetAlpha:"+Clothes.color.a);
        }
    }

    void Update()
    {
        if (Clothes.alpha < 1)
        {
            float backSpeed = Time.deltaTime / BackTime;
            Clothes.alpha += backSpeed;
        }
    }

    void SetClothesAlpha(float a)
    {
        Color m_color = Clothes.color;
        m_color.a = a;
        Clothes.color = m_color;
    }
}
