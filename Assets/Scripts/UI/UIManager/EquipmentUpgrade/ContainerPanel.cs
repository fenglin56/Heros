using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI.MainUI;
using System.Linq;
public class ContainerPanel : MonoBehaviour {
    public UIGrid grid;
    public GameObject ItemPrefab;
    [HideInInspector]
    public List<ContainerItem> ContainerList=new List<ContainerItem>();
    public UIDraggablePanel Panel;
    [HideInInspector]
    public List<ItemFielInfo> ItemDatas;
    private ContainerItem m_currentSelectItem;
	private float startMarkDragpanelPosY ;
	bool isRead = false;
	void Init()
	{
		if (isRead)
			return;
		isRead = true;
		grid.sorted = true;
		startMarkDragpanelPosY = Panel.transform.localPosition.y;
	}
    ContainerItem GetDefultItem()
    {
        ContainerItem item=null;
		if(ItemDatas==null||ItemDatas.Count==0)
		{
			return item;
		}
        if(EquipmentUpgradeDataManger.Instance.CurrentSelectEquip==null)//如果当前选择item=null则默认为第一个
        {
            if(ContainerList.Count>0)
            {
               item=ContainerList[0];
            }
        }
        else
        { 
            var selectItem = EquipmentUpgradeDataManger.Instance.CurrentSelectEquip;
            var id = (selectItem.LocalItemData as EquipmentData).UpgradeID;
            item = ContainerList.SingleOrDefault(c => c.m_itemFileInfo == selectItem);
            if (item == null)//如果当前选择项存在但不在列表中说明是产生了新物品
            {
                if( EquipmentUpgradeDataManger.Instance.NewItemUID!=0)//如果新物品id！=0那再按照新物品id查询一次，还没有就默认第一个
                {
                    item = ContainerList.SingleOrDefault(c => c.m_itemFileInfo.sSyncContainerGoods_SC.uidGoods== EquipmentUpgradeDataManger.Instance.NewItemUID);
                    EquipmentUpgradeDataManger.Instance.NewItemUID=0;
                }
                else
                {
                    if(ContainerList.Count>0)
                    {
                        item=ContainerList[0];
                    }
                }
            }

        }
        return item;
    }

