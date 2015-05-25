using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UIDragPanelController : MonoBehaviour
{

    public UIPanel uiPanel;
    public UIDraggablePanel uiDraggablePanel;
    public Transform Grid;

    void Awake()
    {
        if (uiDraggablePanel == null) { uiDraggablePanel = GetComponent<UIDraggablePanel>(); }
        if (uiPanel == null) { uiPanel = GetComponent<UIPanel>(); }
        if (Grid == null) { Grid = transform.GetChild(0); }
        float Hieght = uiPanel.clipRange.w;
        float ClipX = Hieght * Screen.width / Screen.height;
        //print(ClipX);
        Vector4 ClipRange = uiPanel.clipRange;
        ClipRange.z = ClipX;
        uiPanel.clipRange = ClipRange;
    }

    [ContextMenu("MoveFirst")]
    public void GoHorizontalFirst()
    {
        float ChildDistance =Mathf.Abs(Grid.GetChild(0).localPosition.x - Grid.GetChild(1).localPosition.x);
        float HalfScreent = uiPanel.clipRange.z / 2;
        float MoveTargetPoint = HalfScreent - ChildDistance / 2;
        transform.localPosition = new Vector3(-MoveTargetPoint, 0, 0);
        Vector4 ClipRange = uiPanel.clipRange;
        ClipRange.x = MoveTargetPoint;
        uiPanel.clipRange = ClipRange;
    }

    [ContextMenu("MoveEnd")]
    public void GoHorizontalEnd()
    {
        float ChildDistance = Mathf.Abs(Grid.GetChild(0).localPosition.x - Grid.GetChild(1).localPosition.x);
        float HalfScreent = uiPanel.clipRange.z / 2;
        float MoveTargetPoint = ChildDistance*(Grid.childCount-1)-HalfScreent+ChildDistance/2;
        transform.localPosition = new Vector3(-MoveTargetPoint, 0, 0);
        Vector4 ClipRange = uiPanel.clipRange;
        ClipRange.x = MoveTargetPoint;
        uiPanel.clipRange = ClipRange;
    }

}
