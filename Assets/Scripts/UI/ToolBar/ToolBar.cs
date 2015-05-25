using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolBar : MonoBehaviour {

    public GameObject ToolButton;
    public List<ToolBarChild> toolBarChildList = new List<ToolBarChild>();
     
    int ActiveBtnID = 0;


    public void OnBtnClick(int BtnID)
    {
        this.ActiveBtnID = BtnID;
        foreach (ToolBarChild child in toolBarChildList)
        {
            if (child.ButtonID != BtnID)
            {
                child.DisableMyself();
            }
            else
            {
                child.EnableMyself();
            }
        }
    }



}
