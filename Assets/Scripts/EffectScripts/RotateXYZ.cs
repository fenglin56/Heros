using UnityEngine;
using System.Collections;

public class RotateXYZ : MonoBehaviour
{

    public float XSpeed = 0f;
    public float YSpeed = 0f;
    public float ZSpeed = 0f;

    // Use this for initialization
    void Start()
    {
//        basex = transform.localRotation.eulerAngles.x;
//        basey = transform.localRotation.eulerAngles.y;
//        basez = transform.localRotation.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        // transform.localRotation = Quaternion.EulerAngles(new Vector3((Time.deltaTime * XSpeed) + basex, (Time.deltaTime * YSpeed) + basey, (Time.deltaTime * ZSpeed) + basez));
        transform.Rotate(new Vector3((Time.deltaTime * XSpeed), (Time.deltaTime * YSpeed), (Time.deltaTime * ZSpeed)));
        //transform.Rotate(new Vector3((Time.deltaTime * XSpeed)  , (Time.deltaTime * YSpeed)  , (Time.deltaTime * ZSpeed)  ));
    }
}
