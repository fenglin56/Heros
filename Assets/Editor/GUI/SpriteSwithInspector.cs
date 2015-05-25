//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright ?2011-2012 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Inspector class used to edit UISprites.
/// </summary>

[CustomEditor(typeof(SpriteSwith))]
public class SpriteSwithInspector : Editor
{
    SpriteSwith mSpriteSwith;
    UISprite mSprite;

	List<string> mRecord = new List<string>();

    void OnSelectAtlas(MonoBehaviour obj)
    {
        //UnityEngine.//Debug.Log("选择图集");
        if (mSpriteSwith.target != null)
        {
            NGUIEditorTools.RegisterUndo("Atlas Selection", mSpriteSwith.target);
            mSpriteSwith.target.atlas = obj as UIAtlas;
            mSpriteSwith.target.MakePixelPerfect();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(80f);
        mSpriteSwith = target as SpriteSwith;
        EditorGUILayout.BeginHorizontal();
		mRecord.Clear();
		if(mSpriteSwith.SpriteArray != null)
		{
			mRecord = mSpriteSwith.SpriteArray.ToList();
		}
        mSpriteSwith.SpriteCount = EditorGUILayout.IntField("精灵数量：",mSpriteSwith.SpriteCount);
        if (GUILayout.Button("确定")) 
		{
			mSpriteSwith.SpriteArray = new string[mSpriteSwith.SpriteCount]; 
			int newSpriteArrayLength = mSpriteSwith.SpriteCount;
			for(int i = 0;i<mRecord.Count;i++)
			{
				if(i <newSpriteArrayLength)
				{
					mSpriteSwith.SpriteArray[i] = mRecord[i];
				}
			}
		}
        EditorGUILayout.EndHorizontal();
        mSprite = EditorGUILayout.ObjectField("Sprite", mSpriteSwith.target, typeof(UISprite), true) as UISprite;

        if (mSpriteSwith.target != mSprite)
        {
            NGUIEditorTools.RegisterUndo("Image Button Change", mSpriteSwith);
            mSpriteSwith.target = mSprite;
        }

        if (mSprite != null)
        {
            ComponentSelector.Draw<UIAtlas>(mSprite.atlas, OnSelectAtlas);

            if (mSprite.atlas != null)
            {

                if (mSpriteSwith.SpriteArray == null)
                {
                    mSpriteSwith.SpriteArray = new string[mSpriteSwith.SpriteCount];
					int newSpriteArrayLength = mSpriteSwith.SpriteCount;
					for(int i = 0;i<mRecord.Count;i++)
					{
						if(i <newSpriteArrayLength)
						{
							mSpriteSwith.SpriteArray[i] = mRecord[i];
						}
					}
                }

                for (int i = 0; i < mSpriteSwith.SpriteArray.Length; i++)
                {
                    NGUIEditorTools.SpriteField("Sprite0" + i, mSprite.atlas, mSpriteSwith.SpriteArray[i], OnSelecte,i);
                }
            }
        }
    }

    void OnSelecte(string spriteName,int ArrayID)
    {
        NGUIEditorTools.RegisterUndo("Image Button Change", mSpriteSwith, mSpriteSwith.gameObject, mSprite);
        mSpriteSwith.SpriteArray[ArrayID] = spriteName;
        mSprite.spriteName = spriteName;
        mSprite.MakePixelPerfect();
        Repaint();
    }
}
