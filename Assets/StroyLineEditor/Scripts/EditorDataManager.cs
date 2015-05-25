using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace StroyLineEditor
{
    public class EditorDataManager
    {
        private List<StroyActionConfigData> m_actionConfigList = new List<StroyActionConfigData>();
        private List<StroyCameraConfigData> m_cameraConfigList = new List<StroyCameraConfigData>();
        private List<CameraGroupConfigData> m_cameraGroupConfigList = new List<CameraGroupConfigData>();
        private List<StroyLineConfigData> m_stroyLineConfigList = new List<StroyLineConfigData>();

        private StroyLineKey m_curSelectEctypeKey;
        private int m_curSelectEctypeID = 0;

        private static EditorDataManager m_instance;
        public static EditorDataManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new EditorDataManager();
                }
                return m_instance;
            }
        }
       
        /// <summary>
        /// 当前选择的NpcAction
        /// </summary>
        public NpcAction CurSelectNpcAction { set; get; }
        /// <summary>
        /// 当前UI选择的摄像机
        /// </summary>
        public StroyCameraConfigData CurSelectCameraData { set; get; }

        /// <summary>
        /// 当前UI选择的镜头组
        /// </summary>
        public CameraGroupConfigData CurSelectCameraGroup { set; get; }

        /// <summary>
        /// 当前UI选择的Action
        /// </summary>
        public StroyActionConfigData CurSelectActionData { set; get; }

        public string GetCurSceneName
        {
            get
            {
                var mapID = CurSelectStroyData._SceneMapID;
                return EctypeConfigManager.Instance.SceneConfigList[mapID]._szSceneName;
            }
        }

        public int GetCurMapId
        {
            get { return CurSelectStroyData._SceneMapID; }
        }


        ///
        ///当前UI选择的剧情
        ///
        public void SetCurSelectEctypeID(int stroyID,int condition, int vocation)
        {
			//int vocation = 1;//PlayerManager.Instance.FindHeroDataModel().PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;
			m_curSelectEctypeKey = new StroyLineKey { VocationID = vocation,ConditionID = condition, EctypeID = stroyID };
            m_curSelectEctypeID = stroyID;
            IsUpdateStroyUI = true;
        }

        public StroyLineConfigData CurSelectStroyData
        {
            get { return StroyLineConfigManager.Instance.GetStroyLineConfig[m_curSelectEctypeKey]; }
        }

        /// <summary>
        /// 待保存剧情的数据列表
        /// </summary>
        public List<StroyLineConfigData> StroyLineTempData
        {
            get { return m_stroyLineConfigList; }
        }
        /// <summary>
        /// 待保存动作的数据列表
        /// </summary>
        public List<StroyActionConfigData> StroyActionTempData
        {
            get { return m_actionConfigList; }
        }
        /// <summary>
        /// 待保存摄像机的数据列表
        /// </summary>
        public List<StroyCameraConfigData> StroyCameraTempData
        {
            get { return m_cameraConfigList; }
        }

        public void SubParams(GroupType groupType ,string param)
        {
            switch (groupType)
            {
                case GroupType.MAPID:
                    m_stroyLineConfigList.ApplyAllItem(P =>
                    {
                        if (P._EctypeID == m_curSelectEctypeID)
                        {
                            P._SceneMapID = Convert.ToInt32(param);
                        }
                    });
                    break;
                case GroupType.BGMUSIC:
                    m_stroyLineConfigList.ApplyAllItem(P =>
                    {
                        if (P._EctypeID == m_curSelectEctypeID)
                        {
                            P._BgMusic = param;
                        }
                    });
                    break;
                case GroupType.CONDITION:
                    m_stroyLineConfigList.ApplyAllItem(P =>
                    {
                        if (P._EctypeID == m_curSelectEctypeID)
                        {
                            P._TriggerCondition = Convert.ToInt32(param);
                        }
                    });
                    break;
                case GroupType.ECTYPEID:
                    m_stroyLineConfigList.ApplyAllItem(P =>
                    {
                        if (P._EctypeID == m_curSelectEctypeID)
                        {
                            P._EctypeID = Convert.ToInt32(param);
                        }
                    });
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 是否更新数据
        /// </summary>
        public bool IsUpdateStroyUI { set; get; }

        /// <summary>
        /// 输出保存配置文件
        /// </summary>
        public void SaveConfig()
        {
            if (m_stroyLineConfigList.Count > 0)
            {
                StroyEditorCommand.SaveConfigAsCSV("StroyLineConfig.csv", m_stroyLineConfigList);
            }

            if (m_cameraGroupConfigList.Count > 0)
            {
                StroyEditorCommand.SaveConfigAsCSV("CameraGroupConfig.csv", m_cameraGroupConfigList);
            }

            if (m_cameraConfigList.Count > 0)
            {
                StroyEditorCommand.SaveConfigAsCSV("CameraConfig.csv", m_cameraConfigList);
            }

            if (m_actionConfigList.Count > 0)
            {
                StroyEditorCommand.SaveConfigAsCSV("ActionConfig.csv", m_actionConfigList);
            }
        }

        #region 回退功能实现
        /// <summary>
        /// 回退功能实现
        /// </summary>
        private List<StroyCameraConfigData> m_cameraList = new List<StroyCameraConfigData>();
        private List<StroyActionConfigData> m_actionList = new List<StroyActionConfigData>();
        private const int MAX_RESTORESTEP = 10;
        /// <summary>
        /// 注册回退操作步骤
        /// </summary>
        /// <param name="camera">要保存的镜头数据</param>
        /// <param name="action">要保存的动作数据</param>
        public void RegPreStep(StroyCameraConfigData camera, StroyActionConfigData action)
        {
            if (m_cameraList.Count > MAX_RESTORESTEP)
                m_cameraList.RemoveAt(0);

            if (m_actionList.Count > MAX_RESTORESTEP)
                m_actionList.RemoveAt(0);

            if(camera != null)
                m_cameraList.Add(camera.Clone());

            if (action != null)
                m_actionList.Add(action.Clone());

        }
        /// <summary>
        /// 回退函数
        /// </summary>
        /// <param name="camera">要回退的镜头数据</param>
        /// <param name="action">要回退的动作数据</param>
        public void RestorePreStep(ref StroyCameraConfigData camera, ref StroyActionConfigData action)
        {
            int cameraCount = m_cameraList.Count - 2;
            if (cameraCount >= 0)
            {
                camera._ActionTime = m_cameraList[cameraCount]._ActionTime;
                camera._TargetID = m_cameraList[cameraCount]._TargetID;
                camera._TargetPos = m_cameraList[cameraCount]._TargetPos;
                camera._TargetType = m_cameraList[cameraCount]._TargetType;
                for (int i = 0; i < m_cameraList[cameraCount]._Params.Length; i++)
                {
                    camera._Params[i]._EquA = m_cameraList[cameraCount]._Params[i]._EquA;
                    camera._Params[i]._EquB = m_cameraList[cameraCount]._Params[i]._EquB;
                    camera._Params[i]._EquC = m_cameraList[cameraCount]._Params[i]._EquC;
                    camera._Params[i]._EquD = m_cameraList[cameraCount]._Params[i]._EquD;
                }
                m_cameraList.RemoveAt(cameraCount);
            }

            int actionCount = m_actionList.Count - 2;
            if (actionCount >= 0)
            {
                action._Acceleration = m_actionList[actionCount]._Acceleration;
                action._ActionName = m_actionList[actionCount]._ActionName;
                action._ActionType = m_actionList[actionCount]._ActionType;
                action._Duration = m_actionList[actionCount]._Duration;
                action._EffectLoopTimes = m_actionList[actionCount]._EffectLoopTimes;
                action._EffectPosition = m_actionList[actionCount]._EffectPosition;
                action._EffectStartTime = m_actionList[actionCount]._EffectStartTime;
                action._SoundName = m_actionList[actionCount]._SoundName;
                action._SoundTime = m_actionList[actionCount]._SoundTime;
                action._Speed = m_actionList[actionCount]._Speed;
                action._StartAngle = m_actionList[actionCount]._StartAngle;
                action._StartPosition = m_actionList[actionCount]._StartPosition;
                m_actionList.RemoveAt(actionCount);
            }
        }
        #endregion
    }
        
}