using UnityEngine;
using System.Collections;

namespace UI.Battle
{
    public class MinimapPointBlick : MiniMapPointSetting
    {
        public GameObject BlikObj;

        public void BeginBlik()
        {
            MiniMapUIFlag.Begin(BlikObj,new Vector3(15,15,1),new Vector3(60,60,1));
        }

        public void StopBlik()
        {
            MiniMapUIFlag.StopBlik(BlikObj);
        }
    }
}