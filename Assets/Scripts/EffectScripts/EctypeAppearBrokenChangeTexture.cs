using UnityEngine;
using System.Collections;

public class EctypeAppearBrokenChangeTexture : MonoBehaviour 
{
    //public Transform Texture_parent;

    public UISlicedSprite SlicedSprite;

    public void ChangeTexture(string spriteName)
    {
        //var children = Texture_parent.GetComponentsInChildren<Renderer>();
        //children.ApplyAllItem(p =>
        //    {
        //        p.material.mainTexture = texture;
        //    });
        SlicedSprite.spriteName = spriteName;
    }
}
