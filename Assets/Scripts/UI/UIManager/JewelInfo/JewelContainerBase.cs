using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace UI.MainUI
{
    public abstract class  JewelContainerBase : JewelBesetUIBase {
	public UITable ItemTable;
	public UILabel NoneItemLable;
	public GameObject JewelBeset_ContainerItemProfab;
    [HideInInspector]
	public   List<JewelBset_ContainerItem> ItemList=new List<JewelBset_ContainerItem>();
	protected List<ItemFielInfo> ItemFileinfoList;
	public List<JewelBset_ContainerItem> SelectedItemList=new List<JewelBset_ContainerItem>();
	private GameObject Item_go;
    protected float m_noticeToDragAmount;
    private bool m_shouldMove;
    public virtual IEnumerator RefreshList(JewelState tab)
    {
        return RefreshList(tab, BtnMapId_Sub.Empty);
    }
    public virtual IEnumerator RefreshList(JewelState tab, BtnMapId_Sub btnMapId_Sub)
	{
		ItemTable.transform.ClearChild();
		ItemList.Clear();
		InitItemFileinfoList(tab);
        int columnAmount = Mathf.CeilToInt(ItemFileinfoList.Count / 3.0f);
        int j = 0;
        if (ItemFileinfoList.Count > 0)
        {
            m_shouldMove = ItemFileinfoList.Count > 9;
            for (int i = 0; i < ItemFileinfoList.Count(); i++)
            {
                if (i % 3 == 0)  //3项一排
                {
                    j++;
                }
                Item_go = NGUITools.AddChild(ItemTable.gameObject, JewelBeset_ContainerItemProfab);
                Item_go.name = JewelBeset_ContainerItemProfab.name + i;
                JewelBset_ContainerItem Sc_item = Item_go.GetComponent<JewelBset_ContainerItem>();
                Sc_item.DragAmount = j / columnAmount;
                Sc_item.InitItemData(ItemFileinfoList[i]);
                Sc_item.OnItemClick = ItemSelectedEventHandle;
                ItemList.Add(Sc_item);
            }
            if (ItemList.Count > 0)
            {
                ItemList[0].OnBeSelected();
            }
            yield return null;
            ItemTable.Reposition();
            yield return null;
            //引导
            if (ItemList != null)
            {
                ItemList.ApplyAllItem(P =>
                {
                    if (P != null)
                    {
                        //P.gameObject.RegisterBtnMappingId(P.ItemFielInfo.LocalItemData._goodID, MainUI.UIType.Gem, BtnMapId_Sub.Gem_JewelContainerListPanel_Item, NoticeToDragSlerp, P.DragAmount);
						P.gameObject.RegisterBtnMappingId(MainUI.UIType.Gem,BtnMapId_Sub.Gem_JewelBesetPanel_Weapon+P.ItemFielInfo.LocalItemData._GoodsSubClass-1,
							                                  NoticeToDragSlerp, P.DragAmount);
                    }
                });
            }
            if (m_noticeToDragAmount != 0)
            {
                StartCoroutine(DragAmountSlerp(m_noticeToDragAmount));
                m_noticeToDragAmount = 0;
            }
        }       
	}
    /// <summary>
    /// 记下要自动滚动到的位置
    /// </summary>
    /// <param name="targetAmount"></param>
    protected void NoticeToDragSlerp(float targetAmount)
    {        
        m_noticeToDragAmount = targetAmount;
    }

    protected IEnumerator DragAmountSlerp(float targeAmount)
    {
        yield return null;
        var dragPanelComp = ItemTable.transform.parent.GetComponent<UIDraggablePanel>();
        if (dragPanelComp.shouldMove)
        {
            float smoothTime = 0.3f, currentSmoothTime = 0; ;
            float currentAmount = 0;
            while (true)
            {
                currentSmoothTime += Time.deltaTime;
                currentAmount = Mathf.Lerp(currentAmount, targeAmount, Time.deltaTime * 20);
                dragPanelComp.SetDragAmount(0, currentAmount, false);
                yield return null;
                if ((targeAmount - currentAmount) <= float.Epsilon || currentSmoothTime >= smoothTime)
                {
                    break;
                }
            }
        }
    }
		public void SelectItem(ItemFielInfo itemFileInfo)
		{
			if(itemFileInfo!=null)
			{
		    foreach(var item in ItemList)
			 {
				//ItemList.ApplyAllItem(c=>c.OnLoseFocus());
				if(item.ItemFielInfo==itemFileInfo)
				{
					item.OnBeSelected();
				}
			 }
			}
		}
		public virtual void ShowNoneItemLable(string IDS)
		{
			NoneItemLable.gameObject.SetActive(true);
			NoneItemLable.SetText(LanguageTextManager.GetString(IDS));
		}
		public virtual void HideNoneItemLable()
		{
			NoneItemLable.gameObject.SetActive(false);
		}
		public virtual void ItemSelectedEventHandle(JewelBset_ContainerItem selectedEquipItem)
		{
			//所有项LoseFocus
			ItemList.ApplyAllItem(P=>P.OnLoseFocus());
			SelectedItemList.Clear();
			//当前项GetFocus
			selectedEquipItem.OnGetFocus(false);
			SelectedItemList.Add(selectedEquipItem);
		}
		public abstract void InitItemFileinfoList(JewelState tab);
}
}
