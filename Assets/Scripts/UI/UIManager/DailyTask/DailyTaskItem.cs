using UnityEngine;
using System.Collections;
namespace UI.MainUI
{
    public class DailyTaskItem : MonoBehaviour
    {
        public int TaskID { get; set; }
        public SpriteSwith Switch_;
        public LocalButtonCallBack Button_Goto;
        public UILabel Label_TaskDescription;
        public UILabel Label_Active;
        public UILabel Label_ActiveValue;
        public UILabel Label_progress;

        private DailyTaskConfigData m_DailyTaskConfig;
		public int CanGetActiveValue
		{
			get
			{
				if(m_DailyTaskConfig!=null)
				{
					return m_DailyTaskConfig._activeValue;
				}
				return 0;
			}
		}
        public int Status { get; set; }

        public void Init(DailyTaskConfigData configData, STaskLogUpdate sTaskLogUpdate, STaskLogContext sTaskLogContext)
        {
            this.TaskID = configData._taskID;
            this.m_DailyTaskConfig = configData;

            Label_TaskDescription.text = LanguageTextManager.GetString(m_DailyTaskConfig._taskDescription);//任务描述
            Label_ActiveValue.text = "+" + m_DailyTaskConfig._activeValue;

            UpdateView(sTaskLogUpdate, sTaskLogContext);
        }

        public void UpdateView(STaskLogUpdate sTaskLogUpdate, STaskLogContext sTaskLogContext)
        {
            Status = sTaskLogUpdate.nStatus;//更新状态

            //更新进度
            Label_progress.text = sTaskLogContext.nParam3.ToString() + "/" + m_DailyTaskConfig._conditionParameter.ToString();

            switch (Status)
            {
                case 1:
                    Switch_.ChangeSprite(1);
                    Button_Goto.SetBoxCollider(true);
					Label_TaskDescription.color = Color.white;
					Label_progress.color = Color.white;
					Label_Active.color = new Color(0.992f, 0.46f, 0.543f);
					Label_ActiveValue.color = new Color(0.992f, 0.46f, 0.543f);
                    break;
                case 2:
                    Switch_.ChangeSprite(2);
                    Button_Goto.SetBoxCollider(false);
                    Label_TaskDescription.color = new Color(0.523f, 0.5f, 0.5f);
					Label_progress.color = new Color(0.523f, 0.5f, 0.5f);
                    Label_Active.color = new Color(0.922f, 0.875f, 0.617f);
					Label_ActiveValue.color = new Color(0.922f, 0.875f, 0.617f);
                    break;
            }
        }

        void Awake()
        {
            Button_Goto.SetCallBackFuntion(OnClickHandle, null);
        }

        void OnClickHandle(object obj)
        {
            SoundManager.Instance.PlaySoundEffect("Sound_Button_Default");
            switch (m_DailyTaskConfig._quickTripTo)
            {
                //case 1://武馆
                //    MainUIController.Instance.OpenMainUI(UIType.MartialArtsRoom);
                //    break;
                //case 2://试练
                //    MainUIController.Instance.OpenMainUI(UIType.TrialsEctypePanel);
                //    break;
                case 3://副本
                    MainUIController.Instance.OpenMainUI(UIType.Battle);
                    break;
                case 4://炼妖
                    MainUIController.Instance.OpenMainUI(UIType.Siren);
                    break;
                //case 5://锻造
                //    MainUIController.Instance.OpenMainUI(UIType.EquipStrengthen);
                    break;
                case 6://技能
                    MainUIController.Instance.OpenMainUI(UIType.Skill);
                    break;
                //case 7://经脉
                //    MainUIController.Instance.OpenMainUI(UIType.Meridians);
                    break;
                case 8://宝树
                    MainUIController.Instance.OpenMainUI(UIType.Treasure);
                    break;
                default:
                    break;
            }
        }
    }
}
