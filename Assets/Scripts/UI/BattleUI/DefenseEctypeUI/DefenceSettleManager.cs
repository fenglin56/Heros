using UnityEngine;
using System.Collections;
using UI.MainUI;

public class DefenceSettleManager : BaseUIPanel {

	public GameObject DefenceSettleBoxEff;
	public GameObject DefenceSettleInfo;
	public Animation PanelSharkAnim;
	public GameObject Prefab_DefenceSettleBox;
	public SingleButtonCallBack BackBtn;
	public UILabel EctypeName;

	public GameObject InfoPoint;
	public GameObject BoxPoint;

	const float TWEEN_POSITION_OFFSET=158;
	const float BOX_ANIMATION_DURATION=2f;
	private DefenceSettleInfoBehaviour m_defenceSettleInfoBehaviour;
	private int m_ectypeContainerId;
	void Awake()
	{
		//返回按钮点击
		BackBtn.SetCallBackFuntion((obj)=>
		                           {
			SoundManager.Instance.PlaySoundEffect("Sound_Button_DefenceLevelBack");                                       
            NetServiceManager.Instance.EctypeService.SendEctypeRequestReturnCity(PlayerManager.Instance.FindHeroDataModel().UID);
		});
		//返回按钮按下/松开效果
		BackBtn.SetPressCallBack((isPressed)=>
		                         {
			BackBtn.spriteSwithList.ApplyAllItem(P=>P.ChangeSprite(isPressed?2:1));
		});
	}

	public void Init(int ectypeContainerId)
	{
		m_ectypeContainerId=ectypeContainerId;
		EctypeName.name=LanguageTextManager.GetString(EctypeConfigManager.Instance.EctypeContainerConfigList[ectypeContainerId].lEctypeName);
		m_defenceSettleInfoBehaviour=NGUITools.AddChild(InfoPoint,DefenceSettleInfo).GetComponent<DefenceSettleInfoBehaviour>();
		m_defenceSettleInfoBehaviour.InitSliders(ectypeContainerId);
		//评分结算完成，播放宝箱动画及震屏
		m_defenceSettleInfoBehaviour.SettleInfoComplete=()=>
		{
			StartCoroutine(ShowBoxAnim());
		};
	}
	[ContextMenu("Sharker")]
	private void Test()
	{
		//StartCoroutine(ShowBoxAnim());
        Init(33030);
	}
	private IEnumerator ShowBoxAnim()
	{
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceResult_3");
		NGUITools.AddChild(BoxPoint,DefenceSettleBoxEff);
		PanelSharkAnim.Play();

		yield return new  WaitForSeconds(BOX_ANIMATION_DURATION);
		//两个宝箱左右展开
        SoundManager.Instance.PlaySoundEffect("Sound_UIEff_DefenceResult_4");
		Debug.Log("ShowBoxAnim Over");
		var leftBox=NGUITools.AddChild(BoxPoint,Prefab_DefenceSettleBox);
		leftBox.GetComponent<DefenceSettleBoxBehaviour>().Init(true,m_ectypeContainerId);
		var leftPosAnim=leftBox.GetComponent<TweenPosition>();
		leftPosAnim.to=new Vector3 (TWEEN_POSITION_OFFSET*-1,0,0);
		leftPosAnim.Play(true);

		var rightBox=NGUITools.AddChild(BoxPoint,Prefab_DefenceSettleBox);
		rightBox.GetComponent<DefenceSettleBoxBehaviour>().Init(false,m_ectypeContainerId);
		var rightPosAnim=rightBox.GetComponent<TweenPosition>();
		rightPosAnim.to=new Vector3 (TWEEN_POSITION_OFFSET,0,0);
		rightPosAnim.Play(true);
	}
	public override void Show(params object[] value)
	{
		base.Show(value);		
	}
	
	public override void Close()
	{
		if (!IsShow)
			return;
		StartCoroutine(AnimToClose());
	}
	/// <summary>
	/// 播放关闭动画
	/// </summary>
	/// <returns>The to close.</returns>
	private IEnumerator AnimToClose()
	{
		yield return new WaitForSeconds(0.16f);   //动画时长
		UIEventManager.Instance.TriggerUIEvent(UIEventType.CloseMainUI, this.gameObject);
		base.Close();
	}
}
