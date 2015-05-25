using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomEditor(typeof(SpriteSmoothFlag))]
public class SpriteSmoothInspector : Editor {

    SpriteSmoothFlag mSpriteSmoothFlag;

    public override void OnInspectorGUI()
    {
        mSpriteSmoothFlag = target as SpriteSmoothFlag;
        if (mSpriteSmoothFlag.TargetSprite == null)
        {
            mSpriteSmoothFlag.TargetSprite = mSpriteSmoothFlag.gameObject.GetComponent<UISprite>();
        }
        else
        {
            EditorGUILayout.ObjectField("Ä¿±ê:",mSpriteSmoothFlag.TargetSprite.gameObject,typeof(GameObject),null);
        }

    }
	
}
