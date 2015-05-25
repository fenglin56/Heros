using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class RobotInfo{
	public GameObject robot;
	public GameObject titleRef;
}
public class TownRobotManager : MonoBehaviour
{
	private static TownRobotManager m_instance;
	public static TownRobotManager Instance { get { return m_instance; } }

	//机器人
	public TownRobotPosDataBase TownRobotPosDataBase;
	public RobotConfigDataBase RobotConfigDataBase;

	//公告触发
	public BroadcastConfigDataBase BroadcastConfigDataBase;

	private List<GameObject> m_robotList = new List<GameObject>();
	private Dictionary<int, BroadcastConfigData> m_BroadcastConfigDict = new Dictionary<int, BroadcastConfigData>();

	//自动喊话
	private PhpAutoPropaganda m_AutoPropaganda;

	void Awake()
	{
		m_instance = this;
	}
	void OnDestroy()
	{
		UIEventManager.Instance.RemoveUIEventHandel(UIEventType.WorldChatMsg, ShowWorldChatHandle);
		m_instance = null;
	}

	void Start()
	{
		InitBroadcastCongfigs();
		CreateRobots();
		InitAutoPropagandaData();
		UIEventManager.Instance.RegisterUIEvent(UIEventType.WorldChatMsg, ShowWorldChatHandle);
	}

//	void Update()
//	{
//		if(Input.GetKeyDown( KeyCode.Space))
//		{
//			//GetListFormPhp();
//			InvokeRepeating("Propaganda",5f,5f);
//		}
//	}

	private void InitBroadcastCongfigs()
	{
		BroadcastConfigDataBase._dataTable.ApplyAllItem(p=>{
			m_BroadcastConfigDict.Add(p.BroadcastId,p);
		});
	}

	private void InitAutoPropagandaData()
	{
		StartCoroutine(NetServiceManager.Instance.ChatService.RequestPropagandaList(InvokePropaganda));
	}


	private List<Renderer> m_playerRendererDatas;
	public List<Renderer> PlayerRendererDatas{
		get{
			return m_playerRendererDatas;
		}
	}
	public List<RobotInfo> robotInfoList = new List<RobotInfo>();
	public void GetRobotRender(GameObject robot)
	{
		List<PlayDataStruct<MeshRenderer>> partRenderer;
		List<PlayDataStruct<SkinnedMeshRenderer>> mainRenderer;
		List<PlayDataStruct<ParticleSystemRenderer>> particleRender;
		robot.transform.RecursiveGetComponent<MeshRenderer>("MeshRenderer", out partRenderer);
		robot.transform.RecursiveGetComponent<SkinnedMeshRenderer>("SkinnedMeshRenderer", out mainRenderer);
		robot.transform.RecursiveGetComponent<ParticleSystemRenderer>("ParticleSystemRenderer",out particleRender);
		//PlayerModel = transform.GetComponentInChildren<SkinnedMeshRenderer>();
		
		if (this.m_playerRendererDatas == null)
		{
			this.m_playerRendererDatas = new List<Renderer>();
		}
		this.m_playerRendererDatas.Clear();
		
		this.m_playerRendererDatas.AddRange(partRenderer.Select(P => (Renderer)P.AnimComponent));
		this.m_playerRendererDatas.AddRange(mainRenderer.Select(P =>(Renderer)P.AnimComponent));
		this.m_playerRendererDatas.AddRange(particleRender.Select(P =>(Renderer)P.AnimComponent));
	}
	private void CreateRobots()
	{
		List<RobotConfigData> list = new List<RobotConfigData>();
		list.AddRange(RobotConfigDataBase._dataTable);

		int num = CommonDefineManager.Instance.CommonDefine.TownRobotNum;
		for(int i = 0;i<num;i++)
		{
			int index = Random.Range(0,list.Count);
			var config = list[index];

			var configData = PlayerDataManager.Instance.GetTownItemData((byte)config.RobotOccupation);
			var fashionItem=ItemDataManager.Instance.GetItemData(config.RobotFashion);
			var robot = PlayerFactory.Instance.AssemblyRobot(configData,(byte)config.RobotOccupation, fashionItem == null?null:fashionItem._ModelId);

			var posConfig = TownRobotPosDataBase._dataTable.SingleOrDefault(p=>p.PosId == config.RobotId);
			robot.transform.position = posConfig.BornPos;
			robot.transform.rotation = Quaternion.Euler(0,posConfig.BornOrientation,0);

			string[] ItemWeaponPosition = configData.Item_WeaponPosition;
			var equipmentData = (EquipmentData)ItemDataManager.Instance.GetItemData(config.RobotWeapon);
			var weaponObj = PlayerFactory.Instance.GetWeaponPrefab(equipmentData._ModelId);
			RoleGenerate.ChangeWeapon(robot, weaponObj, equipmentData.WeaponEff);

			var shadowEff=GameObject.Instantiate(configData.ShadowEffect) as GameObject;
			shadowEff.name = "shadow";
			shadowEff.transform.parent = robot.transform;
			shadowEff.transform.localPosition = new Vector3(0, 1, 0);
					
			robot.animation["TIdle01"].wrapMode = WrapMode.Loop;
			robot.animation.CrossFade("TIdle01");

			long robotUID = 100000000 + config.RobotId;
			var title = PlayerFactory.Instance.CreateTitle(robotUID, config.RobotName , false, robot.transform);
			var heroTitle = title.GetComponent<HeroTitle>();
			heroTitle.SetVipLevel(config.RobotVipLevel);

			PlayerFactory.Instance.SetDesignation(robotUID, heroTitle.TitlePoint, config.RobotTitle);

			m_robotList.Add(robot);
			RobotInfo info = new RobotInfo(){robot = robot,titleRef = title};
			robotInfoList.Add(info);
			list.RemoveAt(index);
		}
	}

