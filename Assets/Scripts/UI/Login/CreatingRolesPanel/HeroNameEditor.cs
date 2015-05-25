using UnityEngine;
using System.Collections;

namespace UI.Login
{
    public enum Sex {Man,Woman }
    public class HeroNameEditor : MonoBehaviour
    {

        public UIInput InPutLable;
        public SingleButtonCallBack RandomButton;
        public byte Vocation;
        private Sex heroSex = Sex.Man;

        void Start()
        {
            RandomButton.SetCallBackFuntion(RandomName);
        }

        public void SetHeroSex(byte vocation)
        {
            this.Vocation = vocation;
            switch (Vocation)
            {
                case 1:
                    this.heroSex = Sex.Man;
                    break;
                case 2:
                    this.heroSex = Sex.Man;
                    break;
                case 3:
                    this.heroSex = Sex.Woman;
                    break;
                case 4:
                    this.heroSex = Sex.Woman;
                    break;
                default:
                    break;
            }
        }

        public void ClearName()
        {
            InPutLable.text = "";
        }

        void RandomName(object obj)
        {
			SoundManager.Instance.PlaySoundEffect("Sound_UIEff_Random");
            string NameStr = "";
            CharacterData[] CharacterData= LoginDataManager.Instance.characterNameDataBase.CharacterDataList;
            int Leght = CharacterData.Length;
            int FamilyNameID = Random.Range(0, Leght);
            int NameID = Random.Range(0,Leght);
            switch (heroSex)
            {
                case Sex.Man:
                    NameStr = CharacterData[FamilyNameID].FamilyName + CharacterData[NameID].MaleName;
                    break;
                case Sex.Woman:
                    NameStr = CharacterData[FamilyNameID].FamilyName + CharacterData[NameID].FemaleName;
                    break;
                default:
                    break;
            }
            InPutLable.text = NameStr;
        }

    }
}