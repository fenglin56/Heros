using UnityEngine;
using UnityEditor;
public static class EditorUtils
{

	public static T CreateScriptable<T>() where T: ScriptableObject
	{
		T newScriptable = ScriptableObject.CreateInstance<T>();
		string path = AssetDatabase.GetAssetPath(Selection.activeObject);
		////Debug.Log(path);
		if(path.Length == 0)
		{
			path = "Assets/";
		}
		string className = typeof(T).Name;
		path = AssetDatabase.GenerateUniqueAssetPath(path+"/new"+className+".asset");
		////Debug.Log("path = "+path);
		AssetDatabase.CreateAsset(newScriptable,path);
		return newScriptable;
	}
	
	[MenuItem("Tools/Sound/Create SoundClipList")]
	public static void CreateSoundClipList()
	{
		EditorUtils.CreateScriptable<SoundClipList>();
	}
	
	[MenuItem("Tools/Build/Create BundleLevelDataBase")]
	public static void CreateBundleLevelDataBase()
	{
		EditorUtils.CreateScriptable<BundleLevelDataBase>();	
	}
	
}
[System.AttributeUsage(System.AttributeTargets.Property|
    System.AttributeTargets.Field)]
public class GameDataPostFlag : System.Attribute
{
    public bool ClearFlag{get;private set;}
    public GameDataPostFlag(bool clearFlag)
    {
        this.ClearFlag= clearFlag;
    }
}

