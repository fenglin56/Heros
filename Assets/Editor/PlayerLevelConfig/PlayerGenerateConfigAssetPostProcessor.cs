using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using System.IO;
using UnityEngine;
using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class PlayerGenerateConfig : AssetPostprocessor
{
    private const string EW_RESOURCE_PLAYER_GENERATE_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Res";
    private const string EW_ASSET_PLAYER_GENERATE_CONFIG_FOLDER = "Assets/Data/PlayerConfig/Data";
    private const string EW_ASSET_PLAYER_SKILLEFFECT_FOLDER = "Assets/Effects/Prefab";
    private const string EW_ASSET_PLAYER_WEAPON_FOLDER = "Assets/Players/WeaponLib/Prefabs";

    private static void OnPostprocessAllAssets(string[] importedAssets,string[] deletedAssets
        , string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets)
            || CheckResModified(deletedAssets)
            ||CheckResModified(movedAssets))
        {
            string fileName = "PlayerGenerateConfig";

            MakePlayGenerateConfigDataList(fileName, "Town");
            MakePlayGenerateConfigDataList(fileName, "Battle");
            MakePlayGenerateConfigDataList(fileName, "UI");
            MakePlayGenerateConfigDataList(fileName, "PlayerRoom");
        }
    }
    private static void MakePlayGenerateConfigDataList(string xmlFileName,string sheetName)
    {
        string xmlPath = Path.Combine(EW_RESOURCE_PLAYER_GENERATE_CONFIG_FOLDER, xmlFileName + ".xml");

        TextReader tr = new StreamReader(xmlPath);
        string text = tr.ReadToEnd();
		tr.Close();
		tr.Dispose();
        if (string.IsNullOrEmpty(text))
        {
            Debug.LogError("Player generate config file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text, sheetName);
            

            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;

            string[] keys = XmlSpreadSheetReader.Keys;

            object[] levelIds = sheet[keys[0]];

            List<PlayerGenerateConfigData> tempList = new List<PlayerGenerateConfigData>();

            for (int i = 1; i < levelIds.Length; i++)
            {
                PlayerGenerateConfigData data = new PlayerGenerateConfigData();
                data.PlayerId =Convert.ToByte(sheet["PlayerId"][i]);
                data.PlayerName = Convert.ToString(sheet["PlayerName"][i]);
                data.DefaultAvatar = Convert.ToString(sheet["DefaultAvatar"][i]);
                data.DefaultAnim = Convert.ToString(sheet["DefaultAnim"][i]);
                data.DefaultWeapon = Convert.ToString(sheet["DefaultWeapon"][i]);
                if (!string.IsNullOrEmpty(data.DefaultWeapon))
                {
                    var WeaponPath = Path.Combine(EW_ASSET_PLAYER_WEAPON_FOLDER,data.DefaultWeapon+".prefab");
                    data.WeaponObj = AssetDatabase.LoadAssetAtPath(WeaponPath,typeof(GameObject)) as GameObject;
                }
                var weaponPositins = Convert.ToString(sheet["WeaponPosition"][i]).Split('+');
                if (weaponPositins.Length > 0)
                {
                    data.WeaponPosition = weaponPositins;
                }
                else
                {
                    data.WeaponPosition = new string[] { Convert.ToString(sheet["WeaponPosition"][i]) };
                }
                var normalSkills = Convert.ToString(sheet["NormalSkillID"][i]).Split('+');
                data.NormalSkillID = new int[6];
                data.NormalSkillID[0] = Convert.ToInt32(normalSkills[0]);//突击
                data.NormalSkillID[1] = Convert.ToInt32(normalSkills[1]);//突击后的攻击
                data.NormalSkillID[2] = Convert.ToInt32(normalSkills[2]);//以下是修改前的普攻4段
				data.NormalSkillID[3]= Convert.ToInt32(normalSkills[3]);
                data.NormalSkillID[4] = Convert.ToInt32(normalSkills[4]);
                data.NormalSkillID[5] = Convert.ToInt32(normalSkills[5]);
                data.ScrollSkillID= Convert.ToInt32(normalSkills[4]);
                if (normalSkills.Length >= 9)
                {
                    data.BUFF_SKILLID = Convert.ToInt32(normalSkills[7]);
                    data.FATAL_SKILLID = Convert.ToInt32(normalSkills[8]);
                }
                if (normalSkills.Length >= 10)
                {
                    data.StandUpSkillID = Convert.ToInt32(normalSkills[9]);
                }
                if(normalSkills.Length >= 12)
                {
                    data.OpeningSkillID_1 = Convert.ToInt32(normalSkills[10]);
                    data.OpeningSkillID_2 = Convert.ToInt32(normalSkills[11]);
                }                

                string skillEffectName = Convert.ToString(sheet["SkillEffect"][i]);
                if (skillEffectName == "null") skillEffectName = null;
                if (!string.IsNullOrEmpty(skillEffectName) )
                {
                    var effectPath = Path.Combine(EW_ASSET_PLAYER_SKILLEFFECT_FOLDER, skillEffectName+".prefab");
                    var skillEffectGo = AssetDatabase.LoadAssetAtPath(effectPath, typeof(GameObject)) as GameObject;
                    data.SkillEffect = skillEffectGo;
                }
                //阴影Prefabs
                data.ShadowEffect = AssetDatabase.LoadAssetAtPath(Path.Combine(EW_ASSET_PLAYER_SKILLEFFECT_FOLDER, "JH_Eff_Scenes_011.prefab"), typeof(GameObject)) as GameObject;
                if(sheetName == "Battle")
                {
                    var normalBackAttackSkills = Convert.ToString(sheet["BackAttack"][i]).Split('+');  //普通攻击，边退边攻击
                    data.NormalBackAttackSkillID = new int[6];
                    data.NormalBackAttackSkillID[0] = Convert.ToInt32(normalBackAttackSkills[0]);  //突击
                    data.NormalBackAttackSkillID[1] = Convert.ToInt32(normalBackAttackSkills[1]);  //突击后的攻击
                    data.NormalBackAttackSkillID[2] = Convert.ToInt32(normalBackAttackSkills[2]);  //以下是修改前的普攻4段
                    data.NormalBackAttackSkillID[3] = Convert.ToInt32(normalBackAttackSkills[3]);
                    data.NormalBackAttackSkillID[4] = Convert.ToInt32(normalBackAttackSkills[4]);
                    data.NormalBackAttackSkillID[5] = Convert.ToInt32(normalBackAttackSkills[5]);

                    string runEffectStr = Convert.ToString(sheet["RunEffect"][i]);
                    data.RunEffect = AssetDatabase.LoadAssetAtPath(Path.Combine("Assets", runEffectStr + ".prefab" ), typeof(GameObject)) as GameObject;
                }


                var animations = Convert.ToString(sheet["Animations"][i]).Split('+');
                if (animations.Length > 0)
                {
                    data.Animations = animations;
                }
                else
                {
                    data.Animations = new string[] { Convert.ToString(sheet["Animations"][i]) };
                }
                if (sheetName == "UI")
                {
                    data.ItemAniIdle = Convert.ToString(sheet["Item_Ani_Idle"][i]);
                    data.ItemAniChange = Convert.ToString(sheet["Item_Ani_Change"][i]).Split('|');
                    data.Item_WeaponPosition = Convert.ToString(sheet["Item_WeaponPosition"][i]).Split('+');

                    string[] charPosStr = Convert.ToString(sheet["Avatar_CharPos"][i]).Split('+');
                    data.Avatar_CharPos = new Vector3(float.Parse(charPosStr[1]), float.Parse(charPosStr[2]), float.Parse(charPosStr[3]));
                    data.Avatar_WeaponPos = Convert.ToString(sheet["Avatar_WeaponPos"][i]).Split('+');
                    data.Avatar_Ani = Convert.ToString(sheet["Avatar_Ani"][i]).Split('|');
//                    data.RoleUI_ItemToAvatar_Ani=Convert.ToString(sheet["ItemToAvatar_Ani"][i]).Split('|');
//                    data.RoleUI_AvatarToItem_Ani=Convert.ToString(sheet["AvatarToItem_Ani"][i]).Split('|');
                    data.Item_InAni=Convert.ToString(sheet["Item_InAni"][i]).Split('|');

                    string[] Item_InEf=Convert.ToString(sheet["Item_InEff"][i]).Split('|');
                    List<PackToFashingEff> Item_InEfs=new List<PackToFashingEff>();
                    foreach(var item in Item_InEf)
                    {
                        PackToFashingEff eff=new PackToFashingEff();
                        string ItemInEffPath=Path.Combine(EW_ASSET_PLAYER_SKILLEFFECT_FOLDER, item.Split('+')[0]+".prefab");
                        eff.Eff= AssetDatabase.LoadAssetAtPath(ItemInEffPath, typeof(GameObject))as GameObject;
                        eff.StartTime=Convert.ToSingle(item.Split('+')[1]);
                        Item_InEfs.Add(eff);
                    }
                    data.Item_InEff=Item_InEfs.ToArray();

                    data.Item_OutAni=Convert.ToString(sheet["Item_OutAni"][i]).Split('|');

                    string[] Item_OutEff=Convert.ToString(sheet["Item_OutEff"][i]).Split('|');
                    List<PackToFashingEff> Item_OutEffs=new List<PackToFashingEff>();
                    foreach(var item in Item_OutEff)
                    {
                        PackToFashingEff eff=new PackToFashingEff();
                        string ItemInEffPath=Path.Combine(EW_ASSET_PLAYER_SKILLEFFECT_FOLDER, item.Split('+')[0]+".prefab");
                        eff.Eff= AssetDatabase.LoadAssetAtPath(ItemInEffPath, typeof(GameObject))as GameObject;
                        eff.StartTime=Convert.ToSingle(item.Split('+')[1]);
                        Item_OutEffs.Add(eff);
                    }
                    data.Item_OutEff=Item_OutEffs.ToArray();

                    data.Avatar_InAni=Convert.ToString(sheet["Avatar_InAni"][i]).Split('|');
                    string[] Avatar_InEff=Convert.ToString(sheet["Avatar_InEff"][i]).Split('|');
                    List<PackToFashingEff> Avatar_InEffs=new List<PackToFashingEff>();
                    foreach(var item in Avatar_InEff)
                    {
                        PackToFashingEff eff=new PackToFashingEff();
                        string ItemInEffPath=Path.Combine(EW_ASSET_PLAYER_SKILLEFFECT_FOLDER, item.Split('+')[0]+".prefab");
                        eff.Eff= AssetDatabase.LoadAssetAtPath(ItemInEffPath, typeof(GameObject))as GameObject;
                        eff.StartTime=Convert.ToSingle(item.Split('+')[1]);
                        Avatar_InEffs.Add(eff);
                    }
                    data.Avatar_InEff=Avatar_InEffs.ToArray();
                    data.Avatar_OutAni=Convert.ToString(sheet["Avatar_OutAni"][i]).Split('|');
                    string[] Avatar_OutEff=Convert.ToString(sheet["Avatar_OutEff"][i]).Split('|');
                    List<PackToFashingEff> Avatar_OutEffs=new List<PackToFashingEff>();
                    foreach(var item in Avatar_OutEff)
                    {
                        PackToFashingEff eff=new PackToFashingEff();
                        string ItemInEffPath=Path.Combine(EW_ASSET_PLAYER_SKILLEFFECT_FOLDER, item.Split('+')[0]+".prefab");
                        eff.Eff= AssetDatabase.LoadAssetAtPath(ItemInEffPath, typeof(GameObject))as GameObject;
                        eff.StartTime=Convert.ToSingle(item.Split('+')[1]);
                        Avatar_OutEffs.Add(eff);
                    }
                    data.Avatar_OutEff=Avatar_OutEffs.ToArray();

                    data.Avatar_DefaultAni = Convert.ToString(sheet["Avatar_DefaultAni"][i]).Split('|');
                    string[] camPosStr = Convert.ToString(sheet["Avatar_CameraPos"][i]).Split('+');
                    data.Avatar_CameraPos = new Vector3(float.Parse(camPosStr[1]), float.Parse(camPosStr[2]), float.Parse(camPosStr[3]));
                    string[] camTargerPosStr = Convert.ToString(sheet["Avatar_CameraTargetPos"][i]).Split('+');
                    data.Avatar_CameraTargetPos = new Vector3(float.Parse(camTargerPosStr[1]), float.Parse(camTargerPosStr[2]), float.Parse(camTargerPosStr[3]));

                    string[] roleCharPosStr = Convert.ToString(sheet["RoleUI_CharPos"][i]).Split('+');
                    data.RoleUI_CharPos = new Vector3(float.Parse(roleCharPosStr[1]), float.Parse(roleCharPosStr[2]), float.Parse(roleCharPosStr[3]));
                    data.RoleUI_WeaponPosition = Convert.ToString(sheet["RoleUI_WeaponPosition"][i]).Split('+');
                    data.RoleUI_Ani_Idle = Convert.ToString(sheet["RoleUI_Ani_Idle"][i]).Split('|');
                    string[] roleCamPosStr = Convert.ToString(sheet["RoleUI_CameraPos"][i]).Split('+');
                    data.RoleUI_CameraPos = new Vector3(float.Parse(roleCamPosStr[1]), float.Parse(roleCamPosStr[2]), float.Parse(roleCamPosStr[3]));
                    string[] camTargetPosStr = Convert.ToString(sheet["RoleUI_CameraTargetPos"][i]).Split('+');
                    data.RoleUI_CameraTargetPos = new Vector3(float.Parse(camTargetPosStr[1]), float.Parse(camTargetPosStr[2]), float.Parse(camTargetPosStr[3]));

                    data.PVP_Ready = Convert.ToString(sheet["PVP_Ready"][i]).Split('|');
                    data.PVP_Success = Convert.ToString(sheet["PVP_Success"][i]).Split('|');
                    data.PVP_Fail = Convert.ToString(sheet["PVP_Fail"][i]).Split('|');

                    data.HitFlyHeight = Convert.ToSingle(sheet["HitFlyHeight"][i]);
                    data.BeAttackBreakLV = Convert.ToInt32(sheet["BeAttackBreakLV"][i]);
                    data.BeHitFlyBreakLV = Convert.ToInt32(sheet["BeHitFlyBreakLV"][i]);

					data.Team_WeaponPosition = Convert.ToString(sheet["Team_WeaponPosition"][i]).Split('+');
					data.Team_Ani_Idle = Convert.ToString(sheet["Team_Ani_Idle"][i]).Split('+');
					data.Team_Ani_Join = Convert.ToString(sheet["Team_Ani_Join"][i]).Split('+');
                }
                tempList.Add(data);
            }
            CreatePlayerGenerateConfigDataBase(tempList, xmlFileName + sheetName);
        }
    }
    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(EW_RESOURCE_PLAYER_GENERATE_CONFIG_FOLDER))
            {
                fileModified = true;
                break;
            }
        }

        return fileModified;
    }
    private static void CreatePlayerGenerateConfigDataBase(List<PlayerGenerateConfigData> list, string className)
    {
        string path = Path.Combine(EW_ASSET_PLAYER_GENERATE_CONFIG_FOLDER, className+".asset");

        if (File.Exists(path))
        {
            PlayerGenerateConfigDataBase database = (PlayerGenerateConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerGenerateConfigDataBase));

            if (database == null)
            {
                return;
            }
            database._dataTable = new PlayerGenerateConfigData[list.Count];
            list.CopyTo(database._dataTable) ;
            EditorUtility.SetDirty(database);
        }
        else
        {
            PlayerGenerateConfigDataBase database = ScriptableObject.CreateInstance<PlayerGenerateConfigDataBase>();
            database._dataTable = new PlayerGenerateConfigData[list.Count];
            list.CopyTo(database._dataTable);
            AssetDatabase.CreateAsset(database, path);
        }
    }
}
