using UnityEngine;
using System.Collections;

public class ToolBarChild : MonoBehaviour {

    public GameObject ToolButton;
    ToolBar toobar;
    UISprite MySprite;
    public int ButtonID = 0;

    void Awake()
    {
        MySprite = GetComponent<UISprite>();
        toobar = transform.parent.GetComponent<ToolBar>();
        toobar.toolBarChildList.Add(this);
    }

    void OnClick()
    {
        toobar.OnBtnClick(ButtonID);
    }

    public void EnableMyself()
    {
        MySprite.color = Color.white;
    }

    public void DisableMyself()
    {
        MySprite.color = Color.gray;
    }

}
