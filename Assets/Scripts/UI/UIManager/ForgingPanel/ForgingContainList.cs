using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace UI.Forging
{
public class ForgingContainList : MonoBehaviour {
        public UIGrid ItemTable;
        protected  List<ForgingListItem> ItemList=new List<ForgingListItem>();//所有生成的Item
        protected List<ForgeRecipeData> ForgeRecipeDataList;//所有Item的数据
        [HideInInspector]
        public ForgingListItem SelectedItem;//所有选中的Item;
        private GameObject Item_go;
        public GameObject FriendListItemPrefab;
        private UIDraggablePanel m_dragPanelComp;
        private ForgingType CurrentListType;
        void Awake()
        {
            m_dragPanelComp = transform.GetComponentInChildren<UIDraggablePanel>();
        }
        public void InitListItem()
        {
          
            StartCoroutine(St_InitListItem());
        }

        public void UpdateList()
        {
            StartCoroutine(St_InitListItem());
        }
        
        private  IEnumerator St_InitListItem()
        {
          //  ItemTable.transform.ClearChild();
            ItemList.ApplyAllItem(c=>c.gameObject.SetActive(false));
            InitItemFileinfoList();          
            if(ForgeRecipeDataList.Count>0)
            {
//                if(ForgeRecipeDataList.Count>=4)
//                {

                  for( int i=0;i<ForgeRecipeDataList.Count;i++)
                  {
                    if(ItemList.Count>i)
                    {
                        ItemList[i].gameObject.SetActive(true);
                        ItemList[i].InitItemData(ForgeRecipeDataList[i]);
                    }
                    else
                    {
                    Item_go=NGUITools.AddChild(ItemTable.gameObject,FriendListItemPrefab);
                    Item_go.name=FriendListItemPrefab.name+i;
                    Item_go.RegisterBtnMappingId(ForgeRecipeDataList[i].ForgeID,UI.MainUI.UIType.Forging, BtnMapId_Sub.Forging_ForgingItem);
                    Item_go.AddComponent<UIDragPanelContents>();
                    ForgingListItem Sc_item=Item_go.GetComponent<ForgingListItem>();
                    Sc_item.InitItemData(ForgeRecipeDataList[i]);
                    Sc_item.OnClickCallBack=ItemSelectedEventHandle;
                    ItemList.Add(Sc_item);
                    }
                  }
                if(ForgingPanelManager.GetInstance().CurrentForingType==CurrentListType)
                {
                    if(SelectedItem!=null)
                    {
                        SelectedItem.BeSelected();
                    }
                    else
                    {
                    ItemList[0].BeSelected();
                    }
                }else
                {
                    ItemList[0].BeSelected();
                }
                CurrentListType=ForgingPanelManager.GetInstance().CurrentForingType;
                yield return null;
                ItemTable.Reposition();
                m_dragPanelComp.ResetPosition();
              //  yield return null;
                //如果有引导箭头，不允许拖动
                if (HasGuideArrow)
                {
                    m_dragPanelComp.LockDraggable = true; 
                }
                else
                {
                    m_dragPanelComp.LockDraggable = false;
                }
            }
            else
            {
                

            }
            
        }
        void Update()
        {
            if (m_dragPanelComp != null && m_dragPanelComp.LockDraggable)
            {
                m_dragPanelComp.LockDraggable = HasGuideArrow;
            }


        }
        /// <summary>
        /// 是否有引导箭头
        /// </summary>
        /// <returns></returns>
        private bool HasGuideArrow
        {
            get
            {
                foreach (var item in ItemList)
                {
                    var btnGuideBehaviour = item.GetComponent<GuideBtnBehaviour>();
                    if (btnGuideBehaviour != null)
                    {
                        if (btnGuideBehaviour.BtnFrame != null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        void InitItemFileinfoList()
        {
            int vocation= PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
            ForgeRecipeDataList=ForgingRecipeConfigDataManager.Instance.ForgeRecipeDataList(ForgingPanelManager.GetInstance().CurrentForingType).Where(p=>p.ForgeProfession.LocalContains(vocation)).ToList();
        }
        public  void ItemSelectedEventHandle(ForgingListItem selectedEquipItem)
        {
//            //所有项LoseFocus
              
            if( ForgingPanelManager.GetInstance().CurrentData!=selectedEquipItem.ForgeRecipeItem)
            {
                //SelectedItemList.Clear();
              
                ItemList.ApplyAllItem(p=>p.OnLoseFocus());
                selectedEquipItem.OnGetFocus();
                SelectedItem=selectedEquipItem;
               ForgingPanelManager.GetInstance().CurrentData=selectedEquipItem.ForgeRecipeItem;
               ForgingPanelManager.GetInstance().Sc_ForgingResult.InitForgResult(selectedEquipItem.ForgeRecipeItem); 
            }
            ForgingPanelManager.GetInstance().CheckButtonEff();
          
        }
	
}
}
