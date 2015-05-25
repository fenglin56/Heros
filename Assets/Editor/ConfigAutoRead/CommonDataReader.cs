using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using XmlSpreadSheet = System.Collections.Generic.Dictionary<string, object[]>;

public class CommonDataReader : AssetPostprocessor
{
#if UNITY_EDITOR
    // 编辑状态下配置表的路径
    private static readonly string RESOURCE_ITEM_CONFIG_FOLDER = "Assets/Data/";
#else
    // 运行时状态下配置表的路径
    private static readonly string RESOURCE_ITEM_CONFIG_FOLDER = "Assets/Data/TaskConfig/Res/";
#endif
    // Asset文件的存储路径
    private static readonly string ASSET_ITEM_CONFIG_FOLDER = "Assets/Data";

    private static string m_category;

    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
                                              string[] movedAssets, string[] movedFromPath)
    {
        CheckResModified(importedAssets) ;
        CheckResModified(deletedAssets) ;
        CheckResModified(movedAssets);
    }

    private static void CheckResModified(string[] files)
    {
        foreach (string file in files)
        {
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "NewbieGuideConfig.xml")))
            {
                CommonDataReader.ConfigPostprocess<NewGuideConfigDataBase, NewGuideConfigData>("TaskConfig", "NewbieGuideConfig.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "TaskConfig.xml")))
            {
                CommonDataReader.ConfigPostprocess<TaskNewConfigDataBase, TaskNewConfigData>("TaskConfig", "TaskConfig.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "MainButtonConfig.xml")))
            {
                CommonDataReader.ConfigPostprocess<MainTownButtonConfigDataBase, MainTownButtonConfigData>("TaskConfig", "MainButtonConfig.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "InitMainButton.xml")))
            {
                CommonDataReader.ConfigPostprocess<InitMainTownButtonDataBase, InitMainTownButtonData>("TaskConfig", "InitMainButton.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "TalkIDlist.xml")))
            {
                CommonDataReader.ConfigPostprocess<TalkIdConfigDataBase, TalkIdConfigData>("TaskConfig", "TalkIDlist.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("IllegalCharacterConfig", "IllegalCharacter.xml")))
            {
                CommonDataReader.ConfigPostprocess<IllegalCharacterDataBase, IllegalCharacterData>("IllegalCharacterConfig", "IllegalCharacter.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "GuideStepConfig.xml")))
            {
                CommonDataReader.ConfigPostprocess<EctGuideStepConfigDataBase, EctGuideStepConfigData>("TaskConfig", "GuideStepConfig.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("TaskConfig", "EctypeGuideTalkList.xml")))
            {
                CommonDataReader.ConfigPostprocess<EctGuideTalkDataBase, StepDialogData>("TaskConfig", "EctypeGuideTalkList.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("SceneConfig", "MapDynamicBlock.xml")))
            {
                CommonDataReader.ConfigPostprocess<MapDynamicBlockDataBase, MapDynamicBlockData>("SceneConfig", "MapDynamicBlock.xml", false);
                continue;
            }
            if (file.ToLower().Equals(GetMatchString("CharacterNameConfig", "IllegalNameConfig.xml")))
            {
                CommonDataReader.ConfigPostprocess<IllegalNameDataBase, IllegalNameData>("CharacterNameConfig", "IllegalNameConfig.xml", false);
                continue;
            }
        }
    }
    private static string GetMatchString(string category,string fileName)
    {
        string matchString=System.IO.Path.Combine(RESOURCE_ITEM_CONFIG_FOLDER, category + "/Res/" + fileName).ToLower();
        return matchString;
    }

    public static List<T2> ConfigPostprocess<T, T2>(string category,string configFileName,bool isOnlyReady)
        where T : ConfigBase
        where T2 : class
    {
        m_category = category;
        string path = System.IO.Path.Combine(RESOURCE_ITEM_CONFIG_FOLDER, m_category + "/Res/" + configFileName.ToString());
        //path = System.IO.Path.Combine(path,"Res/"+configFileName.ToString()); 
        Debug.Log("----WQ path" + path);
        FileStream stream = new FileStream(path, FileMode.OpenOrCreate);
        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
        string text = reader.ReadToEnd();
        reader.Close();
       
        if (text == null)
        {
            Debug.LogError("config file not exist!");
            return null;
        }
        else
        {
            XmlSpreadSheetCommonReader.ReadSheet(text);
            XmlSpreadSheet sheet = XmlSpreadSheetCommonReader.Output;
            //XmlSpreadSheet sheet = CSVReader.GetCsvTextData(text);
            string[] keys = XmlSpreadSheetCommonReader.Keys;

            object[] levelIds = sheet[keys[0]];
            int num = levelIds.Length;
            //int num = CSVReader.GetKeyNum(text);

            List<T2> tempList = new List<T2>();           

            for (int i = 0; i < num; i++)
            {
                if (i == 0 || 1 == i) continue;

                Type type = typeof(T2);

                T2 instance = Activator.CreateInstance(type) as T2;//-------返回一个object类型而不是Exercise.person类型
                foreach (FieldInfo field in type.GetFields())
                {
                    string sheetName = field.Name;
                    //Debug.Log("sheetName:" + sheetName);
                    //if (sheet.ContainsKey(sheetName))
                    //{
                    //    Debug.Log(sheetName + " = " + sheet[sheetName][i]);
                    //}
                   if (!Attribute.IsDefined(field, typeof(HideInDataReaderAttribute)))
                    {
                        if (Attribute.IsDefined(field, typeof(CustomerParseAttribute)))
                        {
                            int prefabPathIndex = 0;
                            string sheetVal = sheet[sheetName][i].ToString();
                            if (sheetVal != "0")
                            {
                                var cusAtts = Attribute.GetCustomAttribute(field, typeof(CustomerParseAttribute)) as CustomerParseAttribute;
                                var vals = sheetVal.Split(cusAtts.SplitChar);
                                object obj = field.FieldType.Assembly.CreateInstance(field.FieldType.Name);
                                string initMethod = string.IsNullOrEmpty(cusAtts.InitMethodName) ? "InitData" : cusAtts.InitMethodName;
                                MethodInfo inital = field.FieldType.GetMethod(initMethod);
                                for (int k = 0; k < cusAtts.Types.Length; k++)
                                {
                                    if (cusAtts.Types[k].Equals(typeof(GameObject)))
                                    {
                                        var paramObj = (GameObject)AssetDatabase.LoadAssetAtPath(Path.Combine(cusAtts.PrefabPath[prefabPathIndex], vals[k].ToString() + ".prefab"), typeof(GameObject));
                                        prefabPathIndex++;
                                        inital.Invoke(obj, new object[] { k, paramObj });
                                    }
                                    else
                                    {
                                        inital.Invoke(obj, new object[] { k, vals[k] });
                                    }
                                }
                                field.SetValue(instance, obj);
                            }
                            else
                            {
                                 field.SetValue(instance, null);
                            }
                        }
                        else if (Attribute.IsDefined(field, typeof(DataToObjectAttribute)))
                        {
                            string sheetVal = sheet[sheetName][i].ToString();
                            if (sheetVal != "0")
                            {
                                var cusAtts = Attribute.GetCustomAttribute(field, typeof(DataToObjectAttribute)) as DataToObjectAttribute;
                                var prefabPath = string.IsNullOrEmpty(cusAtts.PrefabPath) ? Path.Combine("Assets/", sheetVal + ".prefab") : Path.Combine(cusAtts.PrefabPath, sheetVal + ".prefab");
                                var obj = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                                field.SetValue(instance, Convert.ChangeType(obj, field.FieldType));
                            }
                            else
                            {
                                field.SetValue(instance, null);
                            }
                        }
                        else if (Attribute.IsDefined(field, typeof(DataSplitToObjectArrayAttribute)))
                        {
                            string sheetVal = sheet[sheetName][i].ToString();
                            if (sheetVal != "0")
                            {
                                var cusAtts = Attribute.GetCustomAttribute(field, typeof(DataSplitToObjectArrayAttribute)) as DataSplitToObjectArrayAttribute;
                                var datas = sheetVal.Split(cusAtts.SplitChar);
                                Array gos = Array.CreateInstance(cusAtts.CustomerType, datas.Length);
                                for (int j = 0; j < datas.Length; j++)
                                {
                                    if (cusAtts.CustomerType.Equals(typeof(GameObject)))
                                    {
                                        gos.SetValue(AssetDatabase.LoadAssetAtPath(Path.Combine(cusAtts.PrefabPath, datas[j] + ".prefab"), cusAtts.CustomerType), j);
                                    }
                                    else
                                    {
                                        gos.SetValue(Convert.ChangeType(datas[j], cusAtts.CustomerType), j);
                                    }

                                    //if (sheetName == "SlowMonitorButtonShielding")
                                    //{
                                    //    Debug.Log(Path.Combine(cusAtts.PrefabPath, datas[j] + ".prefab"));
                                    //    //if(gos.GetValue(j)!=null)
                                    //    //{

                                    //    //}
                                    //}
                                }
                                field.SetValue(instance, Convert.ChangeType(gos, field.FieldType));
                            }
                            else
                            {
                                field.SetValue(instance, null);
                            }
                        }
                        else if (Attribute.IsDefined(field, typeof(DataSplitToCustomerObjectArrayAttribute)))
                        {
                            string sheetVal = sheet[sheetName][i].ToString();
                            if (sheetVal != "0")
                            {
                                var cusAtts = Attribute.GetCustomAttribute(field, typeof(DataSplitToCustomerObjectArrayAttribute)) as DataSplitToCustomerObjectArrayAttribute;
                                var datas = sheetVal.Split(cusAtts.ParSplitChar);
                                Array objs=Array.CreateInstance(cusAtts.CustomerType, datas.Length);
                                for (int l = 0; l < datas.Length; l++)
                                {
                                    int prefabPathIndex = 0;
                                    var vals = datas[l].Split(cusAtts.SplitChar);
                                    object obj = cusAtts.CustomerType.Assembly.CreateInstance(cusAtts.CustomerType.Name);
                                    string initMethod = string.IsNullOrEmpty(cusAtts.InitMethodName) ? "InitData" : cusAtts.InitMethodName;
                                    MethodInfo inital = cusAtts.CustomerType.GetMethod(initMethod);
                                    for (int m = 0; m < cusAtts.Types.Length; m++)
                                    {
                                        if (cusAtts.Types[m].Equals(typeof(GameObject)))
                                        {
                                            //Debug.Log(sheetName+i+" "+ Path.Combine(cusAtts.PrefabPath[prefabPathIndex], vals[m].ToString() + ".prefab"));
                                            var paramObj = (GameObject)AssetDatabase.LoadAssetAtPath(Path.Combine(cusAtts.PrefabPath[prefabPathIndex], vals[m].ToString() + ".prefab"), typeof(GameObject));
                                            if (cusAtts.PrefabPath.Length > prefabPathIndex + 1)
                                            {
                                                prefabPathIndex++;
                                            }
                                            inital.Invoke(obj, new object[] { m, paramObj });
                                        }
                                        else
                                        {
                                            inital.Invoke(obj, new object[] { m, vals[m] });
                                        }
                                    }
                                    objs.SetValue(obj,l);                                    
                                }
                                field.SetValue(instance, Convert.ChangeType(objs, field.FieldType));                               
                            }
                            else
                            {
                                field.SetValue(instance, null);
                            }
                        }
                        else
                        {
                            if (Attribute.IsDefined(field, typeof(FieldMapAttribute)))
                            {
                                var cusAtts = Attribute.GetCustomAttribute(field, typeof(FieldMapAttribute)) as FieldMapAttribute;

                                sheetName = cusAtts.SheetNameOfData;
                            }                            
                            object sheetVal = sheet[sheetName][i];
                            //Debug.Log(sheetVal + " " + sheetName);
                            if (sheetVal.ToString().Trim() != "")
                            {
                                if (Attribute.IsDefined(field, typeof(EnumMapAttribute)))
                                {
                                    field.SetValue(instance, Enum.Parse(field.FieldType, sheetVal.ToString())); 
                                }
                                else
                                {
                                    field.SetValue(instance, Convert.ChangeType(sheetVal, field.FieldType));
                                }
                            }
                        }
                    }
                }
                tempList.Add(instance);
            }            
            if (!isOnlyReady)
            {
                CreateConfigDataBase<T, T2>(tempList);
                //Debug.Log("Count:" + tempList.Count);
            }
            return tempList;
        }
    }

    public static void CreateConfigDataBase<T, T2>(List<T2> list)
        where T : ConfigBase
        where T2 : class
    {
        // Asset文件名
        string fileName = typeof(T).ToString();
        // Asset文件存储路径
        string path = System.IO.Path.Combine(ASSET_ITEM_CONFIG_FOLDER, m_category);
        path = System.IO.Path.Combine(path, "Data/" + fileName + ".asset");

        if (File.Exists(path))
        {
            // 加载Asset文件
            T database = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            if (database == null)
            {
                // 如果加载失败则返回
                Debug.LogWarning("如果加载返回");
                return;
            }
            // 申请内存存储数据
            database.Init(list.Count, list);

            // 当资源已改变并需要保存到磁盘，Unity内部使用dirty标识来查找。
            EditorUtility.SetDirty(database);
        }
        // 如果没有Asset文件，则创建Asset文件
        else
        {           
            // 创建对象
            T database = ScriptableObject.CreateInstance<T>();
            // 申请内存存储数据 
            database.Init(list.Count, list);
            
            // 创建Asset文件
            AssetDatabase.CreateAsset(database, path);
            
        }
    }
}
