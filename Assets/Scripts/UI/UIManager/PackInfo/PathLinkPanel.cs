using UnityEngine;
using System.Collections;
using UI.MainUI;
using UI;

public class PathLinkPanel : MonoBehaviour {

    public SpriteSwith SpriteSwith_IconBG;
    public Transform Point_Icon;
    public UILabel Lable_itemName;
    public UILabel Ids_path;
    public PathLinkItem[]pathLinkItems;
    public bool Isshow;
    void Awake()
    {
        Ids_path.SetText(LanguageTextManager.GetString("IDS_I1_48"));
    }
    public void Show(ItemFielInfo itemFileInfo)
    {

        Point_Icon.ClearChild();
        Lable_itemName.SetText(NGUIColor.SetTxtColor(LanguageTextManager.GetString( itemFileInfo.LocalItemData._szGoodsName),(TextColor)itemFileInfo.LocalItemData._ColorLevel));
        CreatObjectToNGUI.InstantiateObj(itemFileInfo.LocalItemData._DisplayIdSmall,Point_Icon);
        SpriteSwith_IconBG.ChangeSprite(itemFileInfo.LocalItemData._ColorLevel+1);
        ResetPathLinkItem();
        for(int i=0;i<itemFileInfo.LocalItemData.LinkIds.Length;i++)
        {
            if(itemFileInfo.LocalItemData.LinkIds[i]!="0")
            {
            pathLinkItems[i].Show(itemFileInfo.LocalItemData.LinkIds[i]);
            pathLinkItems[i].gameObject.SetActive(true);
            }
            else
            {
                pathLinkItems[i].gameObject.SetActive(false);
            }
        }

    }
    void ResetPathLinkItem()
    {
        foreach(var item in pathLinkItems)
        {
            item.gameObject.SetActive(false);
        }
    }
}
