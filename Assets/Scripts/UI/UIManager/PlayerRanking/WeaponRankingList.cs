using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
namespace UI.Ranking
{
    public class WeaponRankingList : BaseSubUiPanel {
        public GameObject WeaponRankingItem_Prefab;
        public UIGrid ItemTable;
        [HideInInspector]
        public SMsgInteract_RankingList_SC Data;
        [HideInInspector]
        public  List<WeaponRankingItem> WeaponRankingItemList=new List<WeaponRankingItem>();
        [HideInInspector]
        public List<WeaponRankingItem> WeaponRankingSelectedItemList=new List<WeaponRankingItem>();
        private GameObject Item_go;
        public UIDraggablePanel draggablePanel;

        public void InitList(RankingType type)
        {
            for(int i=0;i<10;i++)
            {
                Item_go=NGUITools.AddChild(ItemTable.gameObject,WeaponRankingItem_Prefab);
                Item_go.SetActive(false);
                Item_go.name=WeaponRankingItem_Prefab.name+i;
                Item_go.AddComponent<UIDragPanelContents>();
                WeaponRankingItem Sc_item=Item_go.GetComponent<WeaponRankingItem>();
                //Sc_item.OnClickCallBack=ItemSelectedEventHandle;
                WeaponRankingItemList.Add(Sc_item);
            }
            
        }
        public void StartRefershList( List<RankingEquipFightData> datas)
        {
            StartCoroutine(RefershList(datas));
        }
        
        IEnumerator RefershList(  List<RankingEquipFightData> Datas)
        {
            draggablePanel.ResetPosition();
            WeaponRankingItemList.ApplyAllItem(p=>p.gameObject.SetActive(false));
            if(Datas.Count>0)
            {
                for(int i=0;i<Datas.Count;i++)
                {
                    WeaponRankingItemList[i].InitItemData(Datas[i]);
                    WeaponRankingItemList[i].gameObject.SetActive(true);
                }
            }
            yield return null;
            ItemTable.Reposition();

        }
    }
}