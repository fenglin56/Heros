using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UI;
using UI.Battle;
using System.Linq;

public class SirenDialogEctypeBehaviour : MonoBehaviour
{
    public Transform HeadPoint;
    public UILabel Label_Word;
    public UILabel Label_Name;

	private List<MonsterConfigData.BornDialogueFull> m_MonsConfigList = new List<MonsterConfigData.BornDialogueFull>();

	private GameObject m_HeadGameObj = null;
   
   	private GameObject m_DialogPrefab;

//    public void Init(GameObject headPrefab, string word , string name)
//    {
//        GameObject head = (GameObject)Instantiate(headPrefab);
//        head.transform.parent = HeadPoint;
//        head.transform.localPosition = Vector3.zero;
//        head.transform.localScale = headPrefab.transform.localScale;
//        Label_Word.text = word;
//        Label_Name.text = name;
//    }

//	public void Init(MonsterConfigData.BornDialogueFull[] dialogFullArray)
//	{
//		m_MonsConfigList.AddRange(dialogFullArray);
//		Show();
//		m_isCutDown = true;
//		StartCoroutine("LatePauseAnimation");
//	}
	public void Init(GameObject dialogPrefab, MonsterConfigData.BornDialogueFull[] dialogFullArray )
	{
		m_DialogPrefab = dialogPrefab;
		m_MonsConfigList.AddRange(dialogFullArray);

		Label_Name.text = LanguageTextManager.GetString(m_MonsConfigList[0].MonsterName);
		Label_Word.text = LanguageTextManager.GetString(m_MonsConfigList[0].Dialogue);
		Vector3 newPos = GetDialogBarPosition(m_MonsConfigList[0].BornDialoguePosition);
		transform.localPosition = new Vector3(newPos.x,newPos.y,-20);
		if(m_HeadGameObj!=null)Destroy(m_HeadGameObj);
        GameObject head = null;
        if(m_MonsConfigList[0].protraitType == 0)
        {
		    head = MapResManager.Instance.GetMapEffectPrefab(m_MonsConfigList[0].Portrait);	
        }
        else
        {
            var m_HeroDataModel = PlayerManager.Instance.FindHeroDataModel();
            
            int vocationID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_VOCATION;

            
            int fashionID = m_HeroDataModel.PlayerValues.PlayerCommonValue.PLAYER_FIELD_VISIBLE_FASHION;
            var resData= CommonDefineManager.Instance.CommonDefineFile._dataTable.HeroIcon_Dialogue.FirstOrDefault(P=>P.VocationID == vocationID&&P.FashionID == fashionID);
            head = resData.IconPrefab;
			Label_Name.text = m_HeroDataModel.Name;
        }


		if(head!=null)
		{
			m_HeadGameObj = (GameObject)Instantiate(head);
			m_HeadGameObj.transform.parent = HeadPoint;
			m_HeadGameObj.transform.localPosition = Vector3.zero;
			m_HeadGameObj.transform.localScale = head.transform.localScale;
		}
		float delayTime = m_MonsConfigList[0].Time;

		m_MonsConfigList.RemoveAt(0);

		if(m_MonsConfigList.Count>0)
		{
			StartCoroutine("LateShowNextDialog",delayTime);
		}
		else
		{
			StartCoroutine("PlaybackAndDestroy", delayTime);
//			Animator animator = gameObject.GetComponentInChildren<Animator>();
//			animator.StartPlayback();
//			DestroySelf ds = gameObject.AddComponent<DestroySelf>();
//			ds.Time = delayTime;
		}
	}

	IEnumerator PlaybackAndDestroy(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		Animator animator = gameObject.GetComponentInChildren<Animator>();

		animator.Play("Eff_UI_Siren_Dialogue_a_02");
		gameObject.AddComponent<DestroySelf>();
	}


	IEnumerator LateShowNextDialog(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);
		GameObject IconPrefab = CreatObjectToNGUI.InstantiateObj(m_DialogPrefab, BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.Center));
		SirenDialogEctypeBehaviour sirenDialogEctypeBehaviour = IconPrefab.GetComponent<SirenDialogEctypeBehaviour>();
		sirenDialogEctypeBehaviour.Init(m_DialogPrefab, m_MonsConfigList.ToArray());
		StartCoroutine("PlaybackAndDestroy", 0);
//		//销毁旧对话
//		StartCoroutine("PlaybackAndDestroy", delayTime);
//		Animator animator = gameObject.GetComponentInChildren<Animator>();
//		animator.StartPlayback();
//		DestroySelf ds = gameObject.AddComponent<DestroySelf>();
	}

//	IEnumerator LatePauseAnimation()
//	{
//		yield return new WaitForSeconds(1f);
//		var animations = GetComponentsInChildren<Animator>();
//		animations.ApplyAllItem(p=>p.StopPlayback());
//	}

//	void Update()
//	{
//		if(m_isCutDown)
//		{
//			m_cutDownTime -= Time.deltaTime;
//			Debug.Log(m_cutDownTime);
//			if(m_cutDownTime<=0)
//			{
//				Show();
//				//ShowNextDialog();
//			}
//		}
//
//	}

//	private void Show()
//	{
//		if(m_MonsConfigList.Count > 0)
//		{
//			m_cutDownTime = m_MonsConfigList[0].Time;
//			Label_Name.text = LanguageTextManager.GetString(m_MonsConfigList[0].MonsterName);
//			Label_Word.text = LanguageTextManager.GetString(m_MonsConfigList[0].Dialogue);
//			Vector3 newPos = GetDialogBarPosition(m_MonsConfigList[0].BornDialoguePosition);
//			transform.localPosition = new Vector3(newPos.x,newPos.y,10);
//			if(m_HeadGameObj!=null)Destroy(m_HeadGameObj);
//			GameObject head = MapResManager.Instance.GetMapEffectPrefab(m_MonsConfigList[0].Portrait);	
//			m_HeadGameObj = (GameObject)Instantiate(head);
//			m_HeadGameObj.transform.parent = HeadPoint;
//			m_HeadGameObj.transform.localPosition = Vector3.zero;
//			m_HeadGameObj.transform.localScale = head.transform.localScale;
//			m_MonsConfigList.RemoveAt(0);
//		}
//		else
//		{
//			m_isCutDown = false;
////			var animations = GetComponentsInChildren<Animation>();
////			animations.ApplyAllItem(p=>p.Play());
//			//Destroy(gameObject);
//			var animator = GetComponentInChildren<Animator>();
//			animator.Play("Eff_UI_Siren_Dialogue_a_01 0");
//			gameObject.AddComponent<DestroySelf>();
//		}
//	}

//	private void ShowNextDialog()
//	{
//		if(m_MonsConfigList.Count > 0)
//		{
//			GameObject IconPrefab = CreatObjectToNGUI.InstantiateObj(m_DialogPrefab, BattleUIManager.Instance.GetScreenTransform(ScreenPositionType.Center));
//			SirenDialogEctypeBehaviour sirenDialogEctypeBehaviour = IconPrefab.GetComponent<SirenDialogEctypeBehaviour>();
//		}
//	}


	private Vector2 GetDialogBarPosition(int no)
	{
		int index = no - 1;
		Vector2 v2 = CommonDefineManager.Instance.CommonDefine.BornDialoguePositionList[index];
		return v2;
	}

}
