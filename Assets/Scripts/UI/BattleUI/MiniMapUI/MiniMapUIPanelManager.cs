using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    public class MiniMapUIPanelManager : MonoBehaviour
    {


        public MiniMapArea miniMapArea;

        private SceneConfigData SceneConfigData;
        int MapID;

        void Start()
        {
            Show();
        }

        public void Show()
        {
            MapID = (int)GameManager.Instance.GetCurSceneMapID;
            SceneConfigData sceneConfigData = EctypeConfigManager.Instance.SceneConfigList[MapID];
            miniMapArea.SetMapAreaSize(sceneConfigData);
            miniMapArea.ShowMiniMapUIPoint();
        }


    }
}