using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillCamera : MonoBehaviour {

    private List<SkillCameraData> m_cameraData = new List<SkillCameraData>();
    private List<SkillAnim> m_skillAnim = new List<SkillAnim>();
    private float m_elapseTime = 0;
    private float m_elapse = 0;
    private int m_index = 0;
    private int m_animIndex = 0;
    private Vector3 m_offset = Vector3.zero;
    private Transform m_playerTrans;
    private float[] m_speed = new float[3];
    private Vector3 m_overPosition = Vector3.zero;

    private bool m_isPlay = false;
    private bool m_isPause = false;

    public struct SkillAnim
    {
        public float PlayTime;
        public string AnimClipName;
        public bool IsPlayed;
    }

    /// <summary>
    /// 初始化技能镜头数据
    /// </summary>
    /// <param name="cameraId"></param>
    public void InitCameraData(int[] cameraId)
    {
        if (m_isPlay)
        {
            m_isPause = true;
            m_elapseTime = 0;
            m_index = 0;
			m_animIndex = 0;
        }
        else
        {
            m_isPlay = true;
            name = "SkillCamera";
        }

        m_cameraData.Clear();
	
        for (int i = 0; i < cameraId.Length; i++)
        {
            var cameraData = SkillDataManager.Instance.GetSkillCameraData(cameraId[i]);
            if (cameraData != null)
                m_cameraData.Add(cameraData);
        }

        m_playerTrans = ((PlayerBehaviour)PlayerManager.Instance.FindHeroEntityModel().Behaviour).ThisTransform;
        
        if (m_cameraData.Count > 0)
        {
            //不等于零的时候初始化镜头位置，否则等于场景中相机位置
            InitCameraPosition(m_cameraData[0]);

            if (m_cameraData[0]._ShakeStartTime.Length != m_cameraData[0]._ShakeAnimName.Length)
                return;

            GetCameraAnimList(m_cameraData[0]);
        }

        m_offset = transform.position - m_playerTrans.position;
		m_isPause = false;
    }


    /// <summary>
    /// 初始化技能镜头世界坐标位置
    /// </summary>
    /// <param name="cameraOffset"></param>
    void InitCameraPosition(SkillCameraData cameraData)
    {
        var cameraOffset = cameraData._CameraOffset;

        if (cameraOffset != Vector3.zero)
        {
            Vector3 offset = Vector3.zero;
            if (cameraData._StartType == 0)   //如果初始类型为0时，为偏移位置
            {
                offset.x = m_playerTrans.position.x + cameraOffset.x *
                            m_playerTrans.forward.x - cameraOffset.z * m_playerTrans.forward.z;
                offset.z = m_playerTrans.position.z + cameraOffset.x *
                            m_playerTrans.forward.z + cameraOffset.z * m_playerTrans.forward.x;
                offset.y = cameraOffset.y;
            }
            else  //如果初始类型为1时，为偏移位置
            {
                offset = cameraData._CameraOffset;
            }
            transform.position = offset;
        }
    }



    /// <summary>
    /// 循环体
    /// </summary>
    private void FixedUpdate()
    {
		if(m_playerTrans==null)
		{
			return;
		}
        if (m_isPause) return;

        float timeDelta = Time.fixedDeltaTime;

        if (m_isPlay)
        {
            m_elapseTime += timeDelta;

            if (m_index < m_cameraData.Count)
            {
                var cameraData = m_cameraData[m_index];
                if (m_elapseTime <= cameraData._CameraDuration * 0.001f)
                {

                    //播放震屏动画
                    if (m_animIndex < m_skillAnim.Count)
                    {
                        var item = m_skillAnim[m_animIndex];
                        if (item.PlayTime <= m_elapseTime && !item.IsPlayed)
                        {
                            animation.CrossFade(item.AnimClipName);
                            if (animation[item.AnimClipName].length > m_elapseTime - item.PlayTime)
                            {
                                item.IsPlayed = true;
                                m_animIndex++;
                            }
                        }
                    }
                    //震屏动画结束

                    //相机动画
                    var cameraParam = cameraData._CameraParams;

					if (m_cameraData[m_index]._CameraType == 1)  //移动
					{
						GetMoveSpeed(cameraParam,m_elapseTime);
						transform.position = new Vector3(m_speed[0] * Time.deltaTime + transform.position.x, 
						                                 m_speed[1] * Time.deltaTime + transform.position.y,
						                                 m_speed[2] * Time.deltaTime + transform.position.z);
					}
					else
					{
						for (int i = 0; i < cameraParam.Length; i++)
						{
							float a = cameraParam[i]._EquA;
							float b = cameraParam[i]._EquB;
							
							m_speed[i] = GetRotateSpeed(a, b, m_elapseTime, i);
						}
                        if (m_playerTrans != null)
                        {
                            transform.position = m_playerTrans.position + new Vector3(m_speed[0], m_speed[1], m_speed[2]);         //更新相机旋转位置
                        }
					}
                    if (m_playerTrans != null)
                    {
                        transform.LookAt(m_playerTrans.position + LookAtPoint(transform.position, m_playerTrans.position, cameraData._TargetOffset));  //更新相机的观察方向
                    }
                }
                else
                {
                    m_index += 1;
                    m_elapseTime = 0;

                    if (m_index < m_cameraData.Count)
                    {
                        InitCameraPosition(m_cameraData[m_index]);
                        GetCameraAnimList(m_cameraData[m_index]);
                    }
                    m_animIndex = 0;

                    m_offset = transform.position - m_playerTrans.position;
                    m_overPosition = transform.position;
                }
            }
            else
            {
                
                //相机恢复动画
                m_elapse += Time.deltaTime ;

                if (m_index == 0)
                {
                    OnDestroy();
                    return;
                }

                float resetTime = m_cameraData[m_index - 1]._CameraResetTime == 0 ? 1 : m_elapse / (m_cameraData[m_index - 1]._CameraResetTime * 0.001f);
                transform.position = Vector3.Lerp(m_overPosition, BattleManager.Instance.FollowCamera.transform.position, resetTime);
                transform.LookAt(m_playerTrans);


                if (resetTime >= 1.0f)
                {
                    m_elapse = 0;
					m_elapseTime = 0;
                    m_index = 0;
                    m_isPlay = false;
                    OnDestroy();
                }
            }
        }
    }

    //获取相机动画列表
    private void GetCameraAnimList(SkillCameraData cameraData)
    {
        m_skillAnim.Clear();

        if (cameraData._ShakeAnimName.Length == 1 && cameraData._ShakeAnimName[0] == "0")
            return;

        SkillAnim anim = new SkillAnim();
               
        for (int i = 0; i < cameraData._ShakeStartTime.Length; i++)
        {
            anim.PlayTime = cameraData._ShakeStartTime[i] * 0.001f;
            anim.AnimClipName = cameraData._ShakeAnimName[i];
            anim.IsPlayed = false;
            m_skillAnim.Add(anim);
        }
    }

    /// <summary>
    /// 计算观察点
    /// </summary>
    /// <param name="cameraPos">镜头世界坐标的位置</param>
    /// <param name="viewPoint">观察点世界坐标的位置</param>
    /// <param name="targetOffset">配置观察点偏移</param>
    /// <returns>最终观察点</returns>
    private Vector3 LookAtPoint(Vector3 cameraPos, Vector3 viewPoint, Vector3 targetOffset)
    {
        var xj = viewPoint.x - cameraPos.x;
        var zj = viewPoint.z - cameraPos.z;

		var sqrtA = Mathf.Sqrt(Mathf.Pow(xj, 2) + Mathf.Pow(zj, 2));
		var iax = (sqrtA == 0) ? 0 : xj / sqrtA;  //角度
		var iaz = (sqrtA == 0) ? 0 : zj / sqrtA;  //角度

        var sqrt = Mathf.Sqrt(Mathf.Pow(targetOffset.x, 2) + Mathf.Pow(targetOffset.z, 2));
        var iox = (sqrt == 0) ? 0 : targetOffset.x / sqrt;  //偏移
		var ioz = (sqrt == 0) ? 0 : targetOffset.z / sqrt;  //偏移

        float finalX = sqrt * (iox * iax - ioz * iaz);
        float finalZ = sqrt * (iaz * iox + iax * ioz);
        float finalY = targetOffset.y;

        return new Vector3(finalX, finalY, finalZ);
    }


    private float GetParamValue(float a, float b, float t)
    {
        return a * t + b;
    }

    /// <summary>
    /// 获取相机位移速度
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    /// <returns></returns>
	private void GetMoveSpeed(SkillCameraParam[] cameraParam, float time)
    {
		var xSpeed = GetParamValue(cameraParam[0]._EquA,cameraParam[0]._EquB,time );
		var ySpeed = GetParamValue(cameraParam[1]._EquA,cameraParam[1]._EquB,time );
		var zSpeed = GetParamValue(cameraParam[2]._EquA,cameraParam[2]._EquB,time );
		
		m_speed[0] = xSpeed * m_playerTrans.forward.x - zSpeed * m_playerTrans.forward.z;
		m_speed[1] = ySpeed;
		m_speed[2] = xSpeed * m_playerTrans.forward.z + zSpeed * m_playerTrans.forward.x;
    }

    /// <summary>
    /// 获取相机旋转速度
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    private float GetRotateSpeed(float a, float b, float t, int index)
    {
        var th = b * t * (Mathf.PI / 180);
        switch (index)
        {
            case 0:
                return (m_offset.x * Mathf.Cos(th) - m_offset.z * Mathf.Sin(th));
            case 1:
                return GetParamValue(a, b, t) * t + m_offset.y;
            case 2:
                return (m_offset.x * Mathf.Sin(th) + m_offset.z * Mathf.Cos(th));
            default:
                return 0;
        }

    }

    public void OnDestroy()
    {
		if(BattleManager.Instance != null)
        {
            if(null != BattleManager.Instance.FollowCamera)
            {
        	    //BattleManager.Instance.FollowCamera.gameObject.SetActive(true);
				BattleManager.Instance.FollowCamera.camera.enabled = true;
            }
        }
        Destroy(this.gameObject);
    }

}
