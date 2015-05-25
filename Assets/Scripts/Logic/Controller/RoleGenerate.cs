using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System;
using UI.MainUI;

//网格密度，默认为高
public enum MESHDENSITY
{
	HIGHT = 1,
	MIDDLE,
	LOW,
}

public class RoleGenerate
{
	public const string BASEMODEL_PATH = ConstDefineManager.RoleBaseAssetPath;
	public const string ANIMATION_PATH = ConstDefineManager.RoleAnimationPath;
	public const string SKINEDMESH_PATH = ConstDefineManager.RoleSkinPath;
	
	/// <summary>
	/// 根据配置生成新角色对象
	/// </summary>
	/// <returns></returns>
	public static GameObject GenerateRole(string roleName, string avatarName,bool isHero)
	{
		var basePath = GetResourcePath("root") + roleName;
		
		var baseGo = Resources.Load(basePath) as GameObject;
		
		GameObject root = null;
		if (baseGo != null)
		{
			root = UnityEngine.Object.Instantiate(baseGo) as GameObject;
			root.name = roleName;
			
			GenerateRole(root, avatarName);
			
			//if (isHero)
			{
				var cc = root.AddComponent<CharacterController>();
				var height = cc.height;
				cc.height = 15;
				cc.radius = 3;
				cc.center = new Vector3(cc.center.x, 7.58f, cc.center.z);
				
				if (isHero)
				{
					var nav = root.AddComponent<NavMeshAgent>();
					nav.walkableMask = 1;
					nav.speed = 17f;
					nav.acceleration = 350f;
					nav.angularSpeed = 360f;
					nav.enabled = false;
				}
			}
			
		}
		return root;
	}
	public static GameObject GenerateRole(string roleName, string avatarName)
	{
		return GenerateRole(roleName, avatarName, true);
	}
	/// <summary>
	/// 已有角色换装
	/// </summary>
	/// <param name="root"></param>
	/// <param name="avatarName"></param>
	/// <returns></returns>
	public static GameObject GenerateRole(GameObject root, string avatarName)
	{
		if (root != null)
		{
			var avatarPath = GetResourcePath("smr") + root.name + "/" + avatarName;
			var avatar = Resources.Load(avatarPath) as SkinMeshRenderHolder;
			if (avatar != null)
			{
				Transform smrTr;
				
				root.transform.RecursiveFindObject("SkinnedMesh", out smrTr);
				List<CombineInstance> combineInstances = new List<CombineInstance>();
				List<Material> materials = new List<Material>();
				List<Transform> bones = new List<Transform>();
				Transform[] transforms = root.GetComponentsInChildren<Transform>();
				
				GameObject go = (GameObject)UnityEngine.Object.Instantiate(avatar.SMRGO);
				go.renderer.materials = avatar.Mats.ToArray();
				
				var smr = (SkinnedMeshRenderer)go.renderer;
				
				materials.AddRange(smr.materials);
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
				
				SkinnedMeshRenderer r = smrTr.GetComponent<SkinnedMeshRenderer>();
				r.sharedMesh = new Mesh();
				r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
				r.bones = bones.ToArray();
				r.materials = materials.ToArray();
				Vector3 localBoundsSize = r.localBounds.size;
				//localBoundsSize.y *=10;
				Bounds newBounds = new Bounds(r.localBounds.center, localBoundsSize);
				r.localBounds = newBounds;
				
				UnityEngine.Object.DestroyImmediate(go);
			}
			
			var playerBehaviour = root.GetComponent<PlayerBehaviour>();
			if (playerBehaviour != null)
			{
				playerBehaviour.RefreshRenderCach();
			}
		}
		
		return root;
	}
	
