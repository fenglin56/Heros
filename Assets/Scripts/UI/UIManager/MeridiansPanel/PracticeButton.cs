using UnityEngine;
using System.Collections;


namespace UI.MainUI
{

    public class PracticeButton : MonoBehaviour
    {
        public GameObject EffectPrefab;

        public Transform CreatEffectPoint;

        private GameObject CreatEffectObj;
        private MeridiansPanel MyParent;

        public void InitMySelf(MeridiansPanel MyParent)
        {
            this.MyParent = MyParent;
        }

        public void SetEffectActive(bool Flag)
        {
            if (Flag && CreatEffectObj != null)
            {
                CreatEffectObj = CreatObjectToNGUI.InstantiateObj(EffectPrefab,CreatEffectPoint);
            }
            else if (CreatEffectObj != null)
            {
                Destroy(CreatEffectObj);
            }
        }

        void OnPress(bool isPressed)
        {
            if (!isPressed)
                return;
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            MyParent.AddMeridians(isPressed);
            //if (isPressed)
            //{
            //    SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Meridians",true);
            //}
            //else
            //{
            //    SoundManager.Instance.StopSoundEffect("Sound_UIEff_Meridians");
            //}
        }

        
    }
}