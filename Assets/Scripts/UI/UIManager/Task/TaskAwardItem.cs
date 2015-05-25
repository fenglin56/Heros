using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;

public class TaskAwardItem : MonoBehaviour {
    public SingleButtonCallBack ItemBtn;
    public Transform ItemIconPoint;
    public UILabel AwardAmount;
    public UILabel AwardTitle;
    private int m_GoodsID;
    void Awake()
    {
        if(ItemBtn!=null)
        {
            ItemBtn.SetCallBackFuntion(OnItemClick);
        }
    }
    void OnItemClick(object obj)
    {
        UI.MainUI.ItemInfoTipsManager.Instance.Show(m_GoodsID);
    }
    /// <summary>
    /// 初始化任务奖励信息信息
    /// </summary>
    /// <param name="itemFielInfo">Item fiel info.</param>
    public void InitItemData(ItemData itemData, int awardAmount)
    {
       
        if (itemData != null)
        {
            m_GoodsID=itemData._goodID;
            //装备图标
            if (ItemIconPoint.childCount > 0)
            {
                ItemIconPoint.ClearChild();
            }
            var skillIcon = CreatObjectToNGUI.InstantiateObj(itemData._picPrefab, ItemIconPoint);
            skillIcon.transform.localScale = new Vector3(90, 90, 1);


            AwardAmount.text ="+"+ awardAmount.ToString();
           var itemName=LanguageTextManager.GetString(itemData._szGoodsName);

            itemName=itemName.SetColor((TextColorType)itemData._ColorLevel);
            AwardTitle.text = itemName;
        }
        else
        {
            TraceUtil.Log(SystemModel.Rocky, TraceLevel.Error, "装备为空-- EquipItem->InitItemData");
        }
    }
    
}
