
using UnityEngine;
using System.Collections;
using UnityEditor;

public class AddScriptTool :  EditorWindow
{
    string FindTarget;
    [MenuItem("Tools/AddGuangHuanScriptTool")]
    static void Test()
    {
        AddScriptTool editor = EditorWindow.GetWindow(typeof(AddScriptTool), false, "批量添加光环脚本") as AddScriptTool;
        editor.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("填写要添加脚本的特效名", EditorStyles.boldLabel);
        FindTarget = EditorGUILayout.TextField(FindTarget);   
        if (GUILayout.Button("增加"))
        {
            AddScript();
        }
        this.Repaint();
    }

    private void AddScript()
    {
        Selection.gameObjects.ApplyAllItem(p =>
            {
                Transform result;
                p.transform.RecursiveFindObject(FindTarget, out result);
                if (result != null)
                {
                    if (result.gameObject.GetComponent<GuangHuanBehaviour>() == null)
                    {
                        result.gameObject.AddComponent<GuangHuanBehaviour>();
                    }                    
                }
            });
    }
}

