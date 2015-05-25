using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StroyLineEditor
{
    public enum CheckboxType
    {
        StroyLine,
        CameraID,
        NpcID,
    }

    public class StroyEditorUIManager : MonoBehaviour
    {
        enum WinType
        {
            CameraClip,
            CameraGroup,
            Animation,
            ActionParams,
            None,
        }

        public GameObject StroyLineItem;
        private List<GameObject> m_checkboxList = new List<GameObject>();
        private WinType m_winType = WinType.Animation;
        private bool m_isShowWindow = false;
        private bool m_isNewCreate = false;
        private string tooltip = "";
        private string m_curTitleName = "";
        private int m_cameraGroupCount = 0;
        private Rect m_windowRect = new Rect(Screen.width/2 - 80, 20, 160, 430);
        private List<int> m_curAnimList;
        GUIStyle m_guiStyle = new GUIStyle();
        
        private StroyActionConfigData m_actionData;
        private StroyCameraConfigData m_cameraData;
        private StroyLineConfigData m_stroyLineData;

        private static StroyEditorUIManager m_instance;
        public static StroyEditorUIManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new StroyEditorUIManager();
                }
                return m_instance;
            }
        }

        // Use this for initialization
        void Start()
        {
            m_instance = this;
            InitEditorUI();
            m_guiStyle.normal.textColor = Color.red;
            m_guiStyle.wordWrap = true;
        }

        void InitEditorUI()
        {
            foreach (KeyValuePair<StroyLineKey, StroyLineConfigData> item in StroyLineConfigManager.Instance.GetStroyLineConfig)
            {
                InitEditorUI(item.Value);
            }
        }

        int m_iRow = 0;
        void InitEditorUI(StroyLineConfigData stroyline)
        {
            GameObject editorItem = (GameObject)Instantiate(StroyLineItem);
            editorItem.transform.parent = this.transform;
            editorItem.transform.localScale = Vector3.one;
            editorItem.transform.localPosition = new Vector3(-500, 300 + m_iRow++ * -45, 0);
			editorItem.GetComponent<EditorCheckbox>().InitCheckbox(stroyline._EctypeID,stroyline._TriggerCondition, stroyline._StroyLineID.ToString(), CheckboxType.StroyLine, stroyline._TriggerVocation);
            m_checkboxList.Add(editorItem);
            UICheckbox uiCheckbox = editorItem.GetComponent<UICheckbox>();
            uiCheckbox.radioButtonRoot = this.transform;
        }

        void Reset()
        {
            foreach (GameObject go in m_checkboxList)
            {
                Destroy(go);
            }
            InitEditorUI();
            m_iRow = 0;
            m_isShowWindow = false;
        }

        void OnClick(GameObject go)
        {
            switch (go.name)
            {
                case "PlayButton":

                    StroyLineManager.Instance.ClearList();
                    StroyLineManager.Instance.ResetCameraGroup();
                    StroyLineManager.Instance.StartNextCameraGroup();
                    StroyLineManager.Instance.IsShowEditorUI = true;
                    EditorDataManager.Instance.RegPreStep(m_cameraData, m_actionData);
                    break;
                case "PreButton":
                    EditorDataManager.Instance.RestorePreStep(ref m_cameraData, ref m_actionData);                    
                    break;
                case "SaveButton":
                    AddSaveAction(m_actionData);
                    AddSaveCamera(m_cameraData);
                    EditorDataManager.Instance.SaveConfig();
                    break;
                case "RevertButton":
                    StroyLineConfigManager.Instance.RestroeAllList();
                    Reset();
                    break;
                default:
                    break;
            }

        }

        void OnGUI ()
        {
            if (m_isShowWindow == true)
           {
               m_windowRect = GUI.Window(0, m_windowRect, WindowContain, m_curTitleName);
           }
        }

        public void SetActionUiState(int npcID)
        {
            if (m_isNewCreate)
            {
                tooltip = ">>>请先保存数据!!<<<";
                return;
            }

            var npcAction = EditorDataManager.Instance.CurSelectCameraGroup._ActionList;
            for (int i = 0; i < npcAction.Length; i++)
            {
                if (npcAction[i].NpcID == npcID)
                {
                    m_curAnimList = npcAction[i].AnimID;
                    m_curTitleName = npcAction[i].NpcID + "动画列表";
                    EditorDataManager.Instance.CurSelectNpcAction = npcAction[i];
                    m_winType = WinType.Animation;
                    m_isShowWindow = true;
                    tooltip = "";
                }
            }
        }


        public void SetActionUiState(StroyActionConfigData actionData)
        {
            m_winType = WinType.ActionParams;
            m_actionData = actionData;
            m_curTitleName = "新建动作窗口";
            m_isNewCreate = true;
            m_isShowWindow = true;
            tooltip = "";
        }

        public void CreateStroyLineData(StroyLineConfigData stroyline)
        {
            m_winType = WinType.CameraGroup;
            m_curTitleName = "新建镜头组窗口";
            m_stroyLineData = stroyline;
            m_isNewCreate = true;
            m_isShowWindow = true;
        }

        public void SetCameraUiState(int cameraID)
        {
            if (m_isNewCreate)
            {
                tooltip = ">>>请先保存数据!!<<<";
                return;
            }
            
            if (StroyLineConfigManager.Instance.GetCameraConfig.ContainsKey(cameraID))
            {
                var curSelectCamera = StroyLineConfigManager.Instance.GetCameraConfig[cameraID];
                EditorDataManager.Instance.CurSelectCameraData = curSelectCamera;
                tooltip = "";
            }
            else
            {
                tooltip = "相机配置表中无" + cameraID + "相关数据！";
            }

            m_winType = WinType.CameraClip;
            m_isShowWindow = true;
            
            m_cameraData = EditorDataManager.Instance.CurSelectCameraData;
            m_curTitleName = m_cameraData._CameraID.ToString() + "相机参数";

        }

        public void SetCameraUiState(StroyCameraConfigData cameraData)
        {
            m_winType = WinType.CameraClip;
            m_isShowWindow = true;
            m_isNewCreate = true;
            m_cameraData = cameraData;
            m_curTitleName = "新建相机参数";
            tooltip = "";
        }

        void WindowContain(int windowID)
        {
            int width = 140;

            GUI.DragWindow(new Rect(0, 0, width, 30));

            switch (m_winType)
            {
                case WinType.Animation:
                    
                    if (tooltip.Length > 0)
                    {
                        GUILayout.Label(tooltip, m_guiStyle);
                        m_windowRect.height = (m_curAnimList.Count + 2) * 45;
                    }
                    else
                    {
                        m_windowRect.height = (m_curAnimList.Count + 1) * 45;
                    }


                    for (int i = 0; i < m_curAnimList.Count; i++)
                    {
                        if (GUILayout.Button(m_curAnimList[i].ToString(), GUILayout.Width(width), GUILayout.Height(35)))
                        {
                            if (StroyLineConfigManager.Instance.GetStroyActionConfig.ContainsKey(m_curAnimList[i]))
                            {
                                m_winType = WinType.ActionParams;
                                var curSelectAction = StroyLineConfigManager.Instance.GetStroyActionConfig[m_curAnimList[i]];
                                EditorDataManager.Instance.CurSelectActionData = curSelectAction;
                                m_curTitleName = m_curAnimList[i].ToString() + "动画参数";
                                m_actionData = EditorDataManager.Instance.CurSelectActionData;
                                tooltip = "";
                            }
                            else
                            {
                                tooltip = "配置表中无" + m_curAnimList[i] + "相关数据！";
                            }
                        }
                    }
                    if (GUILayout.Button("关闭", GUILayout.Width(width), GUILayout.Height(35)))
                    {
                        m_isShowWindow = false;
                    }
                    m_windowRect.height = (m_curAnimList.Count + 4) * 25;

                    break;
                case WinType.CameraClip:

                    int icRow = 0;
                    if (tooltip.Length > 0)
                    {
                        if (!m_isNewCreate)
                        {
                            if (GUI.Button(new Rect(10, 23 + 2 * 24, width, 25), "关闭"))
                            {
                                m_isShowWindow = false;
                            }
                            GUILayout.Label(tooltip, m_guiStyle);
                            m_windowRect.height = 5 * 25;
                            return;
                        }
                        icRow++;

                        GUILayout.Label(tooltip, m_guiStyle);
                    }
                    
                    
                    GUI.Label(new Rect(10, 23 + icRow * 24, 52, 25), "目标类型");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + icRow * 24, 88, 22), ref m_cameraData._TargetType);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 60.5f, 25), "目标位置X");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + icRow * 24, 78, 22), ref m_cameraData._TargetPos.x);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 60.5f, 25), "目标位置Y");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + icRow * 24, 78, 22), ref m_cameraData._TargetPos.y);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 52, 25), "目标ID");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + icRow * 24, 88, 22), ref m_cameraData._TargetID);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 52, 25), "偏移X");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + icRow * 24, 88, 22), ref m_cameraData._TargetOffset.x);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 52, 25), "偏移Y");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + icRow * 24, 88, 22), ref m_cameraData._TargetOffset.y);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 52, 25), "偏移Z");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + icRow * 24, 88, 22), ref m_cameraData._TargetOffset.z);

                    GUI.Label(new Rect(10, 23 + ++icRow * 24, 52, 25), "运动时间");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + icRow * 24, 88, 22), ref m_cameraData._ActionTime);

                    for (int i = 0; i < m_cameraData._Params.Length; i++)
                    {
                        int r = 0;
                        GUI.Label(new Rect(10, 23 + ++icRow * 24, 100, 25), "相机运动参数" + i);
                        StroyEditorCommand.ReadData(new Rect(10, 23 + ++icRow * 24, 35, 22),ref m_cameraData._Params[i]._EquA);
                        StroyEditorCommand.ReadData(new Rect(10 + ++r * 36, 23 + icRow * 24, 35, 22), ref m_cameraData._Params[i]._EquB);
                        StroyEditorCommand.ReadData(new Rect(10 + ++r * 36, 23 + icRow * 24, 35, 22), ref m_cameraData._Params[i]._EquC);
                        StroyEditorCommand.ReadData(new Rect(10 + ++r * 36, 23 + icRow * 24, 35, 22), ref m_cameraData._Params[i]._EquD);
                    }
                        

                    ++icRow;
                    if (m_isNewCreate)
                    {
                        if (GUI.Button(new Rect(10, 23 + ++icRow * 24, 65, 25), "取消"))
                        {

                            m_isShowWindow = false;
                            m_isNewCreate = false;
                        }

                        if (GUI.Button(new Rect(85, 23 + icRow * 24, 65, 25), "保存"))
                        {
                            m_isShowWindow = false;
                            m_isNewCreate = false;
                            AddSaveCamera(m_cameraData);

                            EditorDataManager.Instance.CurSelectCameraGroup._CameraID.Add(m_cameraData._CameraID);
                            StroyLineConfigManager.Instance.GetCameraConfig.Add(m_cameraData._CameraID, m_cameraData);
                            EditorDataManager.Instance.IsUpdateStroyUI = true;
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(10, 23 + ++icRow * 24, width, 25), "关闭"))
                        {
                            m_isShowWindow = false;
                        }
                    }
                    m_windowRect.height = (icRow + 2) * 25;

                    break;

                case WinType.ActionParams:
                    int iRow = 0;
                    if (tooltip.Length > 0)
                    {
                        iRow++;
                        GUILayout.Label(tooltip, m_guiStyle);
                    }
                    GUI.Label(new Rect(10, 23 + iRow * 24, 52, 25), "动画名称");
                    m_actionData._ActionName = GUI.TextField(new Rect(65, 23 + iRow * 24, 88, 22), m_actionData._ActionName);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 60.5f, 25), "起始位置X");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + iRow * 24, 78, 22), ref m_actionData._StartPosition.x);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 60.5f, 25), "起始位置Y");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + iRow * 24, 78, 22), ref m_actionData._StartPosition.z);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "动画类型");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + iRow * 24, 88, 22),ref m_actionData._ActionType);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "起始角度");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + iRow * 24, 88, 22), ref m_actionData._StartAngle);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "移动速度");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + iRow * 24, 88, 22), ref m_actionData._Speed);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 42, 25), "加速度");
                    StroyEditorCommand.ReadData(new Rect(55, 23 + iRow * 24, 98, 22), ref m_actionData._Acceleration);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "持续时间");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + iRow * 24, 88, 22), ref m_actionData._Duration);
				
                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "特效名");
                    GUI.TextField(new Rect(55, 23 + iRow * 24, 98, 22), (m_actionData._EffectGo == null ? "null" : m_actionData._EffectGo.name));

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "特效时间");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + iRow * 24, 88, 22), ref m_actionData._EffectStartTime);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 60.5f, 25), "特效位置X");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + iRow * 24, 78, 22), ref m_actionData._EffectPosition.x);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 60.5f, 25), "特效位置Y");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + iRow * 24, 78, 22), ref m_actionData._EffectPosition.y);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 60.5f, 25), "特效位置Z");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + iRow * 24, 78, 22), ref m_actionData._EffectPosition.z);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 80, 25), "特效循环次数");
                    StroyEditorCommand.ReadData(new Rect(93, 23 + iRow * 24, 60, 22), ref m_actionData._EffectLoopTimes);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 52, 25), "声音时间");
                    StroyEditorCommand.ReadData(new Rect(65, 23 + iRow * 24, 88, 22), ref m_actionData._SoundTime);

                    GUI.Label(new Rect(10, 23 + ++iRow * 24, 42, 25), "声音名");
                    m_actionData._SoundName = GUI.TextField(new Rect(55, 23 + iRow * 24, 98, 22), m_actionData._SoundName);

                    //GUILayout.Label("动画名称",GUILayout.Width(60),GUILayout.Height(25));
                    //actionData._ActionName = GUILayout.TextField(actionData._ActionName, GUILayout.Width(50), GUILayout.Height(25));
                    //actionData._Acceleration = Convert.ToSingle(GUILayout.TextField(actionData._Acceleration.ToString(), GUILayout.Width(100), GUILayout.Height(25)));
                    ++iRow;
                    
                    if (m_isNewCreate)
                    {
                        if (GUI.Button(new Rect(10, 23 + ++iRow * 24, 65, 25), "取消"))
                        {

                            m_isShowWindow = false;
                            m_isNewCreate = false;
                        }

                        if (GUI.Button(new Rect(85, 23 + iRow * 24, 65, 25), "保存"))
                        {
                            m_winType = WinType.Animation;

                            //m_isShowWindow = false;
                            m_isNewCreate = false;
                            AddSaveAction(m_actionData);

                            EditorDataManager.Instance.CurSelectNpcAction.AnimID.Add(m_actionData._ActionID);
                            m_curAnimList = EditorDataManager.Instance.CurSelectNpcAction.AnimID;
                            m_curTitleName = EditorDataManager.Instance.CurSelectNpcAction.NpcID + "动画列表";
                            StroyLineConfigManager.Instance.GetStroyActionConfig.Add(m_actionData._ActionID, m_actionData);
                        }
                    }
                    else
                    {
                        if (GUI.Button(new Rect(10, 23 + ++iRow * 24, width, 25), "返回"))
                        {
                            m_winType = WinType.Animation;
                            m_curTitleName = EditorDataManager.Instance.CurSelectNpcAction.NpcID + "动画列表";
                        }
                    }

                    m_windowRect.height = (iRow + 2) * 25;
                    break;

                case WinType.CameraGroup:
                    int icgRow = 0;
                    if (tooltip.Length > 0)
                    {
                        icgRow++;
                        GUILayout.Label(tooltip, m_guiStyle);
                    }

                    GUI.Label(new Rect(10, 23 + icgRow * 24, 52, 25), "背景音乐");
                    m_stroyLineData._BgMusic = GUI.TextField(new Rect(65, 23 + icgRow * 24, 88, 22), m_stroyLineData._BgMusic);

                    GUI.Label(new Rect(10, 23 + ++icgRow * 24, 60.5f, 25), "触发副本");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + icgRow * 24, 78, 22), ref m_stroyLineData._EctypeID);

                    GUI.Label(new Rect(10, 23 + ++icgRow * 24, 60.5f, 25), "加载地图");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + icgRow * 24, 78, 22), ref m_stroyLineData._SceneMapID);

                    GUI.Label(new Rect(10, 23 + ++icgRow * 24, 60.5f, 25), "触发条件");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + icgRow * 24, 78, 22), ref m_stroyLineData._TriggerCondition);

                    GUI.Label(new Rect(10, 23 + ++icgRow * 24, 60.5f, 25), "剧情标识");
                    StroyEditorCommand.ReadData(new Rect(74, 23 + icgRow * 24, 78, 22), ref m_stroyLineData._StroyLineID);

                    if (GUI.Button(new Rect(10, 23 + ++icgRow * 24, width, 25), "增加镜头组"))
                    {
                        m_stroyLineData._CameraGroup.Add(0);
                    }

                    for (int i = 0; i < m_stroyLineData._CameraGroup.Count; i++)
                    {
                        GUI.Label(new Rect(10, 25 + ++icgRow * 24, 60.5f, 25), "镜头组");
                        var inputStr = GUI.TextField(new Rect(74, 23 + icgRow * 24, 78, 22), m_stroyLineData._CameraGroup[i].ToString());
                        m_stroyLineData._CameraGroup[i] = Convert.ToInt32((inputStr.Length == 0) ? "0" : inputStr);
                    }

                    ++icgRow; 
                    if (m_isNewCreate)
                    {
                        if (GUI.Button(new Rect(10, 23 + ++icgRow * 24, 65, 25), "取消"))
                        {

                            m_isShowWindow = false;
                            m_isNewCreate = false;
                        }

                        if (GUI.Button(new Rect(85, 23 + icgRow * 24, 65, 25), "保存"))
                        {
                            m_isShowWindow = false;
                            m_isNewCreate = false;
                            int vocation = PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;

						StroyLineConfigManager.Instance.GetStroyLineConfig.Add(new StroyLineKey { VocationID = vocation, ConditionID = m_stroyLineData._TriggerCondition, EctypeID = m_stroyLineData._EctypeID }, m_stroyLineData);
						EditorDataManager.Instance.SetCurSelectEctypeID(m_stroyLineData._EctypeID,m_stroyLineData._TriggerCondition, m_stroyLineData._TriggerVocation);
                            InitEditorUI(m_stroyLineData);
                            EditorDataManager.Instance.StroyLineTempData.Add(m_stroyLineData);
                        }
                    }

                    m_windowRect.height = (icgRow + 2) * 25;
                    break;
                default:
                    break;
            }

        }

        private void AddSaveAction(StroyActionConfigData actionData)
        {
            if (actionData == null)
            {
                return;
            }

            for (int i = 0; i < EditorDataManager.Instance.StroyActionTempData.Count; i++ )
            {
                if (EditorDataManager.Instance.StroyActionTempData[i]._ActionID == actionData._ActionID)
                {
                    EditorDataManager.Instance.StroyActionTempData.RemoveAt(i);
                }
            }

            EditorDataManager.Instance.StroyActionTempData.Add(actionData);
        }

        private void AddSaveCamera(StroyCameraConfigData cameraData)
        {
            if (cameraData == null)
            {
                return;
            }

            for (int i = 0; i < EditorDataManager.Instance.StroyCameraTempData.Count; i++)
            {
                if (EditorDataManager.Instance.StroyCameraTempData[i]._CameraID == cameraData._CameraID)
                {
                    EditorDataManager.Instance.StroyCameraTempData.RemoveAt(i);
                }
            }
            EditorDataManager.Instance.StroyCameraTempData.Add(cameraData);


        }

    }
}
