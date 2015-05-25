using System;
using UnityEngine;
using UnityEditor;

// 配置表信息的枚举
public enum ConfigDataTypes
{
    TaskConfig,
    NewGuideConfig,
    GuideTalkIdConfig,
    MainTownButtonConfig,
    InitMainTownButtonConfig, 
}

public class ConfigEditor : EditorWindow
{
    // 当前选择的配置表
    public ConfigDataTypes m_currentType;

    public static ConfigEditor window;
    // Unity菜单按钮
    [MenuItem("MyEditor/ConfigEditor")]
    // 点击该菜单按钮执行的函数
    static void Execute()
    {
        if (window == null)
        {
            window = EditorWindow.GetWindow<ConfigEditor>(false, "Config Editor", true);
            window.Show();
        }
    }
    void OnGUI()
    {
        // 描述性文字
        GUILayout.Label("Select a ConfigType first.");
        // 横线
        NGUIEditorTools.DrawSeparator();
        // 隔两个像素
        GUILayout.Space(2);
        // 当前配置表下来框
        m_currentType = (ConfigDataTypes)EditorGUILayout.EnumPopup("ConfigType", m_currentType, GUILayout.Width(400f));
        // 横线
        NGUIEditorTools.DrawSeparator();
        // 描述性文字
        GUILayout.Label("When Ready.");
        // 创建配置表对应的Asset文件按钮
        bool create = GUILayout.Button("Create", GUILayout.Width(120f));
        if (create) CreateConfigFile();
    }
    /// <summary>
    /// 创建配置表对应的Asset文件
    /// </summary>
    private void CreateConfigFile()
    {
        switch (m_currentType)
        {
            // AI数据配置表
            case ConfigDataTypes.TaskConfig: CommonDataReader.ConfigPostprocess<TaskNewConfigDataBase, TaskNewConfigData>("TaskConfig", "TaskConfig.xml", false); break;
            case ConfigDataTypes.NewGuideConfig: CommonDataReader.ConfigPostprocess<NewGuideConfigDataBase, NewGuideConfigData>("TaskConfig", "NewbieGuideConfig.xml", false); break;
            case ConfigDataTypes.MainTownButtonConfig: CommonDataReader.ConfigPostprocess<MainTownButtonConfigDataBase, MainTownButtonConfigData>("TaskConfig", "MainButtonConfig.xml", false); break;
            case ConfigDataTypes.InitMainTownButtonConfig: CommonDataReader.ConfigPostprocess<InitMainTownButtonDataBase, InitMainTownButtonData>("TaskConfig", "InitMainButton.xml", false); break;
            case ConfigDataTypes.GuideTalkIdConfig: CommonDataReader.ConfigPostprocess<TalkIdConfigDataBase, TalkIdConfigData>("TaskConfig", "TalkIDlist.xml", false); break;
            default: break;
        }
    }
}
