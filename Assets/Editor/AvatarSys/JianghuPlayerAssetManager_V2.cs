using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using Object = UnityEngine.Object;

public class JianghuPlayerAssetManager_V2
{
    private const string M_MODELPREFIX = "JH_MOD_";
    private const string M_TEXPREFIX = "JH_TEX_";
    private const string PLAYERMODEL_RES_PATH = ConstDefineManager.RoleModelPath;
    private const string MATERIALS_PATH = ConstDefineManager.RoleMaterialPath;
    private const string TEXTURES_PATH = ConstDefineManager.TexturePath;
    private const string BASEMODEL_PATH = ConstDefineManager.RoleBaseAssetPath;
    private const string ANIMATION_PATH = ConstDefineManager.RoleAnimationPath;
    private const string SKINEDMESH_PATH = ConstDefineManager.RoleSkinPath;


	public JianghuPlayerAssetManager_V2()
    {
    }

    /// <summary>
    /// 生成目标模型的材质
	/// 低模和高模用同一套材质，所以只分离其中一套材质即可。这里只分离高模的
    /// </summary>
    /// <param name="targetModel"></param>
    /// <param name="errorInfo"></param>
    /// <returns></returns>
    public static bool GenerateMaterials(GameObject targetModel, out string errorInfo)
    {
		// 检查模型的正确性
        if (!CheckModelValid(targetModel, out errorInfo))
            return false;

		// 材质存放路径
        string matPath;
        if (string.IsNullOrEmpty(MATERIALS_PATH))
        {
            matPath = MaterialsPath(targetModel);
        }
        else
        {
            matPath = MATERIALS_PATH;

        }

        var modelName = targetModel.name.Replace(M_MODELPREFIX, "");
        matPath += modelName + "/";

        string texPath;
        if (string.IsNullOrEmpty(TEXTURES_PATH))
        {
            texPath = CharacterRoot(targetModel) + "textures";
        }
        else
        {
            texPath = TEXTURES_PATH;
        }

        if (!Directory.Exists(matPath))
        {
            Directory.CreateDirectory(matPath);
        }
        DeleteSpecFiles(matPath, modelName);

		//获取指定路径的所有textures(材质所需要的原textures)
        List<Texture2D> textures = EditorHelpers.CollectAll<Texture2D>(texPath);

        var smrs = targetModel.GetComponentsInChildren<SkinnedMeshRenderer>(true);

        foreach (var smr in smrs)
        {
			// 只用分离一套，所以过滤掉低模的
			if(smr.name.Contains("low")) continue;

            Texture2D normalMap = null;
            Texture2D hurtFlashTex = null;
            Texture2D burstFlashTex = null;

            foreach (var t in textures)
            {
                if (!t.name.ToLower().Contains("normal")) continue;
                if (!t.name.ToLower().Contains(smr.name.ToLower())) continue;

                normalMap = t;
                break;
            }

            foreach (var t in textures)
            {
                if (t.name.ToLower().Contains("hurtflash"))
                {
                    hurtFlashTex = t;
                    continue;
                }

                if (t.name.ToLower().Contains("burstflash"))
                {
                    burstFlashTex = t;
                    continue;
                }
            }



            foreach (var t in textures)
            {
                if (t.name.ToLower().Contains("normal")) continue;
                if (!t.name.ToLower().Contains(smr.name.ToLower())) continue;

                string materialPath = matPath + smr.name.ToLower() + ".mat";
                if (File.Exists(materialPath)) continue;

                string shader = normalMap != null ? ConstDefineManager.BumpShaderName
                    : ConstDefineManager.BaseShaderName;

                Material m = new Material(Shader.Find(shader));

                m.SetColor("_Color", new Color(0.6f, 0.6f, 0.6f));
                
                m.SetTexture("_MainTex", t);
                if (normalMap != null) m.SetTexture("_BumpMap", normalMap);
                if (hurtFlashTex != null) m.SetTexture("_RimLightTex", hurtFlashTex);
                if (burstFlashTex != null) m.SetTexture("_BurstStepTex", burstFlashTex);

                AssetDatabase.CreateAsset(m, materialPath);
            }
        }

        return true;
    }

