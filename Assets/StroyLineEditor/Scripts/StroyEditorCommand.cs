using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;
using UnityEngine;

namespace StroyLineEditor
{

    public static class StroyEditorCommand
    {

        // 定义配置文件路径
#if UNITY_EDITOR
        private static string configPath = "Assets/StroyLineEditor/Config";
#else
    private static string configPath = Application.dataPath.Substring(0, Application.dataPath.Length - 1 - System.IO.Path.GetFileName(Application.dataPath).Length) + "/StroyLineEditor/";
#endif

        /// <summary>
        /// 保存配置文件至CSV
        /// </summary>
        /// <typeparam name="T">数据结构类型</typeparam>
        /// <param name="fileName">保存配置文件名</param>
        /// <param name="listModel">数据列表</param>
        /// <returns></returns>
        public static bool SaveConfigAsCSV<T>(string fileName, IList<T> listModel) where T : class, new()
        {
            bool flag = false;
            try
            {
                StringBuilder sb = new StringBuilder();
                //通过反射 显示要显示的列
                //BindingFlags bf =  BindingFlags.Instance | BindingFlags.NonPublic |  BindingFlags.Static | BindingFlags.Public;//反射标识
                Type objType = typeof(T);

                FieldInfo[] propInfoArr = objType.GetFields();

                string header = string.Empty;
                List<string> listPropertys = new List<string>();
                foreach (FieldInfo info in propInfoArr)
                {
                    if (string.Compare(info.Name.ToUpper(), "ID") != 0) //不考虑自增长的id或者自动生成的guid等
                    {
                        if (!listPropertys.Contains(info.Name))
                        {
                            listPropertys.Add(info.Name);
                        }
                        header += info.Name + ",";
                    }
                }
                sb.AppendLine(header.Trim(',')); //csv头

                foreach (T model in listModel)
                {
                    string strModel = string.Empty;
                    foreach (string strProp in listPropertys)
                    {
                        foreach (FieldInfo propInfo in propInfoArr)
                        {
                            if (string.Compare(propInfo.Name.ToUpper(), strProp.ToUpper()) == 0)
                            {
                                FieldInfo modelProperty = model.GetType().GetField(propInfo.Name);
                                if (modelProperty != null)
                                {
                                    //object objResult = modelProperty.GetValue(model, null);
                                    object objResult = modelProperty.GetValue(model);
                                    string result = "";

                                    if (objResult.GetType().Equals(typeof(Vector2)))
                                    {
                                        Vector2 array = (Vector2)modelProperty.GetValue(model);
                                        result = array.x.ToString() + "+" + array.y.ToString();
                                    }
                                    else if (objResult.GetType().IsArray)
                                    {
                                        Type valueType = modelProperty.FieldType;
                                        if (valueType.Equals(typeof(CameraParam[])))
                                        {
                                            CameraParam[] array = (CameraParam[])modelProperty.GetValue(model);
                                            for (int i = 0; i < array.Length; i++ )
                                            {
                                                result += array[i]._EquA.ToString() + "+" + array[i]._EquB.ToString()
                                                    + "+" + array[i]._EquC.ToString() + "+" +array[i]._EquD.ToString();
                                                if (i < array.Length - 1)
                                                {
                                                    result += "+";
                                                }
                                            }
                                        }
                                        else if (valueType.Equals(typeof(int[])))
                                        {
                                            int[] array = (int[])modelProperty.GetValue(model);
                                            for (int i = 0; i < array.Length; i++)
                                            {
                                                result += array[i].ToString();
                                                if (i < array.Length - 1)
                                                {
                                                    result += "+";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            UnityEngine.Debug.Log("当前数组未作处理，请检查" + valueType);
                                        }
                                    }
                                    else if(objResult.GetType().Equals(typeof(List<Int32>)))
                                    {
                                        List<Int32> array = (List<Int32>)modelProperty.GetValue(model);
                                        for (int i = 0; i < array.Count; i++)
                                        {
                                            result += array[i].ToString();
                                            if (i < array.Count - 1)
                                            {
                                                result += "+";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        result = ((objResult == null) ? string.Empty : objResult).ToString().Trim();
                                    }
                                    
                                    if (result.IndexOf(',') != -1)
                                    {
                                        result = "\"" + result.Replace("\"", "\"\"") + "\""; //特殊字符处理 ？
                                        //result = result.Replace("\"", "“").Replace(',', '，') + "\"";
                                    }
                                    if (!string.IsNullOrEmpty(result))
                                    {
                                        Type valueType = modelProperty.FieldType;
                                        if (valueType.Equals(typeof(Nullable<decimal>)))
                                        {
                                            result = decimal.Parse(result).ToString("#.#");
                                        }
                                        else if (valueType.Equals(typeof(decimal)))
                                        {
                                            result = decimal.Parse(result).ToString("#.#");
                                        }
                                        else if (valueType.Equals(typeof(Nullable<double>)))
                                        {
                                            result = double.Parse(result).ToString("#.#");
                                        }
                                        else if (valueType.Equals(typeof(double)))
                                        {
                                            result = double.Parse(result).ToString("#.#");
                                        }
                                        else if (valueType.Equals(typeof(Nullable<float>)))
                                        {
                                            result = float.Parse(result).ToString("#.#");
                                        }
                                        else if (valueType.Equals(typeof(float)))
                                        {
                                            result = float.Parse(result).ToString("#.#");
                                        }

                                    }
                                    
                                    strModel += result + ",";

                                }
                                else
                                {

                                    strModel += ",";
                                    UnityEngine.Debug.Log("+++" + strModel);
                                }
                                break;
                            }
                        }
                    }

                    strModel = strModel.Substring(0, strModel.Length - 1);
                    
                    sb.AppendLine(strModel);
                }
                string content = sb.ToString();
                string dir = Directory.GetCurrentDirectory();

                string fullName = Path.Combine(configPath, fileName);

                if (File.Exists(fullName)) File.Delete(fullName);
                using (FileStream fs = new FileStream(fullName, FileMode.CreateNew, FileAccess.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Flush();
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
                flag = true;
                StroyLineManager.Instance.SetSaveName += string.Format("{0}保存成功...\n", fileName);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public static void ReadData(Rect rect, ref float value)
        {
            float.TryParse(GUI.TextField(rect, value.ToString()), out value);
            //var inputStr = GUI.TextField(rect, value.ToString());
            //value = Convert.ToSingle((inputStr.Length == 0) ? "0" : inputStr);
        }

        public static void ReadData(Rect rect, ref int value)
        {
            int.TryParse(GUI.TextField(rect, value.ToString()),out value);
            //var inputStr = GUI.TextField(rect, value.ToString());
            //value = Convert.ToInt32((inputStr.Length == 0) ? "0" : inputStr);
        }
    }

    
}