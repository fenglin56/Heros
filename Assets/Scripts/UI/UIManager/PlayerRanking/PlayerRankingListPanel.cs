using UnityEngine;
using System.Collections;
using UI.MainUI;
using System.Collections.Generic;
namespace UI.Ranking
{
    public class PlayerRankingListPanel :  BaseSubUiPanel{
        public GameObject PlayerRankingItem_Prefab;
        public UIGrid ItemTable;
        public SMsgInteract_RankingList_SC Data;
        [HideInInspector]
        public  List<playerRankingItem> PlayerRankingItemList=new List<playerRankingItem>();
        [HideInInspector]
        public List<playerRankingItem> PlayerRankingSelectedItemList=new List<playerRankingItem>();
        private GameObject Item_go;
        public UIDraggablePanel draggablePanel;
        public void InitList(RankingType type)
        {
      
                    for(int i=0;i<10;i++)
                    {
                    Item_go=NGUITools.AddChild(ItemTable.gameObject,PlayerRankingItem_Prefab);
                    Item_go.name=PlayerRankingItem_Prefab.name+i;
                    Item_go.AddComponent<UIDragPanelContents>();
                    playerRankingItem Sc_item=Item_go.GetComponent<playerRankingItem>();
                    Item_go.SetActive(false);
                    //Sc_item.OnClickCallBack=ItemSelectedEventHandle;
                    PlayerRankingItemList.Add(Sc_item);
                    }
            ItemTable.Reposition();
        }
        public void StartRefershList( List<RankingActorFightData> datas)
        {
            StartCoroutine(RefershList(datas));
        }

        IEnumerator RefershList( List<RankingActorFightData> datas)
        {
       
        
             PlayerRankingItemList.ApplyAllItem(p=>p.gameObject.SetActive(false));
            if(datas.Count>0)
            {
                for(int i=0;i<datas.Count;i++)
                {
                    PlayerRankingItemList[i].InitItemData(datas[i]);
                    PlayerRankingItemList[i].gameObject.SetActive(true);
                }
            }
             
        
            yield return null;
            draggablePanel.ResetPosition();
            ItemTable.Reposition();
            
        }
       


    }
}
