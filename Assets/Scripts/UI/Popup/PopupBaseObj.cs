using UnityEngine;
using System.Collections;

public class PopupBaseObj
{
    private GameObject m_popupWidget;

    public void GetPopupWidget(string content, Vector3 position, GameObject popupLabelPrefab)
    {
        m_popupWidget = GameObjectPool.Instance.AcquireLocal(popupLabelPrefab, position, Quaternion.identity);

        var uilabel = m_popupWidget.GetComponent<UILabel>();
        uilabel.text = content;
    }
}
