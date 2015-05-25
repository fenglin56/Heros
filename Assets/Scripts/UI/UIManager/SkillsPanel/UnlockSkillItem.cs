using UnityEngine;
using System.Collections;
using UI;

public class UnlockSkillItem : MonoBehaviour, IPagerItem
{
    public SpriteSwith FocusBGSwitch;
    public Transform UnlockSkillGo;
    public UILabel UnlockLevel;
    private Transform m_cacheTransform;
	// Use this for initialization
    public void InitItemData(int unlockLv)
    {
        UnlockLevel.text = string.Format(LanguageTextManager.GetString("IDS_H1_404"), unlockLv);
    }

    public void OnGetFocus()
    {
        FocusBGSwitch.ChangeSprite(2);
    }

    public void OnLoseFocus()
    {
        FocusBGSwitch.ChangeSprite(1);
    }

    public void OnBeSelected()
    {
        
    }

    public void IsShow(bool bFlag)
    {
        if (bFlag)
        {
            UnlockSkillGo.localPosition = new Vector3(0, 0, -1);
        }
        else
        {
            UnlockSkillGo.localPosition = new Vector3(0, 0, -1000);
        }
    }

    public Transform GetTransform()
    {
        if (m_cacheTransform == null)
        {
            m_cacheTransform = transform;
        }
        return m_cacheTransform;
    }
}