	/// <summary>
	/// 生成一个角色，传入模型网格密度
	/// </summary>
	/// <returns>The role.</returns>
	/// <param name="roleName">Role name.</param>
	/// <param name="avatarName">Avatar name.</param>
	/// <param name="isHero">If set to <c>true</c> is hero.</param>
	/// <param name="meshDensity">Mesh density.</param>
	public static GameObject GenerateRole(string roleName, string avatarName, bool isHero, MESHDENSITY meshDensity)
	{
		var basePath = GetResourcePath("root") + roleName;
		
		var baseGo = Resources.Load(basePath) as GameObject;
		
		if (baseGo == null) Debug.Log("baseGo is null!");
		
		GameObject root = null;
		if (baseGo != null)
		{
			root = UnityEngine.Object.Instantiate(baseGo) as GameObject;
			root.name = roleName;
			
			GenerateRole(root, avatarName, meshDensity);
			
			var cc = root.AddComponent<CharacterController>();
			var height = cc.height;
			cc.height = 15;
			cc.radius = 3;
			cc.center = new Vector3(cc.center.x, 7.58f, cc.center.z);
			
			if (isHero)
			{
				var nav = root.AddComponent<NavMeshAgent>();
				nav.walkableMask = 1;
				nav.speed = 17f;
				nav.acceleration = 350f;
				nav.angularSpeed = 360f;
				nav.enabled = false;
			}
		}
		
		return root;
	}
	
	/// <summary>
	/// 生成角色,传入模型网格密度
	/// </summary>
	/// <returns>The role.</returns>
	/// <param name="root">Root.</param>
	/// <param name="avatarName">Avatar name.</param>
	/// <param name="meshDensity">Mesh density.</param>
	public static GameObject GenerateRole(GameObject root, string avatarName, MESHDENSITY meshDensity)
	{
		if (root != null)
		{
			string avatarPath = string.Empty;
			if (meshDensity == MESHDENSITY.HIGHT)
				avatarPath = GetResourcePath("smr") + root.name + "/" + avatarName;
			else if (meshDensity == MESHDENSITY.MIDDLE)
				avatarPath = GetResourcePath("smr") + root.name + "/" + avatarName + "_middle";
			else
				avatarPath = GetResourcePath("smr") + root.name + "/" + avatarName + "_low";
			
			var avatar = Resources.Load(avatarPath) as SkinMeshRenderHolder;
			
			if (avatar == null)
			{
				Debug.Log("avatar is null!!");
			}
			
			if (avatar != null)
			{
				Transform smrTr;
				
				root.transform.RecursiveFindObject("SkinnedMesh", out smrTr);
				List<CombineInstance> combineInstances = new List<CombineInstance>();
				List<Material> materials = new List<Material>();
				List<Transform> bones = new List<Transform>();
				Transform[] transforms = root.GetComponentsInChildren<Transform>();
				
				GameObject go = (GameObject)UnityEngine.Object.Instantiate(avatar.SMRGO);
				go.renderer.materials = avatar.Mats.ToArray();
				
				var smr = (SkinnedMeshRenderer)go.renderer;
				
				//获取材质
				materials.AddRange(smr.materials);
				
				//获取网格
				for (int sub = 0; sub < smr.sharedMesh.subMeshCount; sub++)
				{
					CombineInstance ci = new CombineInstance();
					ci.mesh = smr.sharedMesh;
					ci.subMeshIndex = sub;
					combineInstances.Add(ci);
				}
				
				//获取骨骼
				foreach (string bone in avatar.Bones)
				{
					foreach (Transform transform in transforms)
					{
						if (transform.name != bone) continue;
						bones.Add(transform);
						break;
					}
				}
				
				SkinnedMeshRenderer r = smrTr.GetComponent<SkinnedMeshRenderer>();
				r.sharedMesh = new Mesh();
				r.sharedMesh.CombineMeshes(combineInstances.ToArray(), false, false);
				r.bones = bones.ToArray();
				r.materials = materials.ToArray();
				Vector3 localBoundsSize = r.localBounds.size;
				
				Bounds newBounds = new Bounds(r.localBounds.center, localBoundsSize);
				r.localBounds = newBounds;
				
				UnityEngine.Object.DestroyImmediate(go);
				
			}
			
			var playerBehaviour = root.GetComponent<PlayerBehaviour>();
			if (playerBehaviour != null)
			{
				playerBehaviour.RefreshRenderCach();
			}
		}
		
		return root;
	}
	
