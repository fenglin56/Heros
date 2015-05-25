using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;
using System.Linq;
using System.Text;

public class RoleSelectItem : MonoBehaviour, IPagerItem
{
    public SpriteSwith FocusBGSwitch;
    public UILabel RoleName;
    public UILabel RoleLevel;
    public UILabel RoleLevelNumber;
    public UISprite Jiangke_vocation;
    public UISprite Cike_vocation;
    [HideInInspector]
    public SSActorInfo ItemDataInfo;
    private Transform m_itemTransform;
    

    public void InitItemData(SSActorInfo? sSActorInfo)
    {
        if (sSActorInfo != null)
        {
            ItemDataInfo = sSActorInfo.Value;
            //this.RoleLevel.SetText(GetItemData(ItemDataType.Level));
            this.RoleLevelNumber.SetText(ItemDataInfo.lLevel.ToString());
            this.RoleName.SetText(GetItemData(ItemDataType.Name));

            if(ItemDataInfo.byKind == 1)
            {
                Jiangke_vocation.enabled = true;
                Cike_vocation.enabled = false;
            }
            else if(ItemDataInfo.byKind == 4)
            {
                Jiangke_vocation.enabled = false;
                Cike_vocation.enabled = true;
            }
        }
        else
        {
            RoleLevel.text = "";
            RoleLevel.text = "";
        }
    }    
    public void OnGetFocus()
    {
        if(ItemDataInfo.byKind == 1)
        {
            FocusBGSwitch.ChangeSprite(2);
        }
        else if(ItemDataInfo.byKind == 4)
        {
            FocusBGSwitch.ChangeSprite(4);
        }
    }

    public void OnLoseFocus()
    {
        if(ItemDataInfo.byKind == 1)
        {
            FocusBGSwitch.ChangeSprite(1);
        }
        else if(ItemDataInfo.byKind == 4)
        {
            FocusBGSwitch.ChangeSprite(3);
        }
    }

    public void OnBeSelected()
    {
    }
    public string GetItemData(ItemDataType itemDataType)
    {
        string itemData = string.Empty;
        switch (itemDataType)
        {
            case ItemDataType.Level:
               itemData = string.Format("{0}  {1}", LanguageTextManager.GetString("IDS_H1_119"), ItemDataInfo.lLevel.ToString());
                break;
            case ItemDataType.Name:
                itemData = ItemDataInfo.SZName;
                break;
        }
        return itemData;
    }

    public byte GetItemVocation()
    {
        return ItemDataInfo.byKind;
    }

    public Transform GetTransform()
    {
        if (m_itemTransform == null)
        {
            m_itemTransform = transform;
        }
        return m_itemTransform;
    }
    private string RoleToColumn(string value)
    {
        string trimValue = value.Trim();
        TraceUtil.Log(string.Format("{0},{1}",value ,value.Length));
        TraceUtil.Log(string.Format("{0},{1}", trimValue, trimValue.Length));
        
        if (string.IsNullOrEmpty(value))
            return string.Empty;
        var chars = trimValue.ToCharArray();
        StringBuilder newValue = new StringBuilder();
       
        foreach(var c in chars)
        {
            if (string.IsNullOrEmpty(c.ToString()))
            {
                continue;
            }
            newValue.AppendFormat("{0}\n", c.ToString());
        }
        newValue.Remove(chars.Length - 1, 1);

        return newValue.ToString().Trim();
    }
}
public enum ItemDataType
{
    Level,
    Name,

}
