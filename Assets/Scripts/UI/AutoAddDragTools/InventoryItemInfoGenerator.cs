using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleItemConfig
{
    public GameObject Obj;
    public float LeftSpacing;

}

public class InventoryItemInfoGenerator 
{

	public static GameObject Generate(List<SingleItemConfig> configList, GameObject bgItem, float topMargin,  float itemSpacing, float bgWidth)
    {
        float totalLength = 0;

        float[] itemYposFromTopLeft = new float[configList.Count];
        float[] itemXposFromTopLeft = new float[configList.Count];
        for(int i = 0; i < configList.Count; i++)
        {
            //total length before add item
            if(0 == i)
            {
                totalLength += topMargin;
            }
            else
            {
                totalLength += itemSpacing;
            }

            //calculate item pos and total length after add item
            Vector3 widgetSize = NGUIMath.CalculateAbsoluteWidgetBounds(configList[i].Obj.transform).size;
            float xSize = widgetSize.x;
            float ySize = widgetSize.y;
            itemYposFromTopLeft[i] = totalLength + ySize/2;
            itemXposFromTopLeft[i] = configList[i].LeftSpacing + xSize/2;
            totalLength = totalLength + ySize;
        }
        totalLength += topMargin;

        //the root obj;
        GameObject rootObj = new GameObject();
        Transform  rootTransform = rootObj.transform;

        float topPosY = totalLength/2;
        float topPosX = -bgWidth/2;

        //add objects to root;
        for(int i  = 0; i < configList.Count; i++)
        {
            float posY = topPosY - itemYposFromTopLeft[i];
            float posX = topPosX + itemXposFromTopLeft[i];

            Transform itemTransform = configList[i].Obj.transform;
            itemTransform.parent = rootTransform;
            itemTransform.localPosition = new Vector3(posX, posY, -0.1f);
        }


        //make the bg item
        Transform bgTransform = bgItem.transform;
        bgTransform.parent = rootTransform;
        bgTransform.localPosition = Vector3.zero;
        bgTransform.localScale = new Vector3(bgWidth, totalLength, 1);


        BoxCollider collider =   rootObj.AddComponent<BoxCollider>();
        collider.center  = Vector3.zero;
        collider.size = new Vector3(bgWidth, totalLength, 1);
        rootObj.AddComponent<UIDragPanelContents>();

        return rootObj;
    }
}