	void ShowWorldChatHandle(object obj)
	{
		CancelInvoke("Propaganda");
		if(m_AutoPropaganda!=null)
		{
			InvokeRepeating("Propaganda",m_AutoPropaganda.time,m_AutoPropaganda.time);
		}
	}

	void InvokePropaganda(object obj)
	{
		CancelInvoke("Propaganda");
		m_AutoPropaganda = (PhpAutoPropaganda)obj;
		InvokeRepeating("Propaganda",m_AutoPropaganda.time,m_AutoPropaganda.time);
	}

	void Propaganda()
	{
		//模拟机器人喊话	
		SMsgChat_SC sMsgChat_SC = new SMsgChat_SC();
		sMsgChat_SC.SenderName = RandomString(m_AutoPropaganda.name);
		sMsgChat_SC.Chat = RandomString(m_AutoPropaganda.content);
		sMsgChat_SC.L_LabelChat = ChatRecordManager.Instance.ParseColorChat(Chat.WindowType.Town,sMsgChat_SC);
		sMsgChat_SC.L_Channel = (int)Chat.WindowType.Town;
		UIEventManager.Instance.TriggerUIEvent(UIEventType.WorldChatMsg, sMsgChat_SC);
		ChatRecordManager.Instance.AddPublicRecord( Chat.WindowType.Town, sMsgChat_SC);
	}

	private string RandomString(string[] strArray)
	{
		int length = strArray.Length;
		int index = Random.Range(0,length);
		return strArray[index];
	}

	/// <summary>
	/// 显示机器人
	/// </summary>
	/// <param name="isShow">If set to <c>true</c> is show.</param>
	public void ShowRobots(bool isShow)
	{
		m_robotList.ApplyAllItem(p=>{
			p.SetActive(isShow);
		});
	}

	/// <summary>
	/// 获取广播配置
	/// </summary>
	/// <returns>The broadcast config.</returns>
	/// <param name="broadcastID">广播id</param>
	public BroadcastConfigData GetBroadcastConfig(int broadcastID)
	{
		BroadcastConfigData data = null;
		m_BroadcastConfigDict.TryGetValue(broadcastID,out data);
		return data;
	}

}
