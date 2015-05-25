using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class AnimationConfigData
{
    public string Clip;
    public string Mode;
    public int Weight;
    public float Time;
    public string EventType;
    public string AfterClip;
    public int StepValue;

    public WrapMode WrapMode
    {
        get
        {
            WrapMode mode=WrapMode.Default;
            try
            {
                mode=(WrapMode)Enum.Parse(typeof(WrapMode),Mode);
            }
            catch (System.Exception ex)
            {
            	TraceUtil.Log(SystemModel.Common,TraceLevel.Error,ex.ToString());
            }
            return mode;
        }
    }
    public float CalcAniSpeed(int stepValue)
    {
        return stepValue / StepValue;
    }
}
public class AnimationConfigDataBase:ScriptableObject
{
    public AnimationConfigData[] AnimationConfigDatas;
}
