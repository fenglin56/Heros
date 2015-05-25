using UnityEngine;
using System.Collections;


[ExecuteInEditMode]
public class NGUIScreenPosition : MonoBehaviour {

    public ScreenPosition screenPosition = ScreenPosition.Left;

    public Camera UICamera;

    [ContextMenu("ResetPosition")]
    void Start()
    {
        FindCamera(transform);
        SetPosition();
    }

    void FindCamera(Transform m_transform)
    {
        var camera = m_transform.GetComponent<Camera>();
        if (camera == null)
        {
            FindCamera(m_transform.parent);
        }
        else 
        {
            this.UICamera = camera;
        }
    }

    void SetPosition()
    {
        Vector3 Screenpoint = transform.localPosition;
        switch (this.screenPosition)
        {
            case ScreenPosition.Left:
                Screenpoint = new Vector3(0,0.5f,0);
                break;
            case ScreenPosition.Right:
                Screenpoint = new Vector3(1, 0.5f, 0);
                break;
            default:
                break;
        }
        Vector3 NewPos = UICamera.ViewportToWorldPoint(Screenpoint);
        //TraceUtil.Log(NewPos);
        transform.position = NewPos;
        Vector3 NewLocalPos = transform.localPosition;
        NewLocalPos.z = 0;
        transform.localPosition = NewLocalPos;
    }

}


public enum ScreenPosition {Left,Right}