	/// <summary>
	/// 附加动画
	/// </summary>
	/// <param name="root">主角对象</param>
	/// <param name="roleName">角色名</param>
	/// <param name="anims">多个动画名...</param>
	public static void AttachAnimation(GameObject root, string roleName, string defaultAnim, params string[] anims)
	{
		Animation anim = root.GetComponent<Animation>();
		if (anim == null)
		{
			anim = root.AddComponent<Animation>();
		}
		else
		{
			List<string> stateName=new List<string>();
			foreach(AnimationState state in anim)
			{
				stateName.Add(state.name);
			}
			
			stateName.ApplyAllItem(P=>anim.RemoveClip(P));
		}
		
		foreach (var s in anims)
		{
			string aniPath = GetResourcePath("anim") + roleName + "/" + s;
			var aniClip = Resources.Load(aniPath) as AnimationClip;
			
			if (aniClip != null)
			{
				root.animation.AddClip(aniClip, s);
				//root.animation[s].wrapMode = aniClip.wrapMode == WrapMode.Default ? WrapMode.Once : aniClip.wrapMode;
			}
		}
	}
	/// <summary>
	/// 挂载技能
	/// </summary>
	/// <param name="root"></param>
	public static SkillBase AttachSkill(GameObject root, int skillId)
	{
		var skillBase=root.AddComponent<SkillBase>();
		
		skillBase.SkillId= skillId;
		
		skillBase.Init(skillId);
		
		return skillBase;
	}
	/// <summary>
	/// 武器更换
	/// </summary>
	/// <param name="root"></param>
	/// <param name="weapon"></param>
	/// <returns></returns>
	public static bool ChangeWeapon(GameObject root, GameObject weapon,GameObject WeaponEff)
	{
		bool flag = false;
		if(root!=null)
		{
			var VeaponCurrentPoint = GetPlayerWeaponCurrentWeaponPoint(root);
			flag = AttachWeapon(root,VeaponCurrentPoint,weapon,WeaponEff);
		}        
		return flag;
	}
	/// <summary>
	/// 挂载武器
	/// </summary>
	/// <param name="root">主角对象</param>
	/// <param name="attachPoint">挂载点</param>
	/// <param name="weapon">武器对象</param>
	/// <returns></returns>
	public static bool AttachWeapon(GameObject root, AttachPoint attachPoint, GameObject weapon,GameObject WeaponEff)
	{
		bool flag = false;
		Transform attachment=null;
		string parentName = attachPoint.GetAttachment();
		
		if (!string.IsNullOrEmpty(parentName))
		{
			flag = root.transform.RecursiveFindObject(parentName, out attachment);
			if (flag)
			{
				var m_child = UnityEngine.Object.Instantiate(weapon) as GameObject;
				m_child.name = weapon.name;
				
				attachment.ClearChild();
				
				// Parent the child to this object
				
				Transform ct = m_child.transform;
				ct.parent = attachment;
				// Reset the pos/rot/scale, just in case
				ct.localPosition = Vector3.zero;
				ct.localRotation = Quaternion.identity;
				ct.localScale = Vector3.one;
				AttachWeaponEff(m_child,WeaponEff);
			}
		}
		
		return flag;
	}
	
