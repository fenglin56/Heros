
using UnityEngine;
using System;

public class NewCharacterConfigDataBase:ScriptableObject{

    public NewCharacterConfigData[] NewCharacterConfigDataList;
}

[Serializable]
public class NewCharacterConfigData
{
    public int VocationID;
    public string Picture;
    public string Model;
    [GameDataPostFlag(true)]
    public GameObject Weapon;
    public string WeaponName;
    public string WeaponPosition;
    public string AnimationsStr;
    public string SelectAnimationsStr;
    public string Introductions;
    public string[] Animations;
    public int[] AnimationsTime;
    public string[] SelectAni;
    public int[] SelectAnimTime;
    public string SelectAniSound;
    public string ChooseVoice;

    public string EffectName;
    public GameObject EffectPrefab;
    public float EffectTime;

    public void PostprocessorAnimation()
    {
        string[] AnimationAndTimes = AnimationsStr.Split('|');
        Animations = new string[AnimationAndTimes.Length];
        AnimationsTime = new int[AnimationAndTimes.Length];
        for (int i = 0; i < AnimationAndTimes.Length; i++)
        {
            string[] LocalAnimatin = AnimationAndTimes[i].Split('+');
            Animations[i] = LocalAnimatin[0];
            AnimationsTime[i] = int.Parse(LocalAnimatin[1]);
        }

        string[] SelectAnimationAndTimes = SelectAnimationsStr.Split('|');
        SelectAni = new string[SelectAnimationAndTimes.Length];
        SelectAnimTime = new int[SelectAnimationAndTimes.Length];
        for (int i = 0; i < SelectAnimationAndTimes.Length; i++)
        {
            string[] LocalAnimatin = SelectAnimationAndTimes[i].Split('+');
            SelectAni[i] = LocalAnimatin[0];
            SelectAnimTime[i] = int.Parse(LocalAnimatin[1]);
        }
    }

}