    /// <summary>
    /// 分离动画
    /// </summary>
    /// <param name="targetModel"></param>
    /// <param name="errorInfo"></param>
    /// <returns></returns>
    public static bool SplitAnimations(GameObject targetModel, out string errorInfo)
    {
        if (!CheckModelValid(targetModel, out errorInfo))
        {
            return false;
        }
        var modelName = targetModel.name.Replace(M_MODELPREFIX, "");
        string animPath = ANIMATION_PATH + modelName + "/";

        if (!Directory.Exists(animPath))
        {
            Directory.CreateDirectory(animPath);
        }
        DeleteSpecFiles(animPath);

		// 找到所有相应的有动画的模型
        var animModels = FindAllAssetAtPath(PLAYERMODEL_RES_PATH, modelName);

        List<AnimationClip> animationClips = new List<AnimationClip>();
        animModels.ApplyAllItem(P =>
        {
            animationClips.AddRange(GetAnimsFromGO(P));
        });
        System.Text.StringBuilder aniName = new System.Text.StringBuilder();
        foreach (var clip in animationClips)
        {
            var clipName = animPath + clip.name.Replace("(Clone)", "");
            aniName.Append(clip.name.Replace("(Clone)", "") + "+");
            var clipPath = clipName + ".anim";
            if (clip != null)
            {
                AssetDatabase.CreateAsset(clip, clipPath);
            }
        }
        if (aniName.Length > 0) aniName.Remove(aniName.Length - 1, 1);

        return true;
    }



    /// <summary>
    /// 生成角色基础预设件
    /// </summary>
    /// <param name="targetModel"></param>
    /// <param name="errorInfo"></param>
    /// <returns></returns>
    public static bool GenerateRoleBaseAsset(GameObject targetModel, out string errorInfo)
    {
        if (!CheckModelValid(targetModel, out errorInfo))
            return false;

        string roleBasePath = BASEMODEL_PATH;
        var modelName = targetModel.name.Replace(M_MODELPREFIX, "");

        if (!Directory.Exists(roleBasePath))
        {
            Directory.CreateDirectory(roleBasePath);
        }
        DeleteSpecFiles(roleBasePath, modelName);

        var modelClone = Object.Instantiate(targetModel) as GameObject;
        if (modelClone != null)
        {
            var animator = modelClone.GetComponent<Animator>();
            Object.DestroyImmediate(animator);
            var animation = modelClone.GetComponent<Animation>();
            Object.DestroyImmediate(animation);

			var smrs = modelClone.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (var smr in smrs)
            {
                Object.DestroyImmediate(smr.gameObject);
            }
            GameObject SkinnedMesh = new GameObject("SkinnedMesh", typeof(SkinnedMeshRenderer));
            SkinnedMesh.transform.parent = modelClone.transform;
            GetPrefab(modelClone, roleBasePath + modelName + ".prefab");
        }
        return true;
    }

