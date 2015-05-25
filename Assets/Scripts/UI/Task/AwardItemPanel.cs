using UnityEngine;
using System.Collections;

public class AwardItemPanel : MonoBehaviour {
	
	public UISprite PropIcon;
    //private GameObject m_propIcon;
	// Use this for initialization
    public void InitPanel(string spriteName)
    {
        //if (m_propIcon != null)
        //{
        //    DestroyImmediate(m_propIcon);
        //}

        PropIcon.spriteName = spriteName;
    }
	
}
