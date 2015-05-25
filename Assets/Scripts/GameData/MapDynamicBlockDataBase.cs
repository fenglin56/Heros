using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class MapDynamicBlockData
{
    public int MapID;
    public int AreaID;
    public int BlockGroup;
    public byte BlockState;
    [DataToObject(PrefabPath="")]
    public GameObject BlockEffect;
    [DataToObject(PrefabPath = "")]
    public GameObject BlockFadeEffect;
    public string EffectPosition;
    public int FadeDelay;
    public int GuideArrow;

    [HideInDataReader]
    public float[] EffectAngle;
    [HideInDataReader]
    public Vector3[] EffectPos;
}
public class MapDynamicBlockDataBase : ConfigBase
{
    public MapDynamicBlockData[] Datas;

    public override void Init(int length, object dataList)
    {
        Datas = new MapDynamicBlockData[length];

        var realData = dataList as List<MapDynamicBlockData>;
        for (int i = 0; i < realData.Count; i++)
        {
            Datas[i] = (MapDynamicBlockData)realData[i];
            string[] effPosAngles = Datas[i].EffectPosition.Split('|');
            int len = effPosAngles.Length;
            Datas[i].EffectAngle = new float[len];
            Datas[i].EffectPos = new Vector3[len];
            for (int j = 0; j < len; j++)
            {
                var effPosAngle=effPosAngles[j].Split('+');
                Datas[i].EffectAngle[j] = float.Parse(effPosAngle[0]);
                Datas[i].EffectPos[j] = new Vector3(float.Parse(effPosAngle[1])*0.1f, 0, float.Parse(effPosAngle[2])*-0.1f);
            }
        }
    }
    public IEnumerable<MapDynamicBlockData> GetMapDynamicBlockData(int areaNum)
    {
        return Datas.Where(P => P.AreaID == areaNum);
    }
}