    /// <summary>
    /// 生成角色蒙皮预设件
    /// </summary>
    /// <param name="targetModel"></param>
    /// <param name="errorInfo"></param>
    /// <returns></returns>
    public static bool GenerateRoleSkinAsset(GameObject targetModel, out string errorInfo)
    {
        if (!CheckModelValid(targetModel, out errorInfo))
            return false;
        var modelName = targetModel.name.Replace(M_MODELPREFIX, "");

        string roleSkinPath = SKINEDMESH_PATH + modelName + "/";

        if (!Directory.Exists(roleSkinPath))
        {
            Directory.CreateDirectory(roleSkinPath);
        }
        DeleteSpecFiles(roleSkinPath);


        List<Material> materials = EditorHelpers.CollectAll<Material>(MATERIALS_PATH + modelName);

        foreach (var smr in targetModel.GetComponentsInChildren<SkinnedMeshRenderer>(true))
        {

            SkinMeshRenderHolder smrHolder = ScriptableObject.CreateInstance<SkinMeshRenderHolder>();
            smrHolder.Mats = new List<Material>();
            smrHolder.Bones = new List<string>();

            GameObject rendererClone = (GameObject)PrefabUtility.InstantiatePrefab(smr.gameObject);
            GameObject rendererParent = rendererClone.transform.parent.gameObject;
            rendererClone.transform.parent = null;
            Object.DestroyImmediate(rendererParent);
            Object rendererPrefab = GetPrefab(rendererClone, roleSkinPath + smr.name + "_smr.prefab");
            smrHolder.SMRGO = rendererPrefab;

            foreach (Material m in materials)
			{
				string smrTemp;
				Debug.Log("smr.name = " + smr.name);
				if(smr.name.Contains("Low"))
				{
					smrTemp = smr.name.Substring(0, smr.name.LastIndexOf("_"));
				}
				else
				{
					smrTemp = smr.name;
				}

				//string smrTemp = smr.name.Substring(0, smr.name.LastIndexOf("_"));
				//smrTemp = smr.name.Substring(0, "Char01_001_low".LastIndexOf("_"));
				//Debug.Log("smrTemp = " + smrTemp);
				//Debug.Log("m = " + m.name);
				if(m.name.Contains(smrTemp.ToLower())) 
				{
					smrHolder.Mats.Add(m);
				}
				//if (m.name.Contains(smr.name.ToLower())) smrHolder.Mats.Add(m);
			}
			//Debug.Log("smrHolder.mats = " + smrHolder.Mats.Count);

            //List<string> boneNames = new List<string>();   //字符串列表，记录子蒙皮模型对应的骨骼名称清单。
            foreach (Transform t in smr.bones)
                smrHolder.Bones.Add(t.name);

            AssetDatabase.CreateAsset(smrHolder, roleSkinPath + smr.name + ".asset");
        }

        return true;
    }


    /// <summary>
    /// 根据配置生成角色(默认高模)
    /// </summary>
    /// <returns></returns>
    public static void GenerateRole(string roleName, string avatarName)
    {
		Debug.Log("两个参数");
        var basePath = BASEMODEL_PATH + roleName + ".prefab";
        //var avatarPath = SKINEDMESH_PATH + roleName + "/" + avatarName + ".asset";
		var avatarPath = SKINEDMESH_PATH + roleName + "/" + avatarName + "_low" + ".asset";

        var baseGo = AssetDatabase.LoadAssetAtPath(basePath, typeof(GameObject)) as GameObject;
        if (baseGo != null)
        {
            var avatar = AssetDatabase.LoadAssetAtPath(avatarPath, typeof(SkinMeshRenderHolder)) as SkinMeshRenderHolder;
            if (avatar != null)
            {
                var baseClone = Object.Instantiate(baseGo) as GameObject;

                List<CombineInstance> combineInstances = new List<CombineInstance>();
                List<Material> materials = new List<Material>();
                List<Transform> bones = new List<Transform>();
                Transform[] transforms = baseClone.GetComponentsInChildren<Transform>();

                GameObject go = (GameObject)Object.Instantiate(avatar.SMRGO);
                go.renderer.materials = avatar.Mats.ToArray();

                var smr = (SkinnedMeshRenderer)go.renderer;

                materials.AddRange(smr.sharedMaterials);
                for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
                {
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = smr.sharedMesh;
                    ci.subMeshIndex = sub;
                    combineInstances.Add(ci);
                }

                foreach (string bone in avatar.Bones)
                {
                    foreach (Transform transform in transforms)
                    {
                        if (transform.name != bone) continue;
                        bones.Add(transform);
                        break;
                    }
                }
                Transform smrTr;

                baseClone.transform.RecursiveFindObject("SkinnedMesh", out smrTr);

                SkinnedMeshRenderer r = smrTr.GetComponent<SkinnedMeshRenderer>();
                r.sharedMesh = new Mesh();
                r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
                r.bones = bones.ToArray();
                r.materials = materials.ToArray();


                AttachAnimation(baseClone, "daoke", "run", "run", "idle");

                Object.DestroyImmediate(go);
            }
        }
    }


