using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class AuctionConfigData
{
	public int AuctionClass;
	public int AuctionID;
	public int GoodsWeight;
	public int GoodsID;
	public int GoodsNum;
	public int StartingBid;
}

public class AuctionConfigDataBase : ScriptableObject{
	public AuctionConfigData[] _dataTable;
}