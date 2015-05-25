using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class PlayerGenerateConfigData
{
    public byte PlayerId;
    public string PlayerName;
    public string DefaultAvatar;
    public string DefaultAnim;
    public string DefaultWeapon;
    public string[] WeaponPosition;
    public string[] Item_WeaponPosition;
    public int[] NormalSkillID;
    public int[] NormalBackAttackSkillID;
    public int ScrollSkillID;
	public int BUFF_SKILLID;
	public int FATAL_SKILLID;
    public int StandUpSkillID;
    public int OpeningSkillID_1;
    public int OpeningSkillID_2;



    [GameDataPostFlag(true)]
    public GameObject SkillEffect;
    public string[] Animations;
    [GameDataPostFlag(true)]
    public GameObject ShadowEffect;
    [GameDataPostFlag(true)]
    public GameObject RunEffect;


    public GameObject WeaponObj;
    public string ItemAniIdle;
    public string[] ItemAniChange;
    public Vector3 Avatar_CharPos;//时装界面角色出生坐标
    public string[] Avatar_WeaponPos;//时装武器挂载点
    public string[] Avatar_Ani;//时装更换动画
    public string[] Avatar_DefaultAni;//时装待机动画
    public Vector3 Avatar_CameraPos;//时装界面镜头位置相对角色的偏移量，A+X+Y+Z
    public Vector3 Avatar_CameraTargetPos;//时装界面镜头目标位置相对角色的偏移量，A+X+Y+Z

    public Vector3 RoleUI_CharPos;//角色界面角色出生坐标（填写世界坐标：A+X+Y+Z）
    public string[] RoleUI_WeaponPosition;//角色界面武器挂载位置
    public string[] RoleUI_Ani_Idle;//角色界面待机动作（动作名+动作持续时间毫秒|动作名+动作持续时间毫秒）
    public Vector3 RoleUI_CameraPos;//角色界面镜头位置相对角色的偏移量，A+X+Y+Z
    public Vector3 RoleUI_CameraTargetPos;//角色界面镜头目标位置相对角色的偏移量，A+X+Y+Z

    public string[]Item_InAni; //道具跳入动作，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public PackToFashingEff[]Item_InEff;//道具跳入特效，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public string[]Item_OutAni; //道具跳出动作，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public PackToFashingEff[]Item_OutEff;//道具跳出特效，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public string[]Avatar_InAni; //时装跳入动作，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public PackToFashingEff[]Avatar_InEff;//时装跳入特效，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public string[]Avatar_OutAni; //时装跳出动作，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒
    public PackToFashingEff[]Avatar_OutEff;//时装转跳出特效，格式：动作名+动作持续时间毫秒|动作名+动作持续时间毫秒


    public string[] PVP_Ready;
    public string[] PVP_Success;
    public string[] PVP_Fail;
    public float HitFlyHeight;  //角色被击飞高度

	public string[] Team_WeaponPosition;
	public string[] Team_Ani_Idle;
	public string[] Team_Ani_Join;

    public int BeAttackBreakLV;
    public int BeHitFlyBreakLV;

    /// <summary>
    /// 根据是否后退获得普通攻击ID
    /// </summary>
    /// <param name="step">普通攻击段数</param>
    /// <param name="backUp">是否后退</param>
    /// <returns></returns>
    public int GetPlayerNormalSkillId(int step,bool backUp)
    {
        return backUp ? NormalBackAttackSkillID[step] : NormalSkillID[step];
    }
}
[Serializable]
public class PackToFashingEff
{
    public GameObject Eff;
    public float StartTime;
}
public class PlayerGenerateConfigDataBase : ScriptableObject
{
    public PlayerGenerateConfigData[] _dataTable;
}
