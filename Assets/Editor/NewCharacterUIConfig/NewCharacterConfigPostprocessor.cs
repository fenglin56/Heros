    using System;
using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System.IO;

using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class NewCharacterConfigPostprocessor : AssetPostprocessor
{

    public static readonly string RESOURCE_CHARACTER_DATA_FOLDER = "Assets/Data/PlayerConfig/Res";
    public static readonly string ASSET_CHARACTER_DATA_FOLDER = "Assets/Data/PlayerConfig/Data";
    public static readonly string ASSET_CHARACTER_DATA_PREFAB_FOLDER = "Assets/Players/WeaponLib/Prefabs";


    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                               string[] movedAssets, string[] movedFromPath)
    {
        if (CheckResModified(importedAssets) || CheckResModified(deletedAssets) || CheckResModified(movedAssets))
        {
            string fileName = "PlayerGenerateConfig";

            MakePlayGenerateConfigDataList(fileName, "NewCharacterUIConfig");
            PostprocessCreateRoleUIDataList(fileName, "CreateRoleUI");
			MakePlayPvpConfigDataList(fileName,"PVPGroup");
        }

    }

    private static void PostprocessCreateRoleUIDataList(string xmlFileName, string sheetName)
    {
        string xmlPath = Path.Combine(RESOURCE_CHARACTER_DATA_FOLDER, xmlFileName + ".xml");

        TextReader tr = new StreamReader(xmlPath);
        string text = tr.ReadToEnd();

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

            List<CreateRoleUIData> tempList = new List<CreateRoleUIData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                CreateRoleUIData data = new CreateRoleUIData();

                data._VocationID = Convert.ToInt32(sheet["VocationID"][i]);
                data._HeadIcon = Convert.ToString(sheet["Picture"][i]);
                data._VocationIcon = Convert.ToString(sheet["VocationIcon"][i]);

                string roleModelName = Convert.ToString(sheet["RoleModel"][i]);
                string path = Path.Combine("Assets/Prefab/NPC/Prefab", roleModelName + ".prefab");
                data._RoleModel = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                
                string[] charPosStr = Convert.ToString(sheet["CharPos"][i]).Split('+');
                data._RolePosition = new Vector3(Convert.ToSingle(charPosStr[1]), 0, Convert.ToSingle(charPosStr[3]));


                //string[] selectAnimStr = Convert.ToString(sheet["SelectAnim"][i]).Split('|');
                //data._SelectAnim = new string[selectAnimStr.Length];
                //for (int j = 0; j < selectAnimStr.Length; j++)
                //{
                //    data._SelectAnim[j] = Convert.ToString(selectAnimStr[j]);
                //}

                //string[] selectAnimSoundStr = Convert.ToString(sheet["SelectAnimSound"][i]).Split('|');
                //data._SelectAnimSound = new SelectSound[selectAnimSoundStr.Length];
                //for (int j = 0; j < selectAnimSoundStr.Length; j++)
                //{
                //    string[] selectSoundStr = Convert.ToString(selectAnimSoundStr[j]).Split('+');
                //    data._SelectAnimSound[j] = new SelectSound();
                //    data._SelectAnimSound[j].SoundName = Convert.ToString(selectSoundStr[0]);
                //    data._SelectAnimSound[j].DalayTime = Convert.ToSingle(selectSoundStr[1]);
                //}

                data._InitAnim = Convert.ToString(sheet["Animations"][i]);
                string[] charAnimStr = Convert.ToString(sheet["Animations_Go"][i]).Split('|');
                data._AnimList = new string[charAnimStr.Length];

                for (int j = 0; j < charAnimStr.Length; j++)
                {
                    string[] animStr = Convert.ToString(charAnimStr[j]).Split('+');
                    data._AnimList[j] = Convert.ToString(animStr[0]);   
                }
                data._StopAnim = Convert.ToString(sheet["Animations_Idle"][i]);
                data._BackAnim = Convert.ToString(sheet["Animations_Back"][i]);

                data._IntroText = Convert.ToString(sheet["Introductions"][i]);
                string[] effectNameStr = Convert.ToString(sheet["EffectName"][i]).Split('+');
                data._EffectList = new GameObject[effectNameStr.Length];
                for (int j = 0; j < effectNameStr.Length; j++)
                {
                    string effectPath = Path.Combine("Assets\\Effects\\Prefab", effectNameStr[j] + ".prefab"); 
                    data._EffectList[j] = AssetDatabase.LoadAssetAtPath(effectPath, typeof(GameObject)) as GameObject;
                }

                string[] effectDelayTimeStr = Convert.ToString(sheet["EffectTime"][i]).Split('+');
                data._EffectDelayTime = new float[effectDelayTimeStr.Length];
                for (int j = 0; j < effectDelayTimeStr.Length; j++)
                {
                    data._EffectDelayTime[j] = Convert.ToSingle(effectDelayTimeStr[j]);
                }

                data._SoundEffectList = Convert.ToString(sheet["ChooseVoice"][i]).Split('+');

                string[] cameraPos = Convert.ToString(sheet["CameraPos"][i]).Split('+');
                data._CameraPosition = new Vector3(Convert.ToSingle(cameraPos[1]), Convert.ToSingle(cameraPos[2]), Convert.ToSingle(cameraPos[3]));

                string[] cameraTarget = Convert.ToString(sheet["CameraTargetPos"][i]).Split('+');
                data._CameraTarget = new Vector3(Convert.ToSingle(cameraTarget[1]), Convert.ToSingle(cameraTarget[2]), Convert.ToSingle(cameraTarget[3]));

				data._UIDelayTime = Convert.ToSingle(sheet["UIAppearTime"][i]) * 0.001f;

                string[] abilityStr = Convert.ToString(sheet["PlayerAbility"][i]).Split('+');
                data.PlayerAbility = new List<int>();
                abilityStr.ApplyAllItem(P => data.PlayerAbility.Add(int.Parse(P)));

                tempList.Add(data);
            }

            CreateRoleUIDataList(tempList);
        }
    }

    private static void MakePlayGenerateConfigDataList(string xmlFileName, string sheetName)
    {

        string path = Path.Combine(RESOURCE_CHARACTER_DATA_FOLDER, xmlFileName + ".xml");
        TextReader tr = new StreamReader(path);
        string text = tr.ReadToEnd();

        if (text == null)
        {
            Debug.LogError("Equipment item file not exist");
            return;
        }
        else
        {
            XmlSpreadSheetReader.ReadSheet(text,sheetName);
            XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
            string[] keys = XmlSpreadSheetReader.Keys;
            object[] levelIds = sheet[keys[0]];

            List<NewCharacterConfigData> tempList = new List<NewCharacterConfigData>();

            for (int i = 2; i < levelIds.Length; i++)
            {
                NewCharacterConfigData data = new NewCharacterConfigData();
                //data.m_SzName = Convert.ToString(sheet["szName"][i]);
                //data.m_IEquipmentID = Convert.ToInt32(sheet["lEquimentID"][i]);
                data.VocationID = Convert.ToInt32(sheet["VocationID"][i]);
                data.Picture = Convert.ToString(sheet["Picture"][i]);
                data.Model = Convert.ToString(sheet["Model"][i]);
                string Weapon = Convert.ToString(sheet["Weapon"][i]);
                if (!string.IsNullOrEmpty(Weapon) && Weapon != "0")
                {
                    var displayPath = Path.Combine(ASSET_CHARACTER_DATA_PREFAB_FOLDER, Weapon + ".prefab");
                    var displayGo = AssetDatabase.LoadAssetAtPath(displayPath, typeof(GameObject)) as GameObject;
                    data.Weapon = displayGo;
                }
                data.WeaponPosition = Convert.ToString(sheet["WeaponPosition"][i]);
                string AnimationTime = Convert.ToString(sheet["Animations"][i]);
                data.AnimationsStr = Convert.ToString(sheet["Animations"][i]);
                data.SelectAnimationsStr = Convert.ToString(sheet["SelectAni"][i]);
                data.Introductions = Convert.ToString(sheet["Introductions"][i]);
                data.EffectName = Convert.ToString(sheet["EffectName"][i]);
                data.EffectPrefab = (GameObject)Resources.LoadAssetAtPath("Assets/Effects/Prefab/" + data.EffectName + ".prefab", typeof(GameObject));
                data.EffectTime = Convert.ToSingle(sheet["EffectTime"][i]);
                data.SelectAniSound = Convert.ToString(sheet["SelectAniSound"][i]);
                data.ChooseVoice = Convert.ToString(sheet["ChooseVoice"][i]);
                data.PostprocessorAnimation();
                tempList.Add(data);
            }

            CreateMedicamentConfigDataList(tempList);
        }

    }
	private static void MakePlayPvpConfigDataList(string xmlFileName, string sheetName)
	{
		
		string path = Path.Combine(RESOURCE_CHARACTER_DATA_FOLDER, xmlFileName + ".xml");
		TextReader tr = new StreamReader(path);
		string text = tr.ReadToEnd();
		
		if (text == null)
		{
			Debug.LogError("Equipment item file not exist");
			return;
		}
		else
		{
			XmlSpreadSheetReader.ReadSheet(text,sheetName);
			XmlSpreadSheet sheet = XmlSpreadSheetReader.Output;
			string[] keys = XmlSpreadSheetReader.Keys;
			object[] levelIds = sheet[keys[0]];
			
			List<PlayerPvpConfigData> tempList = new List<PlayerPvpConfigData>();
			
			for (int i = 2; i < levelIds.Length; i++)
			{
				PlayerPvpConfigData data = new PlayerPvpConfigData();
				//data.m_SzName = Convert.ToString(sheet["szName"][i]);
				//data.m_IEquipmentID = Convert.ToInt32(sheet["lEquimentID"][i]);
				data.PlayerId = Convert.ToInt32(sheet["PlayerId"][i]);
				data.PlayerName = Convert.ToString(sheet["PlayerName"][i]);
				data.DefaultAvatar = Convert.ToString(sheet["DefaultAvatar"][i]);
				data.DefaultAnim = Convert.ToString(sheet["DefaultAnim"][i]);
				data.Animations = Convert.ToString(sheet["Animations"][i]).Split('+');
				data.In_Ani = Convert.ToString(sheet["In_Ani"][i]).Split('+');
				data.In_WeaponPos = Convert.ToString(sheet["In_WeaponPos"][i]).Split('+');
				string inEff = Convert.ToString(sheet["In_Eff"][i]);
				if(inEff!="0")
				{
					string[] str=inEff.Split('+');
					PackToFashingEff eff=new PackToFashingEff();
					eff.Eff=(GameObject)Resources.LoadAssetAtPath("Assets/Effects/Prefab/" + str[0] + ".prefab", typeof(GameObject));
					eff.StartTime=Convert.ToInt32(str[1]);
					data.In_Eff=eff;
				}
				data.NotAdapIdle_Ani = Convert.ToString(sheet["NotAdapIdle_Ani"][i]).Split('+');
				data.NotAdapIdle_WeaponPos = Convert.ToString(sheet["NotAdapIdle_WeaponPos"][i]).Split('+');
				string NotAdapIdle_Eff = Convert.ToString(sheet["NotAdapIdle_Eff"][i]);
				if(NotAdapIdle_Eff!="0")
				{
					string[] str=NotAdapIdle_Eff.Split('+');
					PackToFashingEff eff=new PackToFashingEff();
					eff.Eff=(GameObject)Resources.LoadAssetAtPath("Assets/Effects/Prefab/" + str[0] + ".prefab", typeof(GameObject));
					eff.StartTime=Convert.ToInt32(str[1]);
					data.NotAdapIdle_Eff=eff;
				}
				data.AdapStar_Ani = Convert.ToString(sheet["AdapStar_Ani"][i]).Split('+');
				data.AdapStar_WeaponPos = Convert.ToString(sheet["AdapStar_WeaponPos"][i]).Split('+');
				string AdapStar_Eff = Convert.ToString(sheet["AdapStar_Eff"][i]);
				if(AdapStar_Eff!="0")
				{
					string[] str=AdapStar_Eff.Split('+');
					PackToFashingEff eff=new PackToFashingEff();
					eff.Eff=(GameObject)Resources.LoadAssetAtPath("Assets/Effects/Prefab/" + str[0] + ".prefab", typeof(GameObject));
					eff.StartTime=Convert.ToInt32(str[1]);
					data.AdapStar_Eff=eff;
				}
				data.AdapIdle_Ani = Convert.ToString(sheet["AdapIdle_Ani"][i]).Split('+');
				data.AdapIdle_WeaponPos = Convert.ToString(sheet["AdapIdle_WeaponPos"][i]).Split('+');
				string AdapIdle_Eff = Convert.ToString(sheet["AdapIdle_Eff"][i]);
				if(AdapIdle_Eff!="0")
				{
					string[] str=AdapIdle_Eff.Split('+');
					PackToFashingEff eff=new PackToFashingEff();
					eff.Eff=(GameObject)Resources.LoadAssetAtPath("Assets/Effects/Prefab/" + str[0] + ".prefab", typeof(GameObject));
					eff.StartTime=Convert.ToInt32(str[1]);
					data.AdapIdle_Eff=eff;
				}
				data.EnterGroup_Ani = Convert.ToString(sheet["EnterGroup_Ani"][i]).Split('+');
				data.EnterGroup_WeaponPos = Convert.ToString(sheet["EnterGroup_WeaponPos"][i]).Split('+');
				string EnterGroup_Eff = Convert.ToString(sheet["EnterGroup_Eff"][i]);
				if(EnterGroup_Eff!="0")
				{
					string[] str=EnterGroup_Eff.Split('+');
					PackToFashingEff eff=new PackToFashingEff();
					eff.Eff=(GameObject)Resources.LoadAssetAtPath("Assets/Effects/Prefab/" + str[0] + ".prefab", typeof(GameObject));
					eff.StartTime=Convert.ToInt32(str[1]);
					data.EnterGroup_Eff=eff;
				}
				tempList.Add(data);
			}
			
			CreatePVPConfigDataList(tempList);
		}
		
	}

    private static bool CheckResModified(string[] files)
    {
        bool fileModified = false;
        foreach (string file in files)
        {
            if (file.Contains(RESOURCE_CHARACTER_DATA_FOLDER))
            {
                fileModified = true;
                break;
            }
        }
        return fileModified;
    }

    static void CreateRoleUIDataList(List<CreateRoleUIData> list)
    {
        string fileName = typeof(CreateRoleUIData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_CHARACTER_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            CreateRoleUIDataBase database = (CreateRoleUIDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(CreateRoleUIDataBase));

            if (null == database)
            {
                return;
            }

            database._dataTable = new CreateRoleUIData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            CreateRoleUIDataBase database = ScriptableObject.CreateInstance<CreateRoleUIDataBase>();
            database._dataTable = new CreateRoleUIData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database._dataTable[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

    static void CreateMedicamentConfigDataList(List<NewCharacterConfigData> list)
    {
        string fileName = typeof(NewCharacterConfigData).Name + "DataBase";
        string path = System.IO.Path.Combine(ASSET_CHARACTER_DATA_FOLDER, fileName + ".asset");

        if (File.Exists(path))
        {
            NewCharacterConfigDataBase database = (NewCharacterConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(NewCharacterConfigDataBase));

            if (null == database)
            {
                return;
            }

            database.NewCharacterConfigDataList = new NewCharacterConfigData[list.Count];

            for (int i = 0; i < list.Count; i++)
            {
                database.NewCharacterConfigDataList[i] = list[i];
            }
            EditorUtility.SetDirty(database);
        }
        else
        {
            NewCharacterConfigDataBase database = ScriptableObject.CreateInstance<NewCharacterConfigDataBase>();
            database.NewCharacterConfigDataList = new NewCharacterConfigData[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                database.NewCharacterConfigDataList[i] = list[i];
            }
            AssetDatabase.CreateAsset(database, path);
        }
    }

	static void CreatePVPConfigDataList(List<PlayerPvpConfigData> list)
	{
		string fileName = typeof(PlayerPvpConfigData).Name + "DataBase";
		string path = System.IO.Path.Combine(ASSET_CHARACTER_DATA_FOLDER, fileName + ".asset");
		
		if (File.Exists(path))
		{
			PlayerPvpConfigDataBase database = (PlayerPvpConfigDataBase)AssetDatabase.LoadAssetAtPath(path, typeof(PlayerPvpConfigDataBase));
			
			if (null == database)
			{
				return;
			}
			
			database.PlayerPvpConfigDataList = new PlayerPvpConfigData[list.Count];
			
			for (int i = 0; i < list.Count; i++)
			{
				database.PlayerPvpConfigDataList[i] = list[i];
			}
			EditorUtility.SetDirty(database);
		}
		else
		{
			PlayerPvpConfigDataBase database = ScriptableObject.CreateInstance<PlayerPvpConfigDataBase>();
			database.PlayerPvpConfigDataList = new PlayerPvpConfigData[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				database.PlayerPvpConfigDataList[i] = list[i];
			}
			AssetDatabase.CreateAsset(database, path);
		}
	}

}
