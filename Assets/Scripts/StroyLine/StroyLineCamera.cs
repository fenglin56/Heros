using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StroyLineCamera : MonoBehaviour {

	// Use this for initialization
    private Vector3 m_transPosition = Vector3.zero;
    private Vector3 m_targetPosition = Vector3.zero;
    private Quaternion m_transRotation = Quaternion.identity;
    //private bool m_isInitCameraPos;

    private List<StroyCameraConfigData> m_cameraData = new List<StroyCameraConfigData>();
    private bool m_isAction = false;
	private bool isStartCameraMask = false;
    private float m_elapseTime;
    private int index = 0;
    private bool m_isSnapShot = false;
    private GameObject m_cameraMask;
	private float[] m_position = new float[3];
	
	void Start () {
        StroyLineManager.Instance.AddCameraDelegate(UpdateCameraAction);
        //transform.localPosition = m_transPosition = Vector3.zero;
        //transform.localRotation = m_transRotation = Quaternion.identity;
        //m_targetPosition = Vector3.zero;
        //transform.LookAt(m_targetPosition);
	}
    void Update()
    {
        float timeDelta = Time.deltaTime;

        if (m_isAction)
        {
            m_elapseTime += timeDelta;
            
            if (index < m_cameraData.Count)
            {
                StroyCameraConfigData item = m_cameraData[index];
				//当镜头组最后一个镜头//
				if(isStartCameraMask && index == m_cameraData.Count-1)
				{
					isStartCameraMask = false;
					StroyLineManager.Instance.EndCameraMask ();
				}
                if (item._Params.Length < 3)
                {
                    TraceUtil.Log("相机运动参数配置错误！");
                    return;
                }

                if (item._ActionTime == 0)
                {
                    for (int i = 0; i < item._Params.Length; i++)
                    {
                        float a = item._Params[i]._EquA;
                        float b = item._Params[i]._EquB;
                        float c = item._Params[i]._EquC;
                        float d = item._Params[i]._EquD;

                        SnapShotCamera();
                        m_position[i] = GetPositionValue(a, b, c, d, m_elapseTime);
                    }
                    transform.localPosition = m_transPosition + new Vector3(m_position[0], m_position[1], m_position[2]);

                    SetTarget(item);
                }



                if (item._ActionTime/1000 >= m_elapseTime)
                {
                    for (int i = 0; i < item._Params.Length; i++)
                    {
                        float a = item._Params[i]._EquA;
                        float b = item._Params[i]._EquB;
                        float c = item._Params[i]._EquC;
                        float d = item._Params[i]._EquD;

                        SnapShotCamera();

                        if(item._MoveMode == 1)  //平移
                        {
                            m_position[i] = GetPositionValue(a, b, c, d, m_elapseTime);
                        }
                        else if(item._MoveMode == 2) //旋转
                        {
                            Vector3 offset = transform.position - m_targetPosition;
                            m_position[i] = GetRotateSpeed(a, b, Time.deltaTime, i, offset);
                        }

                    }
                    if(item._MoveMode == 1)  //平移
                    {
                        transform.localPosition = m_transPosition + new Vector3(m_position[0], m_position[1], m_position[2]);
                    }
                    else if(item._MoveMode == 2) //旋转
                    {
                        transform.position = m_targetPosition + new Vector3(m_position[0], m_position[1], m_position[2]);
                    }
					SetTarget(item);
                }
                else
                {
                    m_transPosition = transform.localPosition;
                    m_transRotation = transform.localRotation;
                    index += 1;
                    m_elapseTime = 0;
					//SetTarget(item);
                }
            }
            else
            {
                m_isAction = false;
                m_elapseTime = 0;
            }
        }
    }

    /// <summary>
    /// 获取相机旋转速度
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private float GetRotateSpeed(float a, float b, float t, int index, Vector3 offset)
    {
        var th = b * t * (Mathf.PI / 180);
        switch (index)
        {
            case 0:
                return (offset.x * Mathf.Cos(th) - offset.z * Mathf.Sin(th));
            case 1:
                return GetParamValue(a, b, t) * t + offset.y;
            case 2:
                return (offset.x * Mathf.Sin(th) + offset.z * Mathf.Cos(th));
            default:
                return 0;
        }
        
    }

    /// <summary>
    /// Gets the parameter value.
    /// </summary>
    /// <returns>The parameter value.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    /// <param name="t">T.</param>
    private float GetParamValue(float a, float b, float t)
    {
        return a * t + b;
    }

    void OnDestroy()
    {
        if (StroyLineManager.Instance != null)
            StroyLineManager.Instance.RemoveCameraDelegate(UpdateCameraAction);
    }

    public float GetPositionValue(float a, float b, float c, float d, float t)
    {
        return a * Mathf.Pow(t, 2) + b * t + c * Mathf.Sqrt(t) + d;
    }

    public void UpdateCameraAction(List<StroyCameraConfigData> cameraData)
    {
        if (cameraData.Equals(m_cameraData))
        {
            transform.localPosition = m_transPosition;
            transform.localRotation = m_transRotation;
        }
        else
        {
            m_cameraData.Clear();
            m_cameraData = cameraData;
            m_transPosition = Vector3.zero;
            m_transRotation = Quaternion.identity;
            transform.localPosition = m_transPosition;
            transform.localRotation = m_transRotation;
        }

        m_isSnapShot = true;
		isStartCameraMask = true;
        m_isAction = true;
        m_elapseTime = 0;
        index = 0;
    }

    //UnityEditor.UndoSnapshot
    void SnapShotCamera()
    {
        if (m_isSnapShot)
        {
//            m_transPosition = transform.localPosition;
//            m_transRotation = transform.localRotation;
//            m_isSnapShot = false;
        }
    }


    public void SetTarget(StroyCameraConfigData item)
    {
        switch (item._TargetType)
        {
            case 1:
                if (!StroyLineDataManager.Instance.GetNpcList.ContainsKey(item._TargetID))
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"指定ID为" + item._TargetID + "NPC尚未创建...");
                    return;
                }
                m_targetPosition = StroyLineDataManager.Instance.GetNpcList[item._TargetID].transform.position +item._TargetOffset;
                break;
            case 2:
                m_targetPosition = new Vector3(item._TargetPos.x, 0, item._TargetPos.y);
                break;
            default:
                break;
        }

        transform.LookAt(m_targetPosition);
    }
}
