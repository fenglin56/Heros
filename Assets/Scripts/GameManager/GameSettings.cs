using UnityEngine;
using System.Collections;
/// <summary>
/// EnterPoint Scene GameManager  游戏设置
/// </summary>
public class GameSettings : MonoBehaviour {
	
	private float m_bgmVolume;
	private float m_sfxVolume;
	private int m_gameViewLevel;
	private int m_doubleClickSkill;
	private int m_rightHandControl;
	private int m_joyStickMode;

    private int m_showHurtNum;
	
    public bool ShowHurtNum
    {
        get
        {
            return m_showHurtNum==1;
        }
        set
        {
        m_showHurtNum=value?1:0;
        }
    }

	public float BgmVolume
	{
		get { return m_bgmVolume; }
		set 
		{ 
			m_bgmVolume = value;
			SoundManager.Instance.SetBgmVolume(m_bgmVolume);
            
		}
	}
	
	public float SfxVolume
	{
		get { return m_sfxVolume; }	
		set
		{
			m_sfxVolume = value;	
			SoundManager.Instance.SetSfxVolume(m_sfxVolume);
            GameManager.Instance.SenceAudioFactory.SetSfxVolume(m_sfxVolume);
		}
	}
	
	public int GameViewLevel
	{
		get { return m_gameViewLevel;}
		set 
		{
			m_gameViewLevel = value;
            
		}
	}
	
	public bool DoubleClickSkill
	{
		get {
            if (GameManager.Instance.IsLockOperatorMode)
                return false;
            else
                return (m_doubleClickSkill == 1); }
		set 
		{
			m_doubleClickSkill = value? 1:0;
            
		}
	}
	
	public bool RightHandControl
	{
		get { return ( m_rightHandControl == 1 ); }
		set
		{
			m_rightHandControl = value? 1:0;	
            
		}
	}
	
	public bool JoyStickMode
	{
		get {

            if (GameManager.Instance.IsLockOperatorMode)
                return false;
            else
                return (m_joyStickMode == 1); }	
		set
		{

			m_joyStickMode = value? 1:0;	
		}
	}
	
	
	
	
	
	void Awake()
	{
		Load();
	}
	
	void Load()
	{
		BgmVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 1.0f);
		SfxVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 1.0f);
		m_gameViewLevel = PlayerPrefs.GetInt("GAME_VIEW_LEVEL", 2);
		m_doubleClickSkill = PlayerPrefs.GetInt("DOUBLE_CLICK_SKILL", 0);
		m_rightHandControl = PlayerPrefs.GetInt("RIGHT_HAND_CONTROL", 0);
		m_joyStickMode = PlayerPrefs.GetInt("JOY_STICK_MODE", 1);//默认为摇杆
        m_showHurtNum=PlayerPrefs.GetInt("SHOW_HURTNUM",1);
	}
	
	
	public void Save()
	{
		PlayerPrefs.SetFloat("BGM_VOLUME", m_bgmVolume);
		PlayerPrefs.SetFloat("SFX_VOLUME", m_sfxVolume);
		PlayerPrefs.SetInt("GAME_VIEW_LEVEL", m_gameViewLevel);
		PlayerPrefs.SetInt("DOUBLE_CLICK_SKILL", m_doubleClickSkill);
		PlayerPrefs.SetInt("RIGHT_HAND_CONTROL", m_rightHandControl);
        PlayerPrefs.SetInt("JOY_STICK_MODE", m_joyStickMode);
        PlayerPrefs.SetInt("SHOW_HURTNUM", m_showHurtNum);
		
	}

	[ContextMenu("Clean GAME_VIEW_LEVEL")]
	public void CleanViewSet()
	{
		PlayerPrefs.DeleteKey("GAME_VIEW_LEVEL");
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnDestroy()
	{
		Save();
	}
}
