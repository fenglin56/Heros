using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// EnterPoint Scene GameManager  音效管理
/// </summary>
///
public class SoundManager : MonoBehaviour
{
    public SoundClipList m_backgroundMusicList;
    public SoundClipList m_soundClipList;
	
	private Dictionary<string, SoundClip> m_dicBGMClip;
    private Dictionary<string, SoundClip> m_dictionarySoundClip;
	
    private static bool m_enableBGM = true;
    private static bool m_enableSFX = true;
    public float SfxVolume = 1.0f;
    public float BgmVolume = 1.0f;
	
	public void SetSfxVolume(float volume)
	{
		SfxVolume = volume;
	}
	
	public void SetBgmVolume(float volume)
	{
		BgmVolume = volume;
		if (m_bgmSource != null)
        {
			m_bgmSource.volume = volume;	
		}
	}
	
    private AudioSource m_bgmSource;
    private AudioSource[] m_sfxChannel;

    private int m_sfxChannelsNumber = 20;
    private static SoundManager m_instance = null;
	
	private string m_lastMusicName = null;

    public static SoundManager Instance { 
		get 
		{ 
			if(null == m_instance)
			{
				m_instance = FindObjectOfType(typeof( SoundManager )) as SoundManager;	
				m_instance.InitSelf();
			}
			
			return m_instance; 
		}
	}

    public bool EnableSFX { get { return m_enableSFX; } set { m_enableSFX = value; } }

    public bool EnableBGM
    {
        get { return m_enableBGM; }

        set
        {
            if (m_enableBGM && !value)
            {
                if (m_bgmSource != null)
                {
                    if (m_bgmSource.isPlaying)
                    {
                        m_bgmSource.Stop();
                    }
                }
            }
            else if (!m_enableBGM && value)
            {
                if (m_bgmSource != null)
                {
                   // _bgmSource.Play();
					PlayBGMWithFade(m_lastMusicName,0.1f);
                }
            }

            m_enableBGM = value;
        }
    }

    public void ClearSoundClipResource()
    {
        foreach (string name in m_dictionarySoundClip.Keys)
        {
            SoundClip soundClip = m_dictionarySoundClip[name];
            soundClip._clip = null;
        }
		
		 for (int i = 0; i < m_sfxChannelsNumber; i++)
         {
              m_sfxChannel[i].clip = null;
         }
    }

    void Awake()
    {
        if (m_instance == null || !m_instance)
        {
            m_instance = this;
        }
    }
	
	private bool m_initialized = false;
	
	void InitSelf()
	{
		if(!m_initialized)
		{
			m_sfxChannel = new AudioSource[m_sfxChannelsNumber];
	        for (int i = 0; i < m_sfxChannelsNumber; i++)
	        {
	            m_sfxChannel[i] = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
	            m_sfxChannel[i].volume = SfxVolume;
	            m_sfxChannel[i].priority = 128;
	        }
	
	        m_bgmSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
	        m_bgmSource.loop = false;
	        m_bgmSource.volume = BgmVolume;
	       	m_bgmSource.playOnAwake = false;
	        m_bgmSource.priority = 100;
	
	        BuildDictionary();
            m_initialized = true;
		}
	}

    [ContextMenu("on scene changed")]
    public void OnSceneChanged()
    {
        if(m_initialized)
        {
            foreach(AudioSource sfxChannel in m_sfxChannel)
            {
                if(!sfxChannel.isPlaying)
                {
                    sfxChannel.clip = null;
                }

            }
            /*
            if(m_bgmSource.isPlaying)
            {
                m_bgmSource.Stop();
            }
            m_bgmSource.clip = null;
            */
        }
    }
	
	void Start()
	{		
		
		InitSelf();
        //SetSfxVolume(GameManager.Instance.m_gameSettings.SfxVolume);
        //SetBgmVolume(GameManager.Instance.m_gameSettings.BgmVolume);
	}
	

    void OnDestroy()
    {
        m_instance = null;
    }