    private static void AttachAnimation(GameObject root, string roleName, string defaultAnim, params string[] anims)
    {
        Animation anim = root.GetComponent<Animation>();
        if (anim == null)
        {
            anim = root.AddComponent<Animation>();
        }
        else
        {
            List<string> stateName = new List<string>();
            foreach (AnimationState state in anim)
            {
                stateName.Add(state.name);
            }

            stateName.ApplyAllItem(P => anim.RemoveClip(P));
        }

        foreach (var s in anims)
        {
            string aniPath = GetResourcePath("anim") + roleName + "/" + s;
            //var clipGO = Resources.Load(aniPath);
            var aniClip = Resources.Load(aniPath) as AnimationClip;
            if (aniClip != null)
            {
                root.animation.AddClip(aniClip, s);
            }
        }
        root.animation.CrossFade(defaultAnim);

    }

    private static string GetResourcePath(string resourceName)
    {
        string path = string.Empty;
        switch (resourceName)
        {
            case "root":
                path = ConstDefineManager.RoleBaseAssetPath;
                break;
            case "smr":
                path = ConstDefineManager.RoleSkinPath;
                break;
            case "anim":
                path = ConstDefineManager.RoleAnimationPath;
                break;
        }
        path = path.Replace("Resources/", "|");
        path = path.Substring(path.IndexOf("|") + 1);

        return path;
    }
    /// <summary>
    /// 官方换装原始代码
    /// </summary>
    public static void GenerateMaterials()
    {
        bool validMaterial = false;
        List<Object> playerModels = EditorHelpers.CollectAll<Object>(PLAYERMODEL_RES_PATH);

        //Debug.Log(playerModels.Count);

        foreach (Object o in playerModels)
        {
            if (!(o is GameObject)) continue;
            if (o.name.Contains("@")) continue;
            if (!AssetDatabase.GetAssetPath(o).Contains("/characters/")) continue;

            GameObject characterFbx = (GameObject)o;

            // Create directory to store generated materials.
            if (!Directory.Exists(MaterialsPath(characterFbx)))
                Directory.CreateDirectory(MaterialsPath(characterFbx));

            // Collect all textures.
            List<Texture2D> textures = EditorHelpers.CollectAll<Texture2D>(CharacterRoot(characterFbx) + "textures");

            // Create materials for each SkinnedMeshRenderer.
            foreach (SkinnedMeshRenderer smr in characterFbx.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                // Check if this SkinnedMeshRenderer has a normalmap.
                Texture2D normalmap = null;
                foreach (Texture2D t in textures)
                {
                    if (!t.name.ToLower().Contains("normal")) continue;
                    if (!t.name.ToLower().Contains(smr.name.ToLower())) continue;
                    normalmap = t;
                    break;
                }

                // Create a Material for each texture which name contains
                // the SkinnedMeshRenderer name.
                foreach (Texture2D t in textures)
                {
                    if (t.name.ToLower().Contains("normal")) continue;
                    if (!t.name.ToLower().Contains(smr.name.ToLower())) continue;

                    validMaterial = true;
                    string materialPath = MaterialsPath(characterFbx) + "/" + t.name.ToLower() + ".mat";

                    // Dont overwrite existing materials, as we would
                    // lose existing material settings.
                    if (File.Exists(materialPath)) continue;

                    // Use a default shader according to artist preferences.
                    string shader = "Specular";
                    if (normalmap != null) shader = "Bumped Specular";

                    // Create the Material
                    Material m = new Material(Shader.Find(shader));
                    m.SetTexture("_MainTex", t);
                    if (normalmap != null) m.SetTexture("_BumpMap", normalmap);
                    AssetDatabase.CreateAsset(m, materialPath);
                }
            }
        }
        AssetDatabase.Refresh();

        if (!validMaterial)
            EditorUtility.DisplayDialog("Character Generator", "No Materials created.", "Ok");
    }

