using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StroyLineEditor
{
    public class StroyLineEditor : MonoBehaviour
    {
        private bool m_isInitTerrain = true;

        void Awake()
        {
            Invoke("LoadTerrainData", 1f);
        }

        void LoadTerrainData()
        {
            string sceneName = EditorDataManager.Instance.GetCurSceneName;
            int mapId = EditorDataManager.Instance.GetCurMapId;
            if (sceneName != null)
            {
                m_isInitTerrain = false;
                AsyncOperation ao = Application.LoadLevelAdditiveAsync(sceneName);
                AsyncOperation ao2 = Application.LoadLevelAdditiveAsync(mapId.ToString() + "DataScene");
                var terrain = GameObject.FindGameObjectWithTag("terrain");
                if (terrain != null)
                {
                    terrain.layer = 0;
                }
                //Application.LoadLevelAdditiveAsync("StroyLineUI");
            }
        }

        void OnDestroy()
        {
            if(null != StroyLineConfigManager.Instance)
            {
                StroyLineConfigManager.Instance.RestroeAllList();
            }
        }
    }
}


