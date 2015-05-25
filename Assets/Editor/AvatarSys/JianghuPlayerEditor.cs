using UnityEngine;
using UnityEditor;
using System.Collections;

public class JianghuPlayerEditor : EditorWindow 
{
	static JianghuPlayerEditor window;
	string errorInfo;
	
	SkinMeshRenderHolder _database;
	
	GameObject playerModel;
	
	[MenuItem("Jianghu/PlayerEditor")]
	static void Execute()    
	{
		if (window == null)
		{
			window = (JianghuPlayerEditor)GetWindow(typeof(JianghuPlayerEditor));
		}
		window.Show();
	}
	
	void OnGUI()
	{
		GUILayout.Label("--- Target PlayerModel ---");
		
		GameObject newModel = (GameObject)EditorGUILayout.ObjectField("PlayerModel", playerModel, typeof(GameObject), true);
		if (newModel != playerModel)
		{
			#region a
			var modelClone = Object.Instantiate(newModel) as GameObject;
			bool hasWeaponAttachment = modelClone.transform.RecursiveObjectExist(ConstDefineManager.LBWeaponPos)
				|| modelClone.transform.RecursiveObjectExist(ConstDefineManager.RBWeaponPos)
					|| modelClone.transform.RecursiveObjectExist(ConstDefineManager.LHWeaponPos)
					|| modelClone.transform.RecursiveObjectExist(ConstDefineManager.RHWeaponPos);
			if (!hasWeaponAttachment)
			{
				if (EditorUtility.DisplayDialog("提示！", "模型没有武器挂载点", "ok", "cancel"))
				{
					playerModel = newModel;
				}
			}
			else
			{
				playerModel = newModel;
			}
			
			Object.DestroyImmediate(modelClone);
			#endregion
		}
		EditorGUILayout.Separator();
		if (GUILayout.Button("0-测试AssetBundle"))
		{
			_database = SkinMeshRenderHolder.CreateInstance<SkinMeshRenderHolder>();
			_database.name = "First";
			SetData();
		}

		if (GUILayout.Button("1-创建材质"))
		{
			if (newModel != null)
			{
				if (JianghuPlayerAssetManager_V2.GenerateMaterials(playerModel, out errorInfo))
				{
					EditorUtility.DisplayDialog("Character Generator", "创建材质成功", "ok", "cancel");
				}
				else
				{
					EditorUtility.DisplayDialog("Character Generator", "创建材质失败", "ok", "cancel");
				}
			}
			else
			{ 
				
			}
		}
		
		if (GUILayout.Button("2-分解动画"))
		{
			if (newModel != null)
			{
				if (JianghuPlayerAssetManager_V2.SplitAnimations(playerModel, out errorInfo))
				{
					EditorUtility.DisplayDialog("Character Generator", "分解动画成功", "ok", "cancel");
				}
				else
				{
					EditorUtility.DisplayDialog("Character Generator", "分解动画失败", "ok", "cancel");
				}
			}
			else
			{ 
				
			}
		}
		
		if (GUILayout.Button("3-生成角色基础预设件"))
		{
			if (newModel != null)
			{
				if (JianghuPlayerAssetManager_V2.GenerateRoleBaseAsset(playerModel, out errorInfo))
				{
					EditorUtility.DisplayDialog("Character Generator", "生成角色基础预设件成功", "ok", "cancel");
				}
				else
				{
					EditorUtility.DisplayDialog("Character Generator", "生成角色基础预设件失败", "ok", "cancel");
				}
			}
			else
			{ 
				
			}
		}
		
		//生成蒙皮物件
		if (GUILayout.Button("4-生成蒙皮物件"))
		{
			if (newModel != null)
			{
				if (JianghuPlayerAssetManager_V2.GenerateRoleSkinAsset(playerModel, out errorInfo))
				{
					EditorUtility.DisplayDialog("Character Generator", "生成蒙皮物件成功", "ok", "cancel");
				}
				else
				{
					EditorUtility.DisplayDialog("Character Generator", "生成蒙皮物件失败", "ok", "cancel");
				}
			}
			else
			{ 
				
			}
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.Separator();
		
		//生成一个角色
		if (GUILayout.Button("生成一个角色"))
		{
			RoleGenerate.GenerateRole("char01", "Char01_001", false, MESHDENSITY.HIGHT);
		}
		
		if (GUILayout.Button("生成一个角色(低模)"))
		{
			RoleGenerate.GenerateRole("Char01", "Char01_001", false, MESHDENSITY.LOW);
		}
		
		GUILayout.BeginHorizontal();
		
		GUILayout.EndHorizontal();
		GUILayout.Label(errorInfo);
	}
	
	private void SetData()
	{
		_database.name = "Second";
	}
}
