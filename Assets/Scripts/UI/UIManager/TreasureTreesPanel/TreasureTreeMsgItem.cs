using UnityEngine;
using System.Collections;

public class TreasureTreeMsgItem : MonoBehaviour {

    public UILabel m_text;

    public void Setup(string text)
    {
        m_text.SetText(text);
    }
	
}
