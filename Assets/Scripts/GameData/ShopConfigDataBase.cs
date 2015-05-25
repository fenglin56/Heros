using UnityEngine;
using System.Collections;

[System.Serializable]
public class ShopConfigData
{
    public int _shopGoodsID;
    public int _goodsNum;
    public int _shopID;
    public string _shopName;
    public int GoodsID;
    public int BuyLvl;//购买时装的前置等级
    public int BuyType;//兑换的货币类型，1=铜币，2=绑定元宝，3=金元宝，4=用道具兑换
    public int Price;//出售价格
    public string ExChangeGoodID;//兑换的物品ID
	public string GoodsPicture;//商品图片
	public string GoodsNameIds;
	public GameObject goodsPicturePrefab;
	public int PackageNeed;
}

public class ShopConfigDataBase : ScriptableObject
{
    public ShopConfigData[] _dataTable;
}
