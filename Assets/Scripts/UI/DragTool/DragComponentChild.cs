using UnityEngine;
using System.Collections;

public class DragComponentChild : MonoBehaviour {


    DragComponent dragComponent;

    private bool isDrag = false;

    void Start()
    {
        GetParentDragComponent(transform);
    }

    public void GetParentDragComponent(Transform child)
    {
        DragComponent dragComponent = child.GetComponent<DragComponent>();
        if (dragComponent == null)
        {
            GetParentDragComponent(child.parent);
        }
        else
        {
            this.dragComponent = dragComponent;
        }
    }

    void OnPress(bool isPressed)
    {
        if (isDrag)
        {
            dragComponent.OnPress(isPressed);
        }
        else if (!isDrag)
        {
            OnDragComponetClick();
        }
        //if (isPressed)
        //{
        //    dragComponent.OnPress(isPressed);
        //}
        //else if(!isDrag)
        //{
        //    OnDragComponetClick();
        //}
        isDrag = isPressed;
    }

    void OnDrag(Vector2 delta)
    {
        isDrag = true;
        dragComponent.OnDrag(delta);
    }

    public virtual void OnDragComponetClick()
    {
    }
}
