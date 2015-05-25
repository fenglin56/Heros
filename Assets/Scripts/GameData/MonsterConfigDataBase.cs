using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class SkillGroup
{
    public int _skillID;
    public int _skillLevel;
    public int _skillProbability;
}

[Serializable]
public class MonsterConfigData
{
    public int _monsterID;
    public string _szName;
    public float _moveSpeed;
    public int _skillID;
    public SkillGroup[] _skillGroup;
    //[GameDataPostFlag(true)]
    /*
	[Late]public GameObject _MonsterPrefab;
	[HideInInspector]
	public string _MonsterPrefabId;
	
	public GameObject MonsterPrefab{
		get{
			
			
			if(_MonsterPrefab != null)
			{
				return _MonsterPrefab;
			}
			
			_MonsterPrefab =  AssetId.Resolve(_MonsterPrefab,_MonsterPrefabId);
			return  _MonsterPrefab;
			
			
			//return AssetId.Resolve(_prefab,_prefabId);
		}
	}
 */   

	

    public int MonsterSubType;
    public string Hurt_sfx;

    public int _standUpTime;
    public float _flyHigh;
	public float _fly_initial_high;
	public int _isShowGuideArrow;
    public float m_shieldpoint;   //防护值(罩)
    public float m_breaktime;     //破防时间

    public int _maxHP;

    public string _bornEffects;
    public string _dialogMonsterName;
    public string _dialogPortrait;
    public string _bornDialogue;
    public string _bornSound;
	public BornDialogueFull[] _BornDialogueFulls;


    public string _deadEffect;
    public int _hitRadius;   //碰撞半径

    public Vector3 _cameraFix_pos;//固定位置
    public float _cameraFix_time;//运动时间

    public float _cameraStay_time;
    public float _cameraBack_time;
    public bool _blockPlayerToIdle;

	public string _downTips;  //防守副本事件提示IDS
	public string _upTips;  //防守副本怪物特殊属性提示IDS

	public int[] _DeathBullet;//死亡时发出的子弹


	[Serializable]
	public class BornDialogueFull
	{
		public string Portrait;
		public string MonsterName;
		public string Dialogue;
		public int BornDialoguePosition;
		public float Time;
        public int protraitType;
	}

    /*
	//[Late]
	public GameObject _monsterBloodEffectPrefab;
	[HideInInspector]
	public string _monsterBloodEffectPrefabId;
	public GameObject MonsterBloodEffectPrefab{
		get{
			
			
			if(_monsterBloodEffectPrefab != null)
			{
				return _monsterBloodEffectPrefab;
			}
			
			_monsterBloodEffectPrefab =  AssetId.Resolve(_monsterBloodEffectPrefab,_monsterBloodEffectPrefabId);
			return  _monsterBloodEffectPrefab;
			//return AssetId.Resolve(_prefab,_prefabId);
		}
	}
*/
	

}

public class MonsterConfigDataBase : ScriptableObject 
{
    public MonsterConfigData[] _dataTable;
}
