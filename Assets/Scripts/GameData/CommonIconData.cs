using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class CommonIconData  {

    public String _IconName;
    [GameDataPostFlag(true)]
    public GameObject _IconPrefab;

}
