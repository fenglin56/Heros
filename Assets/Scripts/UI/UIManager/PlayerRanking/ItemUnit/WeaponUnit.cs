using UnityEngine;
using System.Collections;

public class WeaponUnit : MonoBehaviour {

    public GoodsItem EqItem;
    public GoodsItem JewelItem1;
    public GoodsItem JewelItem2;
    public void Init(RankingEquipFightData data)
    {
        EqItem.Init(GoodsItem.GoodsType.equip, data.EquipData);
       
        JewelItem1.Init( GoodsItem.GoodsType.jewel1,data.EquipData);


        JewelItem2.Init(GoodsItem.GoodsType.jewel2, data.EquipData);

    }
}
