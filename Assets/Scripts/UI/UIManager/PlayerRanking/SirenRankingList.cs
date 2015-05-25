using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
namespace UI.Ranking
{
    public class SirenRankingList :BaseSubUiPanel {
        public GameObject SirenRankingItem_Prefab;
        public UIGrid ItemTable;
        public SMsgInteract_RankingList_SC Data;
        [HideInInspector]
        public  List<SirenRankingItem> SirenRankingItemList=new List<SirenRankingItem>();
        [HideInInspector]
        public List<SirenRankingItem> SirenRankingSelectedItemList=new List<SirenRankingItem>();
        private GameObject Item_go;
        public UIDraggablePanel draggablePanel;
        public void InitList(RankingType type)
        {
           for(int i=0;i<10;i++)
                    {
                        Item_go=NGUITools.AddChild(ItemTable.gameObject,SirenRankingItem_Prefab);
                        Item_go.SetActive(false);
                        Item_go.name=SirenRankingItem_Prefab.name+i;
                        Item_go.AddComponent<UIDragPanelContents>();
                        SirenRankingItem Sc_item=Item_go.GetComponent<SirenRankingItem>();
                        //Sc_item.OnClickCallBack=ItemSelectedEventHandle;
                        SirenRankingItemList.Add(Sc_item);
                    }

        }
        public void StartRefershList( List<RankingYaoNvFightData> datas)
        {
            StartCoroutine(RefershList(datas));
        }
        
        IEnumerator RefershList(  List<RankingYaoNvFightData> YaonvData)
        {
                    draggablePanel.ResetPosition();
                    SirenRankingItemList.ApplyAllItem(p=>p.gameObject.SetActive(false));
                    if(YaonvData.Count>0)
                    {
                        for(int i=0;i<YaonvData.Count;i++)
                        {
                            SirenRankingItemList[i].InitItemData(YaonvData[i]);
                            SirenRankingItemList[i].gameObject.SetActive(true);
                        }
                    }
            yield return null;
            ItemTable.Reposition();
            
        }
    }
}
