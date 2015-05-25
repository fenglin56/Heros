using UnityEngine;
using System.Collections;


public class RoleViewClickEvent : MonoBehaviour {

	
    void OnClick()
    {
        UIEventManager.Instance.TriggerUIEvent(UIEventType.OnRoleViewClick,null);
    }

}