    private void BuildDictionary()
    {
        m_dictionarySoundClip = new Dictionary<string, SoundClip>();
        foreach (SoundClip soundClip in m_soundClipList._soundList)
        {
            if (soundClip._name == null)
            {
                TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"sound clip list");
            }
            else
            {
                try
                {
                    m_dictionarySoundClip.Add(soundClip._name.Trim(), soundClip);
                }
                catch
                {
                    TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Duplicated sound name: " + soundClip._name);
                }
            }
        }
		m_dicBGMClip = new Dictionary<string, SoundClip>();
		foreach(SoundClip clip in m_backgroundMusicList._soundList)
		{
			try
			{
				m_dicBGMClip.Add(clip._name.Trim(), clip);
			}
			catch
			{
				TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"Duplicated sound name: " + clip._name);
			}
			
		}
    }

    public void PlayBGM(string clipName, float fadeTime)
    {	
		m_lastMusicName = clipName;
		
        if (clipName == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"PlayBGM name is null");
            return;
        }

        if (m_enableBGM)
        {
            PlayBGMWithFade(clipName, fadeTime);
        }else
		{
			 m_bgmSource.clip = null;
			 m_loadingMusicName = null;
			 m_isLoadingBGMusic = false;
		}
    }

    public void StopBGM(float fadeTime)
    {
        if (m_enableBGM)
        {
            StartCoroutine(StopBGMWithFade(fadeTime));
        }
    }

    public void StopBGM()
    {
        if(m_enableBGM)
        {
            if (m_bgmSource.isPlaying)
            {
                m_bgmSource.Stop();
            }
            m_loadingMusicName = null;
            m_currentMusicName = null;
        }
    }

    public void PlayBGM(string clipName)
    {
        string musicName = null;
        //SoundClipList list = null;
        
        
        //list = m_backgroundMusicList;
        musicName = clipName;
        
        if (musicName == null)
        {
            ////TraceUtil.Log("[BG Music] Music not found: " + clipName);
            return;
        }
        
        if (musicName == m_currentMusicName)
        {
            return;
        }
        
        m_currentMusicName = musicName;
        PlayMusic(m_currentMusicName);

    }

    private IEnumerator StopBGMWithFade(float fadeTime)
    {
        if (m_bgmSource.isPlaying)
        {
            float time = fadeTime;
            while (time > 0)
            {
                m_bgmSource.volume = (time / fadeTime) * BgmVolume;
                time -= Time.deltaTime;

                yield return null;
            }

            m_bgmSource.Stop();
        }
		m_loadingMusicName = null;
    }


    private string m_loadingMusicName;
    private string m_currentMusicName;
    private bool m_isLoadingBGMusic;

    private void PlayBGMWithFade(string clipName, float fadeTime)
    {
        string musicName = null;
        //SoundClipList list = null;

        
        //list = m_backgroundMusicList;
		musicName = clipName;
       
        if (musicName == null)
        {
            ////TraceUtil.Log("[BG Music] Music not found: " + clipName);
            return;
        }

        if (musicName == m_currentMusicName)
        {
            return;
        }

        m_currentMusicName = musicName;

        if (m_isLoadingBGMusic)
        {
            return;
        }

        if (m_bgmSource.isPlaying)
        {
            //fade out current
            _fadeMode = FadeMode.FadeOut;
            _fadeTime = fadeTime;
            _fadeTimer = 0;
            return;
        }
        else
        {
            PlayMusic(m_currentMusicName);
            //StartCoroutine(StreamPlay(_loadingMusicName));
        }
    }
	
	private void PlayMusic(string musicName)
	{
		m_bgmSource.clip = m_dicBGMClip[musicName].Clip;
		m_bgmSource.loop = true;
		m_bgmSource.Play();
	
	}

    public void StopSoundEffect(string name)
    {
        SoundClip soundClip = null;
        m_dictionarySoundClip.TryGetValue(name, out soundClip);

        if (soundClip == null)
        {
            if(MapResManager.Instance != null)
            {
                soundClip = MapResManager.Instance.GetMapSfxSoundClip(name);
            }
        }
        if(soundClip == null)
        {
            if(SceneSoundResManager.Instance != null)
            {
                soundClip = SceneSoundResManager.Instance.GetSceneSFXClip(name);
            }
        }

        if(null == soundClip)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"dont find clip, name is: " + name);
            return;
        }

        StopSoundEffect(soundClip);
    }

    private void StopSoundEffect(SoundClip audioToStop)
    {
        for (int channelIndex = 0; channelIndex < m_sfxChannelsNumber; channelIndex++)
        {
            if (m_sfxChannel[channelIndex].clip == audioToStop.Clip)
            {
                m_sfxChannel[channelIndex].Stop();
            }
        }
    }

    public AudioSource PlaySoundEffect(string name, Vector3 pos)
    {
        float volume = 1.0f;
		

        if (BattleManager.Instance != null)
        {
            Vector3 cameraPos = Camera.main.gameObject.transform.position;
            cameraPos.y = 0;
            pos.y = 0;

            float distance = Vector3.Distance(cameraPos, pos);

            volume = 1 - (distance - 15) * 0.1f;
            if (volume < 0.3f)
            {
                volume = 0.3f;
            }
            else if (volume > 1.0f)
            {
                volume = 1.0f;
            }
			

           ////TraceUtil.Log("cameraPos:"+ cameraPos + " pos:"+ pos + "   distance:"+ distance + "  volume:"+ volume + "  name"+ name);
        }

        return PlaySoundEffect(name, false, volume);
    }

    public AudioSource PlaySoundEffect(string name)
    {
		//Debug.Log ("PlaySoundEffect=="+name);
        return PlaySoundEffect(name, false, SfxVolume);
    }

    public AudioSource PlaySoundEffect(string name, bool loop)
    {
		//Debug.Log ("PlaySoundEffect=="+name);
        return PlaySoundEffect(name, loop, SfxVolume);
    }

    public AudioSource PlaySoundEffect(string name, bool loop, float volume)
    {
        if (!m_enableSFX)
        {
            return null;
        }
		
		
        if (name == null)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"PlaySoundEffect name is null");
            return null;
        }

        name = name.Trim();

        SoundClip soundClip = null;
        if (m_dictionarySoundClip.ContainsKey(name))
        {
            soundClip = m_dictionarySoundClip[name];
        }

        if(null == soundClip)
        {
            if(MapResManager.Instance != null)
            {
                soundClip = MapResManager.Instance.GetMapSfxSoundClip(name);
            }
        }

        if(null == soundClip)
        {
            if(SceneSoundResManager.Instance != null)
            {
                soundClip = SceneSoundResManager.Instance.GetSceneSFXClip(name);

            }
        }

        if(null == soundClip)
        {
            TraceUtil.Log(SystemModel.Common,TraceLevel.Error,"dont find sfx : "  + name);
            return null;
        }



        if (soundClip._rate != 100)
        {
            int random = Random.Range(0, 100);
            if (random > soundClip._rate)
            {
                return null;
            }
        }

        return PlaySoundEffect(soundClip, loop, volume);
    }
    

    private AudioSource PlaySoundEffect(SoundClip soundClip, bool loop, float volume)
    {
        if (m_enableSFX && soundClip.Clip)
        {
            int channelIndex = 0;
            int leastImportantIndex = 0;

            /*
            for (; channelIndex < m_sfxChannelsNumber; channelIndex++)
            {

                if (m_sfxChannel[channelIndex].clip == null)
                {
                    continue;
                }

                if (m_sfxChannel[channelIndex].clip.name == soundClip.Clip.name)
                {
                    if (soundClip._repeate == 0)
                    {
                        if (!m_sfxChannel[channelIndex].isPlaying)
                        {
                            m_sfxChannel[channelIndex].volume = volume;
                            m_sfxChannel[channelIndex].Play();
                        }
						

                        return m_sfxChannel[channelIndex]; // exit
                    }
                    else
                    {
                        if (!m_sfxChannel[channelIndex].isPlaying)
                        {
                            m_sfxChannel[channelIndex].volume = volume;
                            m_sfxChannel[channelIndex].Play();
							

                            return m_sfxChannel[channelIndex]; // exit
                        }
                    }
                }
            }
            */


            channelIndex = 0;
            for (; channelIndex < m_sfxChannelsNumber; channelIndex++)
            {

                if (!m_sfxChannel[channelIndex].isPlaying)
                {
                    m_sfxChannel[channelIndex].clip = soundClip.Clip;
                    m_sfxChannel[channelIndex].loop = loop;
                    m_sfxChannel[channelIndex].priority = soundClip._priority;
                    m_sfxChannel[channelIndex].volume = volume;
                    m_sfxChannel[channelIndex].Play();
					
                    return m_sfxChannel[channelIndex]; // exit
                }


                // verify the least important playing sound based on its priority and timestamp
                if (m_sfxChannel[leastImportantIndex].priority < m_sfxChannel[channelIndex].priority)
                {
                    leastImportantIndex = channelIndex;
                }
            }


            if (channelIndex == m_sfxChannelsNumber)
            {
                m_sfxChannel[leastImportantIndex].Stop();
                m_sfxChannel[leastImportantIndex].clip = soundClip.Clip;
                m_sfxChannel[leastImportantIndex].loop = loop;
                m_sfxChannel[leastImportantIndex].priority = soundClip._priority;
                m_sfxChannel[leastImportantIndex].volume = volume;
                m_sfxChannel[leastImportantIndex].Play();
				

                return m_sfxChannel[leastImportantIndex];
            }
        }

        return null;
    }

    private enum FadeMode
    {
        None,
        FadeIn,
        FadeOut
    }

    private FadeMode _fadeMode;

    private float _fadeTime;
    private float _fadeTimer;

    private float _bgMusicFadeTime;
	
    void Update()
    {	
        UpdateBGMusic();
    }

    void UpdateBGMusic()
    {
        //volume control of fading in/out background music
        if (_fadeMode == FadeMode.FadeOut)
        {
            _fadeTimer += Time.deltaTime;
            m_bgmSource.volume = (1 - _fadeTimer/_fadeTime) * BgmVolume;

            if (_fadeTimer >= _fadeTime)
            {
                _fadeMode = FadeMode.None;
                _fadeTimer = 0;
                
                m_bgmSource.Stop();

                m_bgmSource.clip = null;

                Destroy(m_bgmSource.clip);

                if (m_loadingMusicName != null)
                {
                    PlayMusic(m_loadingMusicName);
                }
            }
        }
        else if (_fadeMode == FadeMode.FadeIn)
        {
            _fadeTimer += Time.deltaTime;

            m_bgmSource.volume = _fadeTimer / _fadeTime * BgmVolume;

            if (_fadeTimer >= _fadeTime)
            {
                _fadeMode = FadeMode.None;
                _fadeTimer = 0;
                ////TraceUtil.Log("[BG Music] Music fully faded in. Volume = " + _bgmSource.volume);
            }
        }
        else
        {
            if ((Mathf.FloorToInt(Time.realtimeSinceStartup) % 5 == 0) &&
				m_enableBGM &&
				m_loadingMusicName != null &&
				!m_isLoadingBGMusic &&
				!m_bgmSource.isPlaying)
            {
				m_bgmSource.Stop();
                m_bgmSource.volume = 0;
				PlayMusic(m_loadingMusicName);
				//StartCoroutine(StreamPlay(_loadingMusicName));
				////TraceUtil.Log("[BG Music] Replaying ............" + _loadingMusicName);
            }
        }
    }
	
	[ContextMenu("test music")]
	public void Testmusic()
	{
		SoundManager.Instance.PlayBGM("BGM1", 0.0f);	
	}
}
