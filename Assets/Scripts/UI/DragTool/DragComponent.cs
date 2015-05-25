using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DragComponent : MonoBehaviour
{

    public Transform ComponentArea;//移动物件的区域
    public Transform NewSlotPoint;//当条件满足时移动到的新位置,为空则默认移动到目标槽上

    protected DragTool dragTool;

    Camera MyUICamera;
    private bool isDrag = false;

    [HideInInspector]
    public bool IsCanDrag = true;

    private Vector3 TouchPointDistance = Vector3.zero;
    private DragComponentSlot LastCollisionSlot = null;
    private Vector3 MyPosition = Vector3.zero;
    
    Transform myParent;

    //protected string PairTag;//配对标志
    //private List<DragComponent> enterDragComponentList = new List<DragComponent>();

    void Awake()
    {
        //MyUICamera = UICamera.currentCamera
    }

    public virtual void Start()
    {
        //TraceUtil.Log("寻找UICamera");
        MyUICamera = UICamera.currentCamera;
        GetParentDragTool(transform);
        MyPosition = transform.localPosition;
        myParent = transform.parent;
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

    public void OnPress(bool isPressed)
    {
        if (!enabled||!IsCanDrag)
            return;
        //TraceUtil.Log("OnDragComponentClick:"+isPressed+",IsDrag:"+isDrag);
        if (!isPressed && !isDrag)
        {
            OnDragComponetClick();
        }
        if (!isPressed && isDrag)
        {
            StartCoroutine(MoveForTime());
        }
        if (isPressed)
        {
            TouchPointDistance = GetTouchPosition() - MyUICamera.WorldToScreenPoint(transform.position);
        }
        else
        {
            isDrag = isPressed;
        }
    }

    IEnumerator MoveForTime()
    {
        yield return null;
            dragTool.CanDrag = false;
            if (LastCollisionSlot != null && LastCollisionSlot.CheckIsPair(this))
            {
                Transform targetTransform = NewSlotPoint == null ? LastCollisionSlot.transform : NewSlotPoint;
                Transform slotChacheTransform = targetTransform.transform.parent;
                targetTransform.transform.parent = dragTool.transform;
                transform.parent = dragTool.transform;
                TweenMoveToNewPoint(targetTransform.localPosition);
                targetTransform.transform.parent = slotChacheTransform;
            }
            else
            {
                TweenMoveBackPosition();
            }
    }

    public void OnDrag(Vector2 delta)
    {
        if (!enabled || !dragTool.CanDrag || !IsCanDrag)
            return;
        if (delta.x != 0 || delta.y != 0)
        {
            isDrag = true;
            OnDragComponent();
            CheckIsColliderBox();
        }
    }

    void OnDragComponent()
    {
        Vector3 TouchPosition = GetTouchPosition();
        float dragPositionZ = transform.position.z;
        Vector3 dragPosition = MyUICamera.ScreenToWorldPoint(TouchPosition - TouchPointDistance);
        dragPosition.z = dragPositionZ;
        transform.position = dragPosition;

        Vector3 currentPosition = transform.localPosition;
        currentPosition.z = MyPosition.z-30;
        transform.localPosition = currentPosition;
    }

    void CheckIsColliderBox()
    {
        DragComponentSlot colliderBox = dragTool.GetColliderBox(this);
        if (colliderBox != null)
        {
            if (colliderBox != LastCollisionSlot)
            {
                LastCollisionSlot = colliderBox;
                colliderBox.OnDragComponentHover();
            }
        }
        else
        {
            LastCollisionSlot = null;
        }
    }

    Vector3 GetTouchPosition()
    {
        Vector3 TouchPosition = Vector3.zero;
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer
            || Application.platform == RuntimePlatform.OSXEditor)
        {
            TouchPosition = Input.mousePosition;
        }
        else if (Input.touchCount > 0)
        {
            TouchPosition = Input.GetTouch(0).position;
        }
        return TouchPosition;
    }

    //bool CheckIsPair(DragComponentSlot checkChild)
    //{
    //    return this.PairTag == checkChild.PairTag;
    //}

    void TweenMoveBackPosition()
    {
        //Debug.LogWarning("返回到原点");
        TweenPosition.Begin(ComponentArea.gameObject, CommonDefineManager.Instance.CommonDefine.ItemReturn, ComponentArea.localPosition, MyPosition, TweenMoveBackComplete);
    }

    void TweenMoveToNewPoint(Vector3 newPosition)
    {
        //Debug.LogWarning("移动到新点");
        TweenPosition.Begin(ComponentArea.gameObject, CommonDefineManager.Instance.CommonDefine.ItemMoving, ComponentArea.localPosition, newPosition, TweenMoveToNewPointComplete);
    }

    void TweenMoveToNewPointComplete(object obj)
    {
        //Debug.LogWarning("移动到新点结束");
        transform.parent = myParent;
        dragTool.CanDrag = true;
        LastCollisionSlot.MoveToHere(this);
        this.MyPosition = transform.localPosition;
        MoveToNewPointComplete();
    }

    void TweenMoveBackComplete(object obj)
    {
        //Debug.LogWarning("返回到原点结束");
        dragTool.CanDrag = true;
        MoveBackComplete();
    }
    /// <summary>
    /// 移动回原位置完成
    /// </summary>
    public virtual void MoveBackComplete()
    {
        //TraceUtil.Log("MoveBackComplete");
    }
    /// <summary>
    /// 移动到新的地方了
    /// </summary>
    public virtual void MoveToNewPointComplete()
    {
    }
    /// <summary>
    /// 点击事件，替换原有的OnClick
    /// </summary>
    public virtual void OnDragComponetClick()
    {
        //TraceUtil.Log("OnDragComponetClick");
    }

    public virtual void OnDestroy()
    {
        if(dragTool!=null&&dragTool.CanDrag == false)
        {
            dragTool.CanDrag = true;
        }
    }
}
