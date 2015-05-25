using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SirenModelViewControl : MonoBehaviour 
{
    public enum AniType
    {
        Click,
    }

    public Transform ViewPoint;
    public FollowCamera FollowCamera;

    public Camera SirenCamera;

    public Vector3 CameraPos;

    private GameObject m_SirenScene;

    private Dictionary<int, GameObject> SirenDict = new Dictionary<int, GameObject>();
    private Vector3 vSirenScale = new Vector3(12, 12, 12);//模型缩放
    private int iCurSirenID = 0;
    private SirenConfigData dCurSirenConfig = new SirenConfigData();
    //private Dictionary<int, SirenConfigData> ConfigDict = new Dictionary<int, SirenConfigData>();

    private Rect m_CameraStandardRect;

	private ButtonCallBack m_CallBack;

	private GameObject m_markGameObj = null;

    void Start()
    {
        m_CameraStandardRect = SirenCamera.rect;
        //ResetCameraWH();
        m_SirenScene = GameObject.FindWithTag("SirenSceneOBJ");
        //transform.parent = null;
        //transform.transform.localScale = Vector3.one;
    }

    private void Show(int sirenID, SirenConfigData configData)
    {
        this.DisplaySiren();
        iCurSirenID = sirenID;
        dCurSirenConfig = configData;
        if (SirenDict.ContainsKey(sirenID))
        {
            SirenDict[sirenID].SetActive(true);
        }
        else
        {
            GameObject siren = (GameObject)Instantiate(configData._prefab);
            //siren.transform.parent = ViewPoint;
            siren.transform.position = configData._sirenPosition;
            //siren.transform.localScale = vSirenScale;
            SirenDict.Add(sirenID, siren);

			//点击事件
			var cb = siren.AddComponent<LocalButtonCallBack>();
			cb.SetCallBackFuntion(ClickSiren);
			var bc = siren.AddComponent<BoxCollider>();
			bc.center = new Vector3(0,10,0);
			bc.size = new Vector3(10,20,10);
			
			//挡板
			if(m_markGameObj!=null)
			{
				m_markGameObj = new GameObject();
				m_markGameObj.transform.position = configData._sirenPosition - Vector3.forward * 10;
				BoxCollider markBC = m_markGameObj.AddComponent<BoxCollider>();
				markBC.size = new Vector3(2000,2000,10);
			}

        }
        PlayAnimation(configData._defaultAnim);
        //如果未解锁，隐藏
        if (configData._growthLevels <= 0)
        {
            SirenDict[sirenID].SetActive(false);
        }
    }

	void ClickSiren(object obj)
	{
		m_CallBack(obj);
	}

	public void SetCallBack(ButtonCallBack callBack)
	{
		m_CallBack = callBack;
	}

    /// <summary>
    /// 显示妖女
    /// </summary>
    /// <param name="sirenID"></param>
    /// <param name="configData"></param>
    /// <param name="newCameraPos"></param>
    public void ShowSiren(int sirenID, SirenConfigData configData)
    {
        this.Show(sirenID, configData);
        CameraPos = configData._cameraPosition;
        FollowCamera.SetInitDistanceFromPlayer(SirenDict[sirenID].transform, CameraPos, true);
    }

    /// <summary>
    /// 更新妖女模型
    /// </summary>
    /// <param name="sirenID"></param>
    public void UpdateSiren(int sirenID, SirenConfigData configData)
    {
        Destroy(SirenDict[sirenID]);
        dCurSirenConfig = configData;
        GameObject siren = (GameObject)Instantiate(configData._prefab);
        //siren.transform.parent = ViewPoint;
        siren.transform.position = configData._sirenPosition;
        //siren.transform.localScale = vSirenScale;
        SirenDict[sirenID] = siren;       

		//增加点击事件
//		var cb = siren.AddComponent<LocalButtonCallBack>();
//		cb.SetCallBackFuntion(ClickSiren);
//		var bc = siren.AddComponent<BoxCollider>();
//		bc.center = new Vector3(0,10,0);
//		bc.size = new Vector3(10,20,10);


        CameraPos = configData._cameraPosition;
        FollowCamera.SetInitDistanceFromPlayer(SirenDict[sirenID].transform, CameraPos, true);
    }

    /// <summary>
    /// 隐藏妖女
    /// </summary>
    public void DisplaySiren()
    {
        SirenDict.Values.ApplyAllItem(p =>
        {
            if (p.activeInHierarchy)
            {
                p.SetActive(false);
            }
        });
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public bool PlayAnimation(string aniName)
    {
        bool isPlayingTouchAnimation = false;
        var animation = SirenDict[iCurSirenID].GetComponentInChildren<Animation>();
        var clip = animation.GetClip(aniName);
        if (clip != null)
        {
            if (!animation.IsPlaying(aniName))
            {
                animation.Stop();
                isPlayingTouchAnimation = true;
            }
            animation.PlayQueued(aniName);
            animation.PlayQueued(dCurSirenConfig._defaultAnim);
        }
        else
        {
            //TraceUtil.Log("[clip]" + aniName + " is null");
        }
        return isPlayingTouchAnimation;
    }

    //重置摄像机视口
    private void ResetCameraWH()
    {
        float YAONV_BACKGROUND_WIDTH = 595f;//妖女背景板宽度 (摄像机视口大小)
        float OFFSET_LEFT = 103f;//向左偏移量
        float width = Screen.width * 640f / Screen.height;
        float ratio = Screen.width * 1f / width;

        float picWidth = YAONV_BACKGROUND_WIDTH * ratio;
        float ofPicWidth = (YAONV_BACKGROUND_WIDTH / 2 + OFFSET_LEFT) * ratio;

        float viewX = (Screen.width / 2 - ofPicWidth) / Screen.width;
        float viewWidth = picWidth / Screen.width;

        Rect newRect = new Rect(viewX, m_CameraStandardRect.y, viewWidth, m_CameraStandardRect.height);
        SirenCamera.rect = newRect;
    }

    /// <summary>
    /// 设置妖女场景active
    /// </summary>
    /// <param name="isFlag">bool</param>
    public void SetSirenSceneActive(bool isFlag)
    {
        if (m_SirenScene != null)
        {
            m_SirenScene.SetActive(isFlag);
        }        
    }

    /// <summary>
    /// 振屏
    /// </summary>
    public void ShakeCamera()
    {
        SirenCamera.animation.Play();
    }
    /// <summary>
    /// 停止振屏
    /// </summary>
    public void StopShakeCamera()
    {
        SirenCamera.animation.Stop();
    }

    //[ContextMenu("SetTarget")]
    //public void SetTarget()
    //{
    //    FollowCamera.SetTarget(ViewPoint);
    //}

    //[ContextMenu("SetCameraPos")]
    //public void SetCameraPos()
    //{
    //    //Vector3 pos = ViewPoint.position + CameraPos;
    //    //FollowCamera.transform.position = pos;        
    //    //FollowCamera.transform.LookAt(ViewPoint);
    //    FollowCamera.SetInitDistanceFromPlayer(ViewPoint, CameraPos);
    //}
}


