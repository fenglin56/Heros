using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ArrangeTool : EditorWindow
{

    GameObject ObjRoot;
    string wide = "0";
    string high = "0";
    string wideCount = "1";
    string KeyWord = "";
    Vector3 RootPosition;
    List<Transform> ObjList = new List<Transform>();
	
    [MenuItem("Window/MyTool/ArrangeTool")]
    static void Init()
    {
        ArrangeTool arrangeTool = (ArrangeTool)EditorWindow.GetWindow(typeof(ArrangeTool));
    }

    void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("节点：");
        ObjRoot = EditorGUILayout.ObjectField(ObjRoot, typeof(GameObject)) as GameObject;
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("关键词：");
        KeyWord = EditorGUILayout.TextField(KeyWord);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("宽度：");
        wide = EditorGUILayout.TextField(wide.ToString());
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("高度：");
        high = EditorGUILayout.TextField(high.ToString());
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("换行个数：");
        wideCount = EditorGUILayout.TextField(wideCount.ToString());
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("获取关键字列表"))
        {
            Transform parent = ObjRoot.transform.parent;
            Transform[] allComponnet = parent.GetComponentsInChildren<Transform>();
            ObjList.Clear();
            foreach (Transform child in allComponnet)
            {
                if (child.name.IndexOf(KeyWord) != -1)
                {
                    ObjList.Add(child);
                }
            }
        }
        if (GUILayout.Button("获取选中物体"))
        {
            ObjList.Clear();
            var getObj = Selection.gameObjects;
            getObj.ApplyAllItem(P => ObjList.Add(P.transform));
        }
        if (GUILayout.Button("确定"))
        {
            int intWide = int.Parse(wide);
            int intHigh = int.Parse(high);
            int intwideCount = int.Parse(wideCount);
            RootPosition = ObjRoot.transform.localPosition;
            int j = 0;
            for (int i = 0; i < ObjList.Count; i++)
            {
                j = i / intwideCount;
                ObjList[i].localPosition = RootPosition + new Vector3((i-j*intwideCount) * intWide, j * intHigh, 0);
                //ObjList[i].name = ObjList[i].name + i.ToString();
                //Debug.Log(ObjList[i].name);
            }
            //Debug.Log("AllComponent:" + allComponnet.Length + "," + ObjList.Count);
        }
        EditorGUILayout.EndHorizontal();
        Transform deleteTransform = null;
        Transform upIndexTransform = null;
        for (int i = 0; i < ObjList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.ToString());
            ObjList[i] = EditorGUILayout.ObjectField(ObjList[i], typeof(Transform)) as Transform;
            if (GUILayout.Button("X"))
            {
                deleteTransform = ObjList[i];
            }
            if (GUILayout.Button("^"))
            {
                upIndexTransform = ObjList[i];
            }
            EditorGUILayout.EndHorizontal();
        }
        if (deleteTransform != null)
        {
            ObjList.Remove(deleteTransform);
        }
        if (upIndexTransform != null)
        {
            int index = ObjList.IndexOf(upIndexTransform);
            if (index > 0)
            {
                ObjList.Remove(upIndexTransform);
                ObjList.Insert(index - 1, upIndexTransform);
            }
        }
    }

}