	public static void AttachWeaponEff(GameObject root,GameObject WeaponEff)
	{
		bool flag = false;
		Transform attachment=null;
		string parentName ="WeaponEffPoint";
		
		if(WeaponEff==null)
		{
			return ;
		}
		
		if (!string.IsNullOrEmpty(parentName))
		{
			flag = root.transform.RecursiveFindObject(parentName, out attachment);
			if (flag)
			{
				var m_child = UnityEngine.Object.Instantiate(WeaponEff) as GameObject;
				m_child.name = WeaponEff.name;
				
				attachment.ClearChild();
				
				// Parent the child to this object
				
				Transform ct = m_child.transform;
				ct.parent = attachment;
				// Reset the pos/rot/scale, just in case
				ct.localPosition = Vector3.zero;
				ct.localRotation = Quaternion.identity;
				ct.localScale = Vector3.one;
			}
		}
		
		
	}
	/// <summary>
	/// 武器多个挂载点
	/// </summary>
	/// <param name="root"></param>
	/// <param name="attachPoints"></param>
	/// <param name="weapon"></param>
	/// <returns></returns>
	public static bool AttachWeapon(GameObject root, string[] attachPoints, GameObject weapon,GameObject WeaponEff)
	{       
		bool flag = false;
		if (weapon != null)
		{
			ClearPlayerWeapon(root);
			
			AttachPoint attackPoint;
			bool hasConfig = false;
			foreach (string pos in attachPoints)
			{
				if (Enum.IsDefined(typeof(AttachPoint), pos))
				{
					attackPoint = (AttachPoint)Enum.Parse(typeof(AttachPoint), pos);
					AttachWeapon(root, attackPoint, weapon,WeaponEff);
					hasConfig = true;
				}
				else
				{
					TraceUtil.Log(pos);
				}
			}
			
			if (!hasConfig)
			{
				attackPoint = AttachPoint.RHWeapon;
				RoleGenerate.AttachWeapon(root, attackPoint, weapon,WeaponEff);
			}
			
		}
		var playerBehaviour = root.GetComponent<PlayerBehaviour>();
		if (playerBehaviour != null&&PlayerManager.Instance.m_playerHideFlag)
		{
			//playerBehaviour.RefreshRenderCach();
			PlayerManager.Instance.HideAllPlayerButHero(true);
		}
		return flag;
	}
	/// <summary>
	/// 角色已有武器挂载点更换
	/// </summary>
	/// <param name="root"></param>
	/// <param name="attachPoints"></param>
	/// <returns></returns>
	public static bool AttachWeapon(GameObject root, string[] attachPoints)
	{
		var currentWeaponName = GetPlayerCurrentWeapon(root);
		GameObject weapon;
		if (string.IsNullOrEmpty(currentWeaponName)&&PlayerFactory.Instance.Weapons.Length>0)
		{
			var WeaponInfo = UI.MainUI.ContainerInfomanager.Instance.GetSSyncContainerGoods_SCList(1).SingleOrDefault(P => P.nPlace == 0);
			if(WeaponInfo .uidGoods==0)
			{
				weapon = PlayerFactory.Instance.Weapons[0];
			}
			else
			{
				currentWeaponName = UI.MainUI.ContainerInfomanager.Instance.GetContainerGoodsInfo(WeaponInfo).LocalItemData._ModelId;
				weapon = PlayerFactory.Instance.Weapons.SingleOrDefault(P => P.name == currentWeaponName);
			}
		}
		else
		{
			weapon = PlayerFactory.Instance.Weapons.SingleOrDefault(P => P.name == currentWeaponName);
		}
		if (weapon != null)
		{
			return AttachWeapon(root, attachPoints, weapon,null);
		}
		else
		{
			return false;
		}
	}
	/// <summary>
	/// 清除角色所有武器
	/// </summary>
	/// <param name="root"></param>
	private static void ClearPlayerWeapon(GameObject root)
	{
		var transform = root.transform;
		Transform weapontParent;
		var attachPoints = Enum.GetValues(typeof(AttachPoint));
		if (transform != null)
		{
			foreach (AttachPoint attachPoint in attachPoints)
			{
				string parentName = attachPoint.GetAttachment();
				if (transform.RecursiveFindObject(parentName, out weapontParent))
				{
					weapontParent.ClearChild();
				}
			}
		}
	}
	/// <summary>
	/// 获得角色当前使用武器
	/// </summary>
	/// <param name="root"></param>
	/// <returns></returns>
	private static string GetPlayerCurrentWeapon(GameObject root)
	{
		var transform = root.transform;
		Transform weapontParent;
		var attachPoints = Enum.GetValues(typeof(AttachPoint));
		if (transform != null)
		{
			foreach (AttachPoint point in attachPoints)
			{
				if (transform.RecursiveFindObject(point.GetAttachment(), out weapontParent))
				{
					if (weapontParent.childCount > 0)
					{
						return weapontParent.GetChild(0).name.Replace("(clone)","");
					}
				}
			}
		}
		return string.Empty;
	}
	/// <summary>
	/// 获得玩家当前武器挂载点
	/// </summary>
	/// <param name="root"></param>
	/// <returns></returns>
	private static string[] GetPlayerWeaponCurrentWeaponPoint(GameObject root)
	{
		var transform = root.transform;
		List<string> weapontParents=new List<string>();
		Transform singleWeapontPoint;
		var attachPoints = Enum.GetValues(typeof(AttachPoint));
		if (transform != null)
		{
			foreach (AttachPoint point in attachPoints)
			{
				if (transform.RecursiveFindObject(point.GetAttachment(), out singleWeapontPoint))
				{
					if (singleWeapontPoint.childCount > 0)
					{
						weapontParents.Add(ConstDefineManager.GetWeaponPosByName(singleWeapontPoint.name).ToString());
					}
				}
			}
		}
		return weapontParents.ToArray();
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
		path=path.Replace("Resources/", "|");
		path = path.Substring(path.IndexOf("|") + 1);
		
		return path;
	}
}
