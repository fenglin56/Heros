using UnityEngine;
using System.Collections;
using System;


[Serializable]
public class PlayerGiftConfigData
{
	public int _goodsID;
	public int _giftType;
	public int _packageNeed;
	//public string _goodsWeight;
	//public string _getGoodsID;
	//public string _getGoodsNum;
	//public string _GoodsDisplay;
	//public GoodsDisplay[] _GoodsDisplay;
	public ProfessionGoodsDisplay[] _ProfessionGoodsDisplay;

	//public GetGoods[] _GetGoods;

	[Serializable]
	public class ProfessionGoodsDisplay
	{
		public int Profession;
		public GoodsDisplay[] GoodsDisplay;
	}
	[Serializable]
	public class GoodsDisplay
	{
		public int Profession;
		public int GoodsID;
		public int MinNum;
		public int MaxNum;
	}

	
	[Serializable]
	public class GetGoods
	{
		public int GoodsID;
		public int GoodsNum;
	}

}


//[Serializable]
//public class GoodsDisplay
//{
//	public int GoodsID;
//	public int MinNum;
//	public int MaxNum;
//}



public class PlayerGiftConfigDataBase : ScriptableObject
{
	public PlayerGiftConfigData[] _dataTable;
}
