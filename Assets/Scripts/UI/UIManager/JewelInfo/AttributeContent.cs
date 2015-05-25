using UnityEngine;
using System.Collections;


namespace UI.MainUI{
public class AttributeContent : MonoBehaviour {
	
	public Transform IconPoint;
	public UILabel ContentLable;

	public void Init(PassiveSkillData skilldata)
	{
			CreatObjectToNGUI.InstantiateObj (skilldata.SkillIconPrefab, IconPoint);
			ContentLable.SetText (LanguageTextManager.GetString(skilldata.SkillDis));
	}
	
}
}
