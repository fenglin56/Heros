using UnityEngine;
using System.Collections;

namespace UI.Town
{

    public class HeroUpgradeLevelData
    {

        private static HeroUpgradeLevelData m_instance;
        public static HeroUpgradeLevelData Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new HeroUpgradeLevelData();
                }
                return m_instance;
            }
        }

        int level = 0;

        int Level {
            get
            {
                return level; 
            }
            set 
            {
                level = value;
                //Debug.LogWarning("SetLevelInMylevel:"+value);
            }
        }

        public HeroUpgradeLevelData()
        {
            //Debug.LogWarning("NewLEvelSaveData");
            GameDataManager.Instance.dataEvent.RegisterEvent(DataType.LoadingSceneData, OnSceneChange);
        }

        void OnSceneChange(object obj)
        {
            LoadSceneData m_LoadSceneData = obj as LoadSceneData;
            switch (m_LoadSceneData.loadSceneType)
            {
                case LoadSceneData.LoadSceneType.Login://退出游戏时触发
                    OnGameExit();
                    break;
                case LoadSceneData.LoadSceneType.Battle:
                    break;
                case LoadSceneData.LoadSceneType.Town:
                    break;
                default:
                    break;
            }
        }

        public void SaveLevel(int saveLevel)
        {
            Level = saveLevel;
            //TraceUtil.Log("SaveLEvel:" + Level);
        }

        public int GetLevel()
        {
            //TraceUtil.Log("GetLEvel:" + Level);
            return Level;
        }
        
        public void OnGameExit()
        {
            //Debug.LogWarning("ClearUpLevel");
            Level = 0;
            m_instance = null;
            GameDataManager.Instance.dataEvent.RemoveEventHandel(DataType.LoadingSceneData, OnSceneChange);
        }
    }
}