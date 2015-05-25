using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PageNumberTipsPanel : MonoBehaviour
{
    public GameObject TipsPrefab;

     List<SpriteSwith> TipsList = new List<SpriteSwith>();

    public void InitTips(float TipsNumber)
    {
        if (TipsNumber < 1)
            return;
        for (float i = 1; i <= TipsNumber; i++)
        {
            GameObject CreatTipsObj = UI.CreatObjectToNGUI.InstantiateObj(TipsPrefab,transform);
            CreatTipsObj.transform.localPosition = new Vector3(50 * (i - TipsNumber / 2-(TipsNumber%2/2)), 0, 0);
            TipsList.Add(CreatTipsObj.GetComponent<SpriteSwith>());
        }
    }

    public void SetActivePageID(int ID)
    {
        if (ID - 1 < 0 || ID-1 >= TipsList.Count)
            return;
        foreach (var child in TipsList)
        {
            child.ChangeSprite(1);
        }
        TipsList[ID - 1].ChangeSprite(2);
    }
}
