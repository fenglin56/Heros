using UnityEngine;
using System.IO;
using System.Collections;

public class BillbroadScript : MonoBehaviour
{

    public enum BILLBROADTYPE 
    {
        LOOKATCAMERA,
        LOOKATY,
    }

    public BILLBROADTYPE m_billboradType = BILLBROADTYPE.LOOKATCAMERA; 
    // Use this for initialization

    // Update is called once per frame
    void Update()
    {
        if (Camera.main == null)
            return;

        float t = Camera.main.transform.rotation.eulerAngles.y;

        if (m_billboradType == BILLBROADTYPE.LOOKATY)
        {
            transform.transform.rotation = Quaternion.EulerAngles(0,(t/180*Mathf.PI) , 0);
        }
        else
        {
            transform.transform.rotation = Camera.main.transform.rotation;
        }

       
    }
}
