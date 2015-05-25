//using UnityEngine;
//using System.Collections;
//using System;

//namespace UI.Battle
//{

//    public class BattleButtonManager : MonoBehaviour
//    {


//        public SkillButton[] CreatSkillButtonPoint = new SkillButton[4];
//        public HealthAndMagicButton[] CreatPackegeButtonPoint = new HealthAndMagicButton[2];

//        //void OnGUI()
//        //{
//        //    if (GUILayout.Button("AddSkillBtn"))
//        //    {
//        //        AddButton(BattleButtonType.Skill, new SkillButtonInfo(OnButtonCallBack, BattleButtonPosition.SkillButton2));
//        //    }
//        //    if (GUILayout.Button("DeleteSkillBtn"))
//        //    {
//        //        DeleteButton(BattleButtonPosition.SkillButton2);
//        //    }
//        //    if (GUILayout.Button("AddMagicBtn"))
//        //    {
//        //        AddButton(BattleButtonType.Packege,new PackegeButtonInfo(BtnCallBack,BattleButtonPosition.MagicButton));
//        //    }
//        //    if (GUILayout.Button("DeleteMagicBtn"))
//        //    {
//        //        DeleteButton(BattleButtonPosition.MagicButton);
//        //    }
//        //}
//        //void BtnCallBack()
//        //{
//        //    //TraceUtil.Log("CallBack");
//        //}
//        //void OnButtonCallBack(SkillButtonStatus buttonStatus)
//        //{
//        //    //TraceUtil.Log(buttonStatus);
//        //}


//        private static BattleButtonManager m_instance = null;
//        public static BattleButtonManager Instance
//        {
//            get {                
//                return m_instance; }
//        }

//        void Awake()
//        {
//            m_instance = this;
//        }


//        public void AddButtonInputDelegate(BattleButtonType buttonType, IBattleButton buttonInfo)//入口，增加按钮
//        {
//            switch (buttonType)
//            {
//                case BattleButtonType.Skill:
//                    foreach (SkillButton child in CreatSkillButtonPoint)
//                    {
//                        child.SetButtonAttribute((SkillButtonInfo)buttonInfo);
//                    }
//                    break;
//                case BattleButtonType.Packege:
//                    foreach(HealthAndMagicButton child in CreatPackegeButtonPoint)
//                    {
//                        child.SetButtonActive((PackegeButtonInfo)buttonInfo);
//                    }
//                    break;
//                default:
//                    break;
//            }
//        }

//        public void DeleteButton(BattleButtonPosition buttonPosition)//删除按钮
//        {
//            foreach (SkillButton child in CreatSkillButtonPoint)
//            {
//                child.ClearButton(buttonPosition);
//            }
//            foreach (HealthAndMagicButton child in CreatPackegeButtonPoint)
//            {
//                child.ClearButton(buttonPosition);
//            }
//        }

//    }

//    public enum BattleButtonType { Skill, Packege }
//    public delegate void SkillButtonCallBack(SkillButtonStatus ButtonStatus, SkillButtonInfo skillButtonInfo);

//    public interface IBattleButton
//    {
//    }

//    public class SkillButtonInfo : IBattleButton
//    {
//        public BattleButtonPosition ButtonPosition { get; set; }
//        public SkillButtonStatus ButtonStatus { get; set; }
//        public int SkillID { get; private set; }
//        public string SkillLable { get; private set; }
//        public float RecoverTime { get; private set; }
//        public SkillButtonCallBack buttonCallBack { get; private set; }

//        public SkillButtonInfo()
//        {
//        }
//        public SkillButtonInfo(SkillButtonCallBack buttonCallBack, BattleButtonPosition ButtonPosition)
//        {
//            this.ButtonPosition = ButtonPosition;
//            this.ButtonStatus = SkillButtonStatus.Enable;
//            this.SkillID = 1;
//            this.SkillLable = "Level5";
//            this.RecoverTime = 5;
//            this.buttonCallBack = buttonCallBack;
//        }

//        public void Copy(ref SkillButtonInfo skillButtonInfo)
//        {
//            skillButtonInfo.buttonCallBack = this.buttonCallBack;
//            skillButtonInfo.ButtonStatus = this.ButtonStatus;
//            skillButtonInfo.SkillID = this.SkillID;
//            skillButtonInfo.RecoverTime = this.RecoverTime;
//        }

//    }

//    public class PackegeButtonInfo : IBattleButton
//    {
//        public BattleButtonPosition ButtonPosition { get; private set; }
//        public int MagicBottleNumber { get; private set; }
//        public int MagicBottleType { get; private set; }
//        public int HealthBottleNumber { get; private set; }
//        public int HealthBottleType { get; private set; }
//        public ButtonCallBack buttonCallBack { get; private set; }

//        public PackegeButtonInfo(ButtonCallBack buttonCallBack,BattleButtonPosition buttonPosition)
//        {
//            this.ButtonPosition = buttonPosition;
//            this.MagicBottleNumber = 65;
//            this.MagicBottleType = 1;
//            this.HealthBottleNumber = 77;
//            this.HealthBottleType = 0;
//            this.buttonCallBack = buttonCallBack;
//        }
//    }

//}