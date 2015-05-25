using UnityEngine;
using System.Collections;

public class PickUpEffectBehaviour : MonoBehaviour 
{
    public float DelayTime = 3f;
    public UISprite Sprite_Icon;
    private bool m_isRise;
    private float m_lifeTime = 0;

    public void Begin(string iconSpriteName)
    {        
        this.Sprite_Icon.spriteName = iconSpriteName;
        m_isRise = true;
    }

    void LateUpdate()
    {
        if (m_isRise)
        {
            transform.rotation = Quaternion.identity;
            m_lifeTime += Time.deltaTime;
            if (m_lifeTime > DelayTime)
            {
                m_isRise = false;
                Destroy(gameObject);
            }
        }
    }
}
