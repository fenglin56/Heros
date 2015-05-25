using UnityEngine;
using System.Collections;
namespace UI.Battle
{
	public class BattleSkillTip : View {
		//技能提示
		public GameObject skillTipPrefab;
		// Use this for initialization
		void Awake () {
			RegisterEventHandler();
		}
		
		protected override void RegisterEventHandler()
		{
			AddEventHandler(EventTypeEnum.FightCommand.ToString(), ReceiveFightCommand);
			AddEventHandler(EventTypeEnum.SingleFigntCommand.ToString(), ReceiveFightCommondHandle);
		}
		void ReceiveFightCommondHandle(INotifyArgs inotifyArgs)
		{
			SMsgFightCommand_SC serData = (SMsgFightCommand_SC)inotifyArgs;
			SkillFight (serData.nFightCode);
		}
		void ReceiveFightCommand(INotifyArgs inotifyArgs)
		{
			SMsgBattleCommand serData = (SMsgBattleCommand)inotifyArgs;
			SkillFight (serData.nFightCode);
		}
		void SkillFight(int skillID)
		{
			SkillConfigData configData = SkillDataManager.Instance.GetSkillConfigData (skillID);
			if (configData.skillText.Equals ("0")) {
				return;
			}
			StartCoroutine (SkillTipFun(configData));		
		}
		IEnumerator SkillTipFun(SkillConfigData configData)
		{
			yield return new WaitForSeconds(configData.HintTextList [2]/1000f);
			GameObject tip = NGUITools.AddChild (gameObject,skillTipPrefab);//(GameObject)Instantiate(skillEffTipPrefab);//
			Transform skillTipBg = tip.transform.Find("Bg");
			UILabel tipLabel = tip.transform.Find("Tip").GetComponent<UILabel>();
			//tip.transform.parent = skillTip.transform;
			tip.transform.localPosition = new Vector3 (configData.HintTextList[0],-1*configData.HintTextList[1],1);
			tipLabel.text = LanguageTextManager.GetString(configData.skillText);
			yield return new WaitForSeconds(configData.HintTextList [3]/1000f);
			Destroy (tip);
		}
		void OnDestroy()
		{
			//RemoveEventHandler(EventTypeEnum.ColdWork.ToString(), ColdSkillButton);
			RemoveEventHandler(EventTypeEnum.FightCommand.ToString(), ReceiveFightCommand);
			RemoveEventHandler(EventTypeEnum.SingleFigntCommand.ToString(), ReceiveFightCommondHandle);
		}
	}
}
