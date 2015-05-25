using UnityEngine;
using System.Collections;
using UI;
using System.Linq;

public class EnableFunctionPanel : MonoBehaviour {

    public LocalButtonCallBack CloseButton;
    public GameObject EnableFunctionEffect;
    //public GameObject EnableSysEffect;
    private GameObject m_enableButton;
    private GameObject m_funcEffect;
    //private GameObject m_sysEffect;

    //private bool m_flag;

    public SystemFuntionButton MyParent { get; private set; }

    void Start()
    {
        CloseButton.SetCallBackFuntion(ClosePanel);
    }

    void ClosePanel(object obj)
    {
        //Destroy(this.gameObject);
        //StartCoroutine(ScaleSelf());
    }

    public void InitPanel(UI.MainUI.UIType func, Transform trans,SystemFuntionButton myParent)
    {
        MyParent = myParent;
        if (m_funcEffect != null)
            DestroyImmediate(m_funcEffect);

        m_funcEffect = CreatObjectToNGUI.Instantiate(EnableFunctionEffect) as GameObject;
        m_funcEffect.transform.parent = this.transform;
        m_funcEffect.transform.localScale = Vector3.one;
       
        var parent = m_funcEffect.GetComponent<EnableFuncEffect>().parentTrans;
        CloneEnableFunc(parent, func);
		TaskModel.Instance.isNewFunctionEffing = true;
        //if(m_sysEffect != null)
        //    DestroyImmediate(m_sysEffect);

        //m_sysEffect = CreatObjectToNGUI.Instantiate(EnableSysEffect) as GameObject;
        //m_sysEffect.transform.parent = trans;
        //m_sysEffect.transform.localPosition = Vector3.zero;
        //m_sysEffect.transform.localScale = Vector3.one;

        TraceUtil.Log("开放新功能特效开始播放："+Time.realtimeSinceStartup);
    }

    private void CloneEnableFunc(Transform parent, UI.MainUI.UIType func)
    {
        if (m_enableButton != null)
            DestroyImmediate(m_enableButton);

        var buttonPrefab = NewUIDataManager.Instance.TownMainButtonList.SingleOrDefault(P => P.ButtonFunc == func).ButtonPrefab;

        if (buttonPrefab != null)
        {
            m_enableButton = Instantiate(buttonPrefab) as GameObject;
            m_enableButton.GetComponent<BoxCollider>().enabled = false;
            m_enableButton.transform.parent = parent;
            m_enableButton.transform.localScale = new Vector3(0.006f, 0.006f, 1);
            m_enableButton.transform.localPosition = new Vector3(0, 0, 0);
            m_enableButton.layer = LayerMask.NameToLayer("PopUp");
        }
    }

    //public void InitPanel(UIType func, Transform trans, Camera uiCamera)
    //{
    //    if (m_enableButton != null)
    //        Destroy(m_enableButton);

    //    var buttonPrefab = UIDataManager.Instance.TownMainButtonList.SingleOrDefault(P=>P._ButtonFunc == func)._ButtonPrefab;

    //    if (buttonPrefab != null)
    //    {
    //        m_enableButton = Instantiate(buttonPrefab) as GameObject;
    //        m_enableButton.GetComponent<BoxCollider>().enabled = false;
    //        m_enableButton.transform.parent = this.transform;
    //        m_enableButton.transform.localScale = Vector3.one;
    //        m_enableButton.transform.localPosition = new Vector3(-18, -78, -300);
    //        m_enableButton.layer = 25;
    //    }

    //    MoveAnim(trans, uiCamera);
    //}

    //void MoveAnim(Transform trans, Camera uiCamera)
    //{
    //    var srcTransPos = uiCamera.ViewportToWorldPoint(m_enableButton.transform.position);
    //    var targetTransPos = uiCamera.ViewportToWorldPoint(trans.position);
    //    targetTransPos.x *= 0.5f;
    //    srcTransPos.z = targetTransPos.z = -50f;

    //    m_direction = targetTransPos - srcTransPos;
    //    m_length = Vector3.Distance(srcTransPos, targetTransPos) * 0.5f - 0.1f;
    //}

    //float m_moveElapseTime = 0;
    //float m_speed = 1f;
    //float m_length = 0;
    //Vector3 m_direction = Vector3.zero;

    //// Update is called once per frame
    //void Update()
    //{
    //    float timeDelta = Time.deltaTime;
    //    m_moveElapseTime += timeDelta;

    //    if (m_moveElapseTime * m_speed < m_length)
    //    {
    //        var m_moveTime = m_speed * timeDelta;
    //        m_enableButton.transform.Translate(m_direction.normalized * m_moveTime);
    //    }
    //    else
    //    {
    //        DestroyImmediate(m_enableButton);
    //    }
    //}

    IEnumerator ScaleSelf()
    {
        yield return new WaitForSeconds(1.5f);

        //var scale = this.transform.localScale;
        //this.transform.localScale = scale * 0.001f;
    }
	void OnDestroy()
	{
		TaskModel.Instance.isNewFunctionEffing = false;
	}
}
