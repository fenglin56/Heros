using UnityEngine;
using System.Collections;

public class SkillUIDragBox : DragComponent
{

	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// 移动到新的地方了
    /// </summary>
    public override void MoveToNewPointComplete()
    {
        //TraceUtil.Log("#############+MoveToNewPointComplete");
    }
    /// <summary>
    /// 点击事件，替换原有的OnClick
    /// </summary>
    public override void OnDragComponetClick()
    {
        
    }

    
}