    public void RefreshEachListItem()
    {
        ContainerList.ApplyAllItem(c=>c.RefreshItem());
        m_currentSelectItem.SelectItem();
    }
    public void UpdateListPanel()
    {
      if(EquipmentUpgradeDataManger.Instance.CurrentType!=UpgradeType.Upgrade)
        {
            ItemDatas=ContainerInfomanager.Instance.GetEquiptItemList();
        }
        else
        {
            ItemDatas=ContainerInfomanager.Instance.GetAllEquipment().Where(c=>(c.LocalItemData as EquipmentData).UpgradeID!=0).ToList();
        }
        ItemDatas.Sort((x,y)=>Rank(x,y));
       StartCoroutine( CreatListItem());
       var item = GetDefultItem();
       if(item!=null)
            {
            item.SelectItem();
            }
        else
        {
            EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=null;
            EquipmentUpgradePanelManager.GetInstance().UpdateResultPanel();

        }

    }
    IEnumerator CreatListItem()
    { 
       // grid.transform.ClearChild();
      //  ContainerList.Clear();
		Init ();
        ContainerList.ApplyAllItem(c=>c.gameObject.SetActive(false));

        for(int i=0;i<ItemDatas.Count;i++)
        {
            if(ItemDatas[i].LocalItemData._GoodsSubClass==2)
            {
                continue;
            }
            if(ItemPrefab==null)
            {
                Debug.LogError("ItemPrefab=null");
               // return ;
            }
            GameObject go;
            if(ContainerList.Count>i)
            {
                go=ContainerList[i].gameObject;
                go.SetActive(true);
                ContainerList[i].Init(ItemDatas[i]);
            }
            else
            {
                go=UI.CreatObjectToNGUI.InstantiateObj(ItemPrefab,grid.transform) as GameObject;
                go.name=ItemPrefab.name+i;
                ContainerItem scContainerItem= go.GetComponent<ContainerItem>();
                scContainerItem.SetSelcetCallBack(OnSelect);
                ContainerList.Add(scContainerItem);
                scContainerItem.Init(ItemDatas[i]);
            }
			bool isBtnMapMark = true;

			if(isBtnMapMark)
			{
				int index = i;
				if( i == 0 )
				{
					index = -1;
				}
				float dragAmount = (index+1) / (float)ItemDatas.Count;
				BtnMapId_Sub subType = BtnMapId_Sub.EquipmentUpgrade_Stren_Weapon;
				bool isSpecial = false;
				switch(EquipmentUpgradeDataManger.Instance.CurrentType)
				{
				case UpgradeType.Strength:
					subType = BtnMapId_Sub.EquipmentUpgrade_Stren_Weapon;
					break;
				case UpgradeType.StarUp:
					subType = BtnMapId_Sub.EquipmentUpgrade_Star_Weapon;
					break;
				case UpgradeType.Upgrade:
				{
					isSpecial = true;
					subType = BtnMapId_Sub.EquipmentUpgrade_Upgrade_Item;
				}
					break;
				}
				if(isSpecial)
				{
					go.RegisterBtnMappingId(ItemDatas[i].LocalItemData._goodID,UIType.EquipmentUpgrade, subType ,NoticeToDragSlerp, dragAmount);
				}
				else
				{
					subType = subType+ItemDatas[i].LocalItemData._GoodsSubClass-1;
					go.RegisterBtnMappingId(UIType.EquipmentUpgrade, subType ,NoticeToDragSlerp, dragAmount);
				}

			}
        }
        yield return null;
		grid.sorted=true;
        grid.Reposition();
        Panel.ResetPosition();
		if (m_noticeToDragAmount != 0) {
			Panel.transform.localPosition = new Vector3(Panel.transform.localPosition.x,startMarkDragpanelPosY,Panel.transform.localPosition.z);
			StartCoroutine (DragAmountSlerp (m_noticeToDragAmount));
			m_noticeToDragAmount = 0;
		}
    }
	private float m_noticeToDragAmount;
	/// <summary>
	/// 记下要自动滚动到的位置
	/// </summary>
	/// <param name="targetAmount"></param>
	protected void NoticeToDragSlerp(float targetAmount)
	{        
		m_noticeToDragAmount = targetAmount;
	}
	public IEnumerator DragAmountSlerp(float targeAmount)
	{
		yield return null;
		var dragPanelComp = Panel.GetComponent<UIDraggablePanel>();
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
    void OnSelect(ContainerItem item)
    {
        m_currentSelectItem=item;
        ContainerList.ApplyAllItem(c=>c.LoseFouce());
        item.OnFouce();
        EquipmentUpgradeDataManger.Instance.CurrentSelectEquip=item.m_itemFileInfo;
        EquipmentUpgradePanelManager.GetInstance().UpdateResultPanel();
    }

    /// <summary>
    /// Rank the specified x and y.排序规则：1已装备排在前面 2.如果已装备按照位置从小到大排 3.如果未装备先按颜色排，再按等级排
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    public int Rank(ItemFielInfo x,ItemFielInfo y)
    {
        return  GetWeight(y)-GetWeight(x);
    }

    public int GetWeight(ItemFielInfo item)
    {
        int weigth=0;
        if(ContainerInfomanager.Instance.IsItemEquipped(item))
        {
            weigth+=(16-item.sSyncContainerGoods_SC.nPlace)*100000;

        }
        weigth+=(item.LocalItemData._ColorLevel+1)*10000;
        weigth+=item.LocalItemData._AllowLevel;
        return weigth;
    }
}
