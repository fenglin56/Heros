using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DownLoadUIController : MonoBehaviour {

	public string m_versionDownMessageText;

	public UISlider m_progressBar;
	public UILabel m_downLoadText;

    public UILabel m_downLoadProgressText;
    public UILabel m_downLoadTipText;
	
	
	private int m_downLoadBundleCount;
	private float m_totalSize;

    private float m_currentDownLoadSpeed;

    private float m_loadBundleCount;


    public GameObject m_downLoadMsgObj;

    public SimpleButtonCallBack m_downLoadButton;
    public SimpleButtonCallBack m_cancelDownLoadButton;

    public UILabel m_downLoadMsgMainText;

	//joke
	public GameObject jokePanel;
	public UILabel jokeTipLabel;
	public UILabel loadTipLabel;

	public GameObject LogoPanel;
	
	void Awake()
	{
		LinkActions();
		StartJoke ();
		jokePanel.SetActive (true);
		m_progressBar.gameObject.SetActive(false);
		m_downLoadText.gameObject.SetActive(false);
		m_progressBar.gameObject.SetActive(false);

        m_downLoadButton.AddClickDelegate(OnDownButtonClick);
        m_cancelDownLoadButton.AddClickDelegate(OnCancelButtonClick);

        m_downLoadMsgObj.SetActive(false);
	}

    void OnDownButtonClick()
    {
        AppManager.Instance.LaunchAppUpdate();
    }

    void OnCancelButtonClick()
    {
        Application.Quit();
    }
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void CheckVersion()
	{
		m_progressBar.gameObject.SetActive(false);
		m_downLoadText.gameObject.SetActive(true);
		m_progressBar.sliderValue = 0.0f;
		m_downLoadText.SetText("");
        m_downLoadProgressText.SetText("");
		m_downLoadTipText.SetText("");
	}
	
	public void DownLoadStart(AssetBundlesDatabase dataBase, List<AssetBundlesDatabase.BundleData> bundleList, int totalSize)
	{
		m_progressBar.gameObject.SetActive(true);
		m_downLoadText.gameObject.SetActive(true);
		m_progressBar.sliderValue = 0.0f;
		m_downLoadText.SetText("");
		m_downLoadBundleCount = bundleList.Count;
		m_totalSize = ((float)totalSize)/(1024.0f);	
        m_downLoadProgressText.SetText("");

		ShowLoadTip (true);
	}
	
	public void DownLoadProgress(int bundleCounter, int totalDownloadedBytes)
	{
		float downloadedSize = ((float)totalDownloadedBytes)/(1024.0f);
		
		m_progressBar.gameObject.SetActive(true);
		m_downLoadText.gameObject.SetActive(true);
		m_progressBar.sliderValue = downloadedSize/m_totalSize;
        m_downLoadText.SetText("DownLoading");
        string strDownLoadProgress = ((int)downloadedSize).ToString() + "/" + ((int)m_totalSize).ToString() + "KB";
        m_downLoadProgressText.SetText("strDownLoadProgress");
		
	}
	public void DownloadingFinish()
	{
		ShowLoadTip (false);
	}
	public void LoadStart(AssetBundlesDatabase database)
	{
		m_progressBar.gameObject.SetActive(true);
		m_downLoadText.gameObject.SetActive(true);
		m_progressBar.sliderValue = 0.0f;
		m_downLoadText.SetText("Loading");
        m_downLoadProgressText.SetText("");
        m_loadBundleCount = database.Bundles.Count;

		ShowLoadTip (false);
	}
	
	public void LoadProgress(int bondleCounter)
	{
        m_progressBar.sliderValue = (float)bondleCounter/ (float)m_loadBundleCount;
		
	}
	
	public void LoadEnd()
	{
		m_progressBar.gameObject.SetActive(false);
		m_downLoadText.gameObject.SetActive(false);
		ClearActions();
	}

    public void AppUpdate(string appVersion, bool force)
    {
        m_downLoadMsgObj.SetActive(true);
		string msg = string.Format(m_versionDownMessageText, appVersion);
		m_downLoadMsgMainText.SetText(msg);

    }

	public void DownLoadVersionEnd(string result)
	{
		if("fail" == result)
		{


		}

	}

	public void CloseLogoPanel()
	{
		LogoPanel.SetActive(false);
	}
	
	public void LinkActions()
	{
		AppManager.Instance.CloseLogoPanel += CloseLogoPanel;

		AppManager.Instance.CheckingVersion += CheckVersion;	
		AppManager.Instance.DownloadStart += DownLoadStart;
		AppManager.Instance.DownloadProgress += DownLoadProgress;
		AppManager.Instance.DownloadingFinish += DownloadingFinish;
		AppManager.Instance.LoadStart += LoadStart;
		AppManager.Instance.LoadProgress += LoadProgress;
		AppManager.Instance.LoadingFinish += LoadEnd;

        AppManager.Instance.AppUpdate += AppUpdate;
		AppManager.Instance.DownloadingVersionEnd += DownLoadVersionEnd;
	}


	
	public void ClearActions()
	{
		AppManager.Instance.CloseLogoPanel -= CloseLogoPanel;

		AppManager.Instance.CheckingVersion -= CheckVersion;	
		AppManager.Instance.DownloadStart -= DownLoadStart;
		AppManager.Instance.DownloadProgress -= DownLoadProgress;
		AppManager.Instance.LoadStart -= LoadStart;
		AppManager.Instance.LoadProgress -= LoadProgress;
		AppManager.Instance.LoadingFinish -= LoadEnd;

        AppManager.Instance.AppUpdate -= AppUpdate;
		AppManager.Instance.DownloadingVersionEnd -= DownLoadVersionEnd;
	}

	#region joke
	public JokeLanguageDataBase jokeLanguageDataBase;
	private Dictionary<string, JokeLanguageData> LanguageTextDataList = new Dictionary<string, JokeLanguageData>();
	public JokeConfigDataBase jokeConfig;
	private Dictionary<int,JokeConfigData> jokeConfigMap = new Dictionary<int, JokeConfigData> ();
	private List<int> leftJokeList = new List<int> ();
	private List<int> alreadyJokedList = new List<int> ();
	private bool isRead = false;
	void InitJoke()
	{
		if (isRead)
			return;
		isRead = true;
		InitLanguageConfigData ();
		InitJokeConfig ();
	}
	void InitLanguageConfigData()
	{
		foreach (JokeLanguageData child in jokeLanguageDataBase.stringTable)
		{
			if (LanguageTextDataList.ContainsKey(child.key))
			{
				TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"相同ID：" + child.key);
			}
			else
			{
				LanguageTextDataList.Add(child.key, child);
			}
		}
	}
	void InitJokeConfig()
	{
		foreach (JokeConfigData data in jokeConfig.JokeDataList) {
			jokeConfigMap.Add(data.ID,data);
			leftJokeList.Add(data.ID);
		}
	}
	public string GetString(string key)
	{
		string result = "null string, id:" + key;
		if(!string.IsNullOrEmpty(key) && LanguageTextDataList.ContainsKey(key))
		{
			result = LanguageTextDataList[key].text;
		}
		
		////TraceUtil.Log("GetString " + result);
		return result;
	}

	private void StartJoke()
	{
		InitJoke ();
		ShowJokeTip ();
	}
	public void ShowLoadTip(bool isDownLoad)
	{
		InitJoke ();
		if (isDownLoad) {
			loadTipLabel.text = GetString("IDS_System_2");
				} else {
			loadTipLabel.text = GetString("IDS_System_1");
		}
	}
	//开始显示笑话//
	private void ShowJokeTip()
	{
		JokeConfigData joke = SelectJoke ();
		if (joke == null)
			return;
		jokeTipLabel.text = GetString (joke.IDS);
		if (IsInvoking ("ShowJokeTip")) {
			CancelInvoke("ShowJokeTip");		
		}
		Invoke ("ShowJokeTip",joke.DelayTime);
	}
	private JokeConfigData SelectJoke()
	{
		int index = RandJoke ();
		int id = leftJokeList [index];
		alreadyJokedList.Add (id);
		leftJokeList.RemoveAt (index);
		if (alreadyJokedList.Count > 5) {
			leftJokeList.Add(alreadyJokedList[0]);
			alreadyJokedList.RemoveAt(0);		
		}
		return jokeConfigMap[id];
	}
	private int RandJoke()
	{
		return Random.Range (0,leftJokeList.Count);
	}
	private void OnDestroy()
	{
		if (IsInvoking ("ShowJokeTip")) {
			CancelInvoke("ShowJokeTip");		
		}
	}
	#endregion
}
