using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SenceAudioFactory : MonoBehaviour {

    public Scene3DAudioConfigaDataBase AudioConfig;
    //public GameObject NpcTitle;
    private List<AudioSource> AudioList=new List<AudioSource>();
    void Awake()
    {
        //m_createNPC = false;
    }
    

    private void CreateAudio(Scene3DAudioData audioData)
    {
        GameObject obj= new GameObject();

        obj.transform.position=audioData.PointPos;
        obj.name="Audio_"+audioData.Sound;
        AudioSource audioSource= obj.AddComponent<AudioSource>();
        audioSource.maxDistance=audioData.Radius;
        audioSource.rolloffMode=AudioRolloffMode.Linear;
        audioSource.dopplerLevel=0;
        audioSource.loop=true;
        audioSource.clip=SceneSoundResManager.Instance.GetSceneSFXClip(audioData.Sound).Clip;
        SetVolume(audioSource,GameManager.Instance.m_gameSettings.SfxVolume);
        audioSource.Play();
        AudioList.Add(audioSource);
    }

    public void SetSfxVolume(float volume)
    {
        foreach(var item in AudioList)
        {
            SetVolume(item,volume);
        }      
    }

    private void SetVolume(AudioSource audio,float Volume)
    {
        audio.volume=Volume;
    }
    public void CreateSceneAudioObject(uint mapId)
    {
        AudioList.Clear();
        foreach(var item in AudioConfig.SoundList)
        {
            if(item.MapId==(int)mapId)
            {
                CreateAudio(item);
            }
        }
       
    }
}
