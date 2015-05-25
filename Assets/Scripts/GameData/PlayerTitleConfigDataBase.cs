using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerTitleConfigData : ItemData
{
	public int _lGoodsID; //称号ID 既是物品ID
	//public string _TitleName; //称号名称
	public int _TitleDroit; //称号所属
	public int _ByHaveTimeLimit;//是否有时间限制
	public int _lUseTerm;//lUseTerm
	public string _lDisplayIdSmall;//称号图片
	public GameObject _lDisplayIdSmallPrefab;
	public GameObject _ModelIdPrefab;	//称号特效 "ModelId"
	//public string _szDesc;//称号获得条件文字 ids
	public string _lDisplayID;//称号效果（被动技能）描述文本
	public PlayerTitleGetReq[] _GetReqs;//称号获得条件
	public string _vectEffects;//加成效果
	public string _vectEffectsAdd;//加成效果 额外


}

[System.Serializable]
public class PlayerTitleGetReq
{
	public int ID;
	public int Parameter;
}


public class PlayerTitleConfigDataBase : ScriptableObject
{
	public PlayerTitleConfigData[] _dataTable;
}
