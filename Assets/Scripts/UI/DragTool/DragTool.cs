using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DragTool : MonoBehaviour {


    public bool canDrag;

    public bool CanDrag
    {
        get { return canDrag; }
        set { 
            canDrag = value; 
            //Debug.LogWarning("SetDragToolDragFlag:"+value.ToString()); 
        }
    }
    private List<DragComponentSlot> DragChildList = new List<DragComponentSlot>();

    void Start()
    {
        CanDrag = true;
        UIEventManager.Instance.RegisterUIEvent(UIEventType.ResetDragComponentStatus, ResetDragStatus);
    }

    void OnDestroy()
    {
        UIEventManager.Instance.RemoveUIEventHandel(UIEventType.ResetDragComponentStatus,ResetDragStatus);
    }

    void ResetDragStatus(object obj)
    {
        this.CanDrag = true;
    }

    public void AddDragSlot(DragComponentSlot dragChild)
    {
        DragChildList.Add(dragChild);
    }
    /// <summary>
    /// 检测是否有碰撞
    /// </summary>
    /// <param name="CheckChild"></param>
    /// <returns></returns>
    public DragComponentSlot GetColliderBox(DragComponent CheckChild)
    {
        DragComponentSlot minDistanceChild = null;
        float minDistance = 1000;
        foreach (var child in DragChildList)
        {
            if (child.IsCheckMySelf)
            {
                float distance = Vector3.Distance(child.ComponentArea.position, CheckChild.ComponentArea.position);
                if (CheckChild != child && distance < minDistance)
                {
                    minDistance = distance;
                    minDistanceChild = child;
                }
            }
        }
        if (CheckIsCollision(CheckChild, minDistanceChild))
        {
            return minDistanceChild;
        }
        else
        {
            return null;
        }
    }
    /// <summary>
    /// 检测两个是否碰撞
    /// </summary>
    /// <param name="childA"></param>
    /// <param name="childB"></param>
    /// <returns></returns>
    bool CheckIsCollision(DragComponent childA, DragComponentSlot childB)
    {
        Transform childAParent = childA.transform.parent;
        Transform childBParent = childB.transform.parent;
        childA.transform.parent = transform;
        childB.transform.parent = transform;
        Vector3 positionA = childA.ComponentArea.localPosition;
        Vector3 positionB = childB.ComponentArea.localPosition;
        BoxCollider boxColliderA = childA.ComponentArea.collider as BoxCollider;
        BoxCollider boxColliderB = childB.ComponentArea.collider as BoxCollider;
        //Vector3 scaleA = boxColliderA.size;
        //Vector3 scaleB = childB.ComponentArea.localScale;
        bool isXCollision = Mathf.Abs(positionA.x - positionB.x) < (boxColliderA.size.x / 2 + boxColliderB.size.x / 2);
        bool isYCollision = Mathf.Abs(positionA.y - positionB.y) < (boxColliderA.size.y / 2 + boxColliderB.size.y / 2);
        //TraceUtil.Log(string.Format("检测是否碰撞：{0}和{1},是否碰撞：{2}", childA.gameObject.name, childB.gameObject.name, isXCollision && isYCollision));
        childA.transform.parent = childAParent;
        childB.transform.parent = childBParent;
        return isXCollision && isYCollision;
    }

}
