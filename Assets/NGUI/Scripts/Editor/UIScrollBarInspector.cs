//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2013 Tasharen Entertainment
//----------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UIScrollBar))]
public class UIScrollBarInspector : Editor
{
    int StepPointCount = 0;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeControls(100f);
        UIScrollBar sb = target as UIScrollBar;

        NGUIEditorTools.DrawSeparator();

        float val = EditorGUILayout.Slider("Value", sb.scrollValue, 0f, 1f);
        float size = EditorGUILayout.Slider("Size", sb.barSize, 0f, 1f);
        float alpha = EditorGUILayout.Slider("Alpha", sb.alpha, 0f, 1f);
        UIFilledSprite ProgressBackground = (UIFilledSprite)EditorGUILayout.ObjectField("进度条", sb.ProgressBackground, typeof(UIFilledSprite), true);
        float ForegroundZDepth = EditorGUILayout.FloatField("拖拽按钮Z坐标", sb.ForegroundZDepth);
        Vector2 foregroundSize = EditorGUILayout.Vector2Field("拖拽按钮缩放比", sb.ForegroundScaleSize);
        GUILayout.BeginHorizontal();
        StepPointCount = EditorGUILayout.IntField("节点个数", StepPointCount);
        if (GUILayout.Button("确定"))
        {
            sb.StepList = new float[StepPointCount];
        }
        GUILayout.EndHorizontal();

        if (sb.StepList != null)
        {
            for (int i = 0; i < sb.StepList.Length; i++)
            {
                float value = EditorGUILayout.FloatField("节点" + i, sb.StepList[i]);
                value = value > 1 ? 1 : value;
                value = value < 0 ? 0 : value;
                sb.StepList[i] = value;
            }
        }

        NGUIEditorTools.DrawSeparator();

        UISprite bg = (UISprite)EditorGUILayout.ObjectField("Background", sb.background, typeof(UISprite), true);
        UISprite fg = (UISprite)EditorGUILayout.ObjectField("Foreground", sb.foreground, typeof(UISprite), true);
        UIScrollBar.Direction dir = (UIScrollBar.Direction)EditorGUILayout.EnumPopup("Direction", sb.direction);
        bool inv = EditorGUILayout.Toggle("Inverted", sb.inverted);

        if (sb.scrollValue != val ||
            sb.barSize != size ||
            sb.background != bg ||
            sb.foreground != fg ||
            sb.direction != dir ||
            sb.inverted != inv ||
            sb.alpha != alpha ||
            sb.ForegroundScaleSize.x != foregroundSize.x ||
            sb.ForegroundScaleSize.y != foregroundSize.y ||
            sb.ForegroundZDepth != ForegroundZDepth ||
            sb.ProgressBackground != ProgressBackground)
        {
            NGUIEditorTools.RegisterUndo("Scroll Bar Change", sb);
            sb.scrollValue = val;
            sb.barSize = size;
            sb.inverted = inv;
            sb.background = bg;
            sb.foreground = fg;
            sb.direction = dir;
            sb.alpha = alpha;
            sb.ForegroundScaleSize = foregroundSize;
            sb.ForegroundZDepth = ForegroundZDepth;
            sb.ProgressBackground = ProgressBackground;
            UnityEditor.EditorUtility.SetDirty(sb);
        }
    }

}