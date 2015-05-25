using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    public class MiniMapPointSetting : MonoBehaviour
    {

        private GameObject ShowObj;

        private Vector3 m_Position;
        private SceneConfigData sceneConfigData;

        float PositionX;
        float PositoinY;
        float PositionScale;

        bool Show = true;
        bool MoveBack = false;

        public void SetPosition(GameObject Obj, SceneConfigData sceneConfigData)
        {
            this.sceneConfigData = sceneConfigData;
            this.ShowObj = Obj;
            float mapWidth = sceneConfigData._mapWidth;
            float mapHeight = sceneConfigData._mapHeight;
            float MaxFloat = mapWidth > mapHeight ? mapWidth : mapHeight;
            PositionScale = 100 / MaxFloat;
            
        }

        void Update()
        {
            if (ShowObj == null)
            {
                Destroy(gameObject);
            }
            else
            {
                if (!ShowObj.activeSelf)
                {
                    if (Show)
                    {
                        transform.localPosition = new Vector3(0, 0, 1000);
                        Show = false;
                    }
                    return;
                }
                else
                {
                    Show = true;
                }
                float PositionX = PositionScale * (ShowObj.transform.localPosition.x - sceneConfigData._mapWidth / 2);
                float PositionY = PositionScale * (ShowObj.transform.localPosition.z + sceneConfigData._mapHeight / 2);
                transform.localPosition = new Vector3(PositionX, PositionY, 0);
            }
        }


    }
}