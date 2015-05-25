using UnityEngine;
using System.Collections;

public class NPCTitle : MonoBehaviour {

    private bool m_showNpcTitle = false;
    private Vector3 m_showPosition = Vector3.zero;
    public UILabel NPCName;
    public UILabel NPCProfession;

    public void SetNpcTitle(string name, string profession, Vector3 position)
    {
        string _name;
        string _profession;

        if (name == "0" || name.Length == 0)
            _name = "";
        else
            _name = LanguageTextManager.GetString(name);

        if (profession == "0" || profession.Length == 0)
            _profession = "";
        else
           _profession = LanguageTextManager.GetString(profession);

        SetText(_name, _profession, position);
    }

    public void SetText(string name, string profession, Vector3 position)
    {
        this.NPCName.SetText(name);
        this.NPCProfession.SetText(profession);

        this.m_showNpcTitle = true;
        this.m_showPosition = position;
    }

    void Update()
    {
        if (m_showNpcTitle)
        {
            transform.position = GetPopupPos(this.m_showPosition, BattleManager.Instance.UICamera);
            Vector3 newPosition = transform.localPosition;
            newPosition.z = 601;
            transform.localPosition = newPosition;
        }
    }

    public Vector3 GetPopupPos(Vector3 sPos, Camera uiCamera)
    {
        var worldPos = Camera.main.WorldToViewportPoint(sPos);
        var uipos = uiCamera.ViewportToWorldPoint(worldPos);

        uipos.z = 1;
        return uipos;
    }
}
