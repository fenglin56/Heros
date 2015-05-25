using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PvpRoleControl : MonoBehaviour {
	private GameObject m_hero;
	private int m_weaponID;
	private int m_FashionID;
	private string[] m_defaultWeaponAtt;
	private GameObject EffPoint;
	void  Start()
	{
		EffPoint=new GameObject();
		EffPoint.name="EffPoint";
		EffPoint.transform.parent=m_hero.transform;
		EffPoint.transform.localPosition=Vector3.zero;
		EffPoint.transform.localEulerAngles=Vector3.zero;
	}
	public void ChangeWearponAndFashion(GameObject hero,int weaponID,int FashionID,string[] defaultWeaponAtt)
	{
		m_hero=hero;
		m_weaponID=weaponID;
		m_FashionID=FashionID;
		m_defaultWeaponAtt=defaultWeaponAtt;
		ChangeHeroWeapon();
		ChangeHeroFashion();
		SetCildLayer(m_hero.transform,10);
	}


	public void AttachEffect(GameObject effectPrefab)
	{
		GameObject newObj = transform.InstantiateNGUIObj(effectPrefab);
		newObj.transform.localPosition = m_hero.transform.localPosition;

	}


	public void ChangeHeroWeapon()
	{
		if (!gameObject.activeSelf)
			return;
		//TraceUtil.Log("当前默认武器ID："+WeaponInfo.uidGoods);

		if (m_weaponID != 0)
			{
			EquipmentData weapdata=ItemDataManager.Instance.GetItemData(m_weaponID) as EquipmentData;
				string weapon = weapdata._ModelId;
				var weaponEff=weapdata.WeaponEff;
				ChangeWeapon(weapon,weaponEff);
			}

	}


	void  ChangeWeapon(string Weapon ,GameObject weaponEff)
	{
		//yield return null;
		var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(Weapon);
		SetCildLayer(weaponObj.transform,10);
		RoleGenerate.AttachWeapon(this.m_hero,m_defaultWeaponAtt, weaponObj,weaponEff);
		TraceUtil.Log("改变武器" + weaponObj);
	}

	void SetCildLayer(Transform m_transform, int layer)
	{
		m_transform.gameObject.layer = layer;
		if (m_transform.childCount > 0)
		{
			foreach (Transform child in m_transform)
			{
				SetCildLayer(child, layer);
			}
		}
	}

	public void ChangeHeroFashion()
	{
		if (!gameObject.activeSelf)
			return;
			ChangeFashion(m_FashionID); 	
	}

	/// <summary>
	/// 改变主角时装
	/// </summary>
	/// <param name="weaponName"></param>
	void ChangeFashion(int fashionID)
	{
		var FashionData = ItemDataManager.Instance.GetItemData(fashionID);
		TraceUtil.Log("切换时装：" + FashionData._ModelId);
		//yield return null;
		RoleGenerate.GenerateRole(m_hero, FashionData._ModelId);
	}

	public void PlayAnimation(string[] OnceWeaponPos,string[] OnceAnimationNames ,PackToFashingEff OnceEffs,string[] LoopWeaponPos,string[] LoopAnimationNames,PackToFashingEff Loopeffs)
	{
		List<AnimationClip> OnecAnimationClips=new List<AnimationClip>();
		if(OnceAnimationNames!=null)
		{
		 OnceAnimationNames.ApplyAllItem(c=>OnecAnimationClips.Add( m_hero.animation.GetClip(c)));
		}

		List<AnimationClip> LoopAnimationClips=new List<AnimationClip>();

		LoopAnimationNames.ApplyAllItem(c=>LoopAnimationClips.Add( m_hero.animation.GetClip(c)));

		StopCoroutine("PlayAnimation");
		StopCoroutine("PlayOnceAnimation");
		StopCoroutine("PlayeOnceAni");
		if(OnecAnimationClips.Count>0)
		{
			StartCoroutine(PlayAnimation(OnceWeaponPos,OnecAnimationClips,OnceEffs,LoopWeaponPos,LoopAnimationClips[0],Loopeffs));
		}
		else
		{
			RoleGenerate.AttachWeapon(this.m_hero,LoopWeaponPos);
			PlayLoopAnimation(LoopAnimationClips[0],Loopeffs);
		}
	}

	IEnumerator  PlayAnimation(string[] OnceWeaponPos,List<AnimationClip> OnecAnimationClips,PackToFashingEff onceEff,string[] LoopWeaponPos,AnimationClip clip,PackToFashingEff LoopEff)
	{
		RoleGenerate.AttachWeapon(this.m_hero,OnceWeaponPos);
		yield return null;
		yield return StartCoroutine( PlayOnceAnimation(OnecAnimationClips,onceEff));
		RoleGenerate.AttachWeapon(this.m_hero,LoopWeaponPos);
		yield return null;
		PlayLoopAnimation(clip,LoopEff);
	}

	IEnumerator PlayOnceAnimation(List<AnimationClip> clips,PackToFashingEff eff)
	{
		StartCoroutine(ShowEffect(eff));
		m_hero.animation.wrapMode=WrapMode.Once;
		int i=0;
		while(i<clips.Count)
		{

			yield return StartCoroutine( PlayeOnceAni(clips[i]));
			i++;
		}
	}

	IEnumerator PlayeOnceAni(AnimationClip clip)
	{
		m_hero.animation.CrossFade(clip.name);
		yield return new WaitForSeconds(clip.length);
	}

	void PlayLoopAnimation(AnimationClip clip,PackToFashingEff eff)
	{

		m_hero.animation.wrapMode=WrapMode.Loop;
		m_hero.animation.CrossFade(clip.name);
		StartCoroutine(ShowEffect(eff));
	}
	IEnumerator ShowEffect(PackToFashingEff eff)
	{

		if(eff!=null&&eff.Eff!=null)
		{
			SetCildLayer(eff.Eff.transform,10);
			yield return new WaitForSeconds(eff.StartTime);
			UI.CreatObjectToNGUI.InstantiateObj(eff.Eff,EffPoint.transform);
		}
		else
		{
			yield return 0;
			EffPoint.transform.ClearChild();
		}
	}
}
