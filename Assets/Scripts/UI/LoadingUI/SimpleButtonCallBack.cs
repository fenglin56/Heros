using UnityEngine;
using System.Collections;

public class SimpleButtonCallBack : MonoBehaviour {
    public delegate void OnButtonClick();

    private OnButtonClick m_onButtonClick;
	
    void OnClick()
    {
        if(null != m_onButtonClick)
        {
            m_onButtonClick();
        }
    }

    public void AddClickDelegate(OnButtonClick del)
    {
        m_onButtonClick += del;
    }

    void OnDestroy()
    {
        m_onButtonClick = null;
    }
}
