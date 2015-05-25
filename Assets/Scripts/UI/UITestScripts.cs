using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class UITestScripts:MonoBehaviour
{
    /// <summary>
    /// 测试UI模块功能代码，无他用
    /// </summary>

    void Start()
    {
        TraceUtil.Log(Mathf.FloorToInt(1.1f));
        TraceUtil.Log(Mathf.FloorToInt(1.4f));
        TraceUtil.Log(Mathf.FloorToInt(1.6f));
        TraceUtil.Log(Mathf.FloorToInt(1.8f));
        TraceUtil.Log(Mathf.FloorToInt(1.9f));
        TraceUtil.Log(Mathf.FloorToInt(2.1f));
        TraceUtil.Log(Mathf.FloorToInt(2.3f));
        TraceUtil.Log(Mathf.FloorToInt(2.6f));
        TraceUtil.Log(Mathf.FloorToInt(2.8f));
    }

    //GameObject FindObj(Transform point)
    //{
    //}

}
