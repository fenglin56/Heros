using UnityEngine;
using System.Collections;

public class TipsManager : MonoBehaviour {

    private static TipsManager m_instance;
    public static TipsManager Instance { get { return m_instance; } }

    public GameObject tipsPrefab;
    private GameObject tipsObj;
    private UILabel TipsLable;

    private float LifeCycle =2;

    void Awake()
    {
        m_instance = this;
    }

    void OnDestroy()
    {
        m_instance = null;
    }

    public void Show(string msg)
    {
        if (TipsLable == null)
        {
            tipsObj = UI.CreatObjectToNGUI.InstantiateObj(tipsPrefab, transform);
            TipsLable = tipsObj.GetComponent<UILabel>();
        }
        else
        {
            StopAllCoroutines();
        }
        TipsLable.text = msg;
        StartCoroutine(DestoryObj());
    }

    IEnumerator DestoryObj()
    {
        yield return new WaitForSeconds(LifeCycle);
        if (tipsObj != null)
        {
            Destroy(tipsObj);
        }
    }

}