    public static bool CreateAssetBundles(GameObject targetModel, out string errorInfo)
    {
        if (!CheckModelValid(targetModel, out errorInfo))
            return false;
        var modelName = targetModel.name.Replace(M_MODELPREFIX, "");
        string name = modelName.ToLower();

        string baseModelPath;
        if (string.IsNullOrEmpty(BASEMODEL_PATH))
        {
            baseModelPath = CharacterRoot(targetModel) + "Prefabs";
        }
        else
        {
            baseModelPath = BASEMODEL_PATH;

        }
        if (!Directory.Exists(baseModelPath))
        {
            Directory.CreateDirectory(baseModelPath);
        }
        string[] existPrefabs = Directory.GetFiles(baseModelPath);
        foreach (var filename in existPrefabs)
        {
            if (filename.EndsWith(".prefab") && filename.Contains(name))
            {
                File.Delete(name);
            }
        }
        return true;
    }
    static Object GetPrefab(GameObject go, string path)
    {
        Object tempPrefab = PrefabUtility.CreateEmptyPrefab(path);
        tempPrefab = PrefabUtility.ReplacePrefab(go, tempPrefab);
        Object.DestroyImmediate(go);
        return tempPrefab;
    }
    // Returns the path to the directory that holds the specified FBX.
    static string CharacterRoot(GameObject character)
    {
        string root = AssetDatabase.GetAssetPath(character);
        return root.Substring(0, root.Substring(0, root.LastIndexOf('/')).LastIndexOf('/') + 1);
    }
    public static string MaterialsPath(GameObject character)
    {
        // we will use it only for file enumeration, and separator will be appended for us
        // if we do append here, AssetDatabase will be confused
        return CharacterRoot(character) + "Per Texture Materials";
    }



    private static bool CheckModelValid(GameObject targetModel, out string errorInfo)
    {
        var modelName = targetModel.name.Replace(M_MODELPREFIX, "");

        errorInfo = string.Empty;
        if (!(targetModel is GameObject))
        {
            errorInfo = "模型不是GameObject";
            return false;
        }
        if (modelName.Contains("@"))
        {
            errorInfo = "模型类型不正确";
            return false;
        }
        return true;
    }
    private static IEnumerable<GameObject> FindAllAssetAtPath(string path)
    {
        var fileNames = Directory.GetFiles(path);
        fileNames = fileNames.Where(P => !P.Contains("meta") && P.Contains("@")).ToArray();

        var gos = fileNames.Select(P =>
        {
            var go = AssetDatabase.LoadAssetAtPath(P, typeof(GameObject)) as GameObject;
            return go;
        });

        return gos;
    }
    private static IEnumerable<GameObject> FindAllAssetAtPath(string path, string filteModelName)
    {
        var dos = FindAllAssetAtPath(path);

        return dos.Where(P => P.name.Contains(filteModelName));
    }
    private static List<AnimationClip> GetAnimsFromGO(GameObject go)
    {
        List<AnimationClip> returnClips = new List<AnimationClip>();
        Animation anim = null;
        var goInstance = Object.Instantiate(go) as GameObject;
        if (goInstance != null)
        {
            anim = goInstance.GetComponent<Animation>();
            var clips = AnimationUtility.GetAnimationClips(anim);
            foreach (var clip in clips)
            {
                var tempClip = Object.Instantiate(clip) as AnimationClip;
                tempClip.name = clip.name;
                returnClips.Add(tempClip);
            }
            GameObject.DestroyImmediate(goInstance);
            return returnClips;
        }
        return null;
    }
    private static void DeleteSpecFiles(string dirPath)
    {
        DeleteSpecFiles(dirPath, string.Empty);
    }
    private static void DeleteSpecFiles(string dirPath, string filterName)
    {
        var fileNames = Directory.GetFiles(dirPath);
        fileNames = fileNames.Where(P => !P.Contains("meta") && P.Contains("@") && (string.IsNullOrEmpty(filterName) ? true : P.Contains(filterName))).ToArray();

        foreach (string fileName in fileNames)
        {
            File.Delete(fileName);
        }
    }
}
//分离模型后，在组装的时候会这么做：

/*
 
 * 
 * 
 * 
 */
