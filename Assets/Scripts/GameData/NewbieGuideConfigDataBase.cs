using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class GuideConfigData
{
    public int _GuideID;
    /// <summary>
    /// 0=普通箭头，1=智能[副本界面智能跳转]，2=拖动引导，3=寻路引导,4=智能查找物品ID
    /// </summary>
    public int _GuideType;
    public string _NpcName;
    public string _NpcIcon;//头像
    public string[] _PreDialogList;
    public string _DialogTitle;
    public string _BtnSignText;
    //[Late]
    public GameObject _ArrowPrefab;
    [HideInInspector]
    public string _ArrowPrefabId;

    public GameObject ArrowPrefab
    {
        get
        {
            if (_ArrowPrefab != null)
            {
                return _ArrowPrefab;
            }

            _ArrowPrefab = AssetId.Resolve(_ArrowPrefab, _ArrowPrefabId);
            return _ArrowPrefab;
        }
    }
    public float _ArrowOffsetX;
    public float _ArrowOffsetY;
    public int[] _GuideBtnID;
    public Vector3 _BtnPosOffset;
    public int _FrameScale;
    //[Late]
    public GameObject _SourceFrame;
    [HideInInspector]
    public string _SourceFrameId;
    public GameObject SourceFrame
    {
        get
        {
            if (_SourceFrame != null)
            {
                return _SourceFrame;
            }

            _SourceFrame = AssetId.Resolve(_SourceFrame, _SourceFrameId);
            return _SourceFrame;
        }
    }
    //[Late]
    public GameObject _TargetFrame;
    [HideInInspector]
    public string _TargetFrameId;
    public GameObject TargetFrame
    {
        get
        {
            if (_TargetFrame != null)
            {
                return _TargetFrame;
            }

            _TargetFrame = AssetId.Resolve(_TargetFrame, _TargetFrameId);
            return _TargetFrame;
        }
    }
	public int _SkipRole;
    public int _OverRole;
}

public class NewbieGuideConfigDataBase : ScriptableObject
{
    public GuideConfigData[] _dataTable;
}

