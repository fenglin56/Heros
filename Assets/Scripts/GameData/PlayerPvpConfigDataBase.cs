using UnityEngine;
using System.Collections;
[System.Serializable]
public class PlayerPvpConfigData {

	public int PlayerId;
	public string PlayerName;
	public string DefaultAvatar;
	public string DefaultAnim;
	public string[] Animations;
	public string[] In_Ani;
	public string[] In_WeaponPos;
	public PackToFashingEff In_Eff;
	public string[] NotAdapIdle_Ani;
	public string[] NotAdapIdle_WeaponPos;
	public PackToFashingEff NotAdapIdle_Eff;
	public string[] AdapStar_Ani	;
	public string[] AdapStar_WeaponPos;
	public PackToFashingEff AdapStar_Eff;
	public string[] AdapIdle_Ani;
	public string[] AdapIdle_WeaponPos;
	public PackToFashingEff AdapIdle_Eff;
	public string[] EnterGroup_Ani;
	public string[] EnterGroup_WeaponPos;
	public PackToFashingEff EnterGroup_Eff;

}
public class PlayerPvpConfigDataBase :ScriptableObject
{
	public PlayerPvpConfigData[] PlayerPvpConfigDataList;
}
