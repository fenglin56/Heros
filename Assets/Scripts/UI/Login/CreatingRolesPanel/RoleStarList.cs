using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UI.Login
{

    public class RoleStarList : MonoBehaviour
    {

        public SpriteSwith[] StartList;


        public void Init(int startNumber)
        {
            for (int i = 0; i < StartList.Length; i++)
            {
                StartList[i].ChangeSprite(i<startNumber?2:1);
            }
        }

        [ContextMenu("InitStartObj")]
        void InitStartObj()
        {
            StartList = transform.GetComponentsInChildren<SpriteSwith>();
        }

    }
}