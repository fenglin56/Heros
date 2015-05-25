using UnityEngine;
using System.Collections;

public class DragComponentSlot : MonoBehaviour {

    public Transform ComponentArea;// 槽区域
    protected DragComponent myDragComponentChild;
    [HideInInspector]
    public bool IsCheckMySelf = true;//是否被检测到

    //public string PairTag;

    DragTool dragTool;

    public virtual void Start()
    {
        GetParentDragTool(transform);
        dragTool.AddDragSlot(this);
    }

    public void GetParentDragTool(Transform child)
    {
        if (child.parent == null)
            return;
        DragTool dragTool = child.parent.GetComponent<DragTool>();
        if (dragTool == null)
        {
            GetParentDragTool(child.parent);
        }
        else
        {
            this.dragTool = dragTool;
        }
    }

    /// <summary>
    /// 检测被拖拽的物体能否移动到该槽位,判断条件由继承的子类自定
    /// </summary>
    /// <param name="checkChild">拖拽的物体</param>
    /// <returns></returns>
    public virtual bool CheckIsPair(DragComponent dragChild)
    {
        return true;
    }

    /// <summary>
    ///手指悬放到这里的时候
    /// </summary>
    public virtual void OnDragComponentHover()
    {
    }
    /// <summary>
    /// 某个物件要放到这里了
    /// </summary>
    /// <param name="enterComponent"></param>
    public virtual void MoveToHere(DragComponent enterComponent)
    {
    }
    

}
