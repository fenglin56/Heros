using UnityEngine;
using System.Collections;

public class NetServiceManager : Controller
{
    public LoginService LoginService { get; private set; }
    public CommonService CommonService { get; private set; }
    public EntityService EntityService { get; private set; }
    public BattleService BattleService { get; private set; }
    public EctypeService EctypeService { get; private set; }
    public InteractService InteractService { get; private set; }
    public ContainerService ContainerService { get; private set; }
    public TeamService TeamService { get; private set; }
    public FriendService FriendService { get; private set; }
    public EquipStrengthenService EquipStrengthenService { get; private set; }
    public TradeService TradeService { get; private set; }
    public ChatService ChatService { get; private set; }
    public EmailService EmailService {get; private set; }

    public NetServiceManager()
    {
        //初始化Service
        this.LoginService = new LoginService();
        this.CommonService = new CommonService();
        this.EntityService = new EntityService();
        this.BattleService = new BattleService();
        this.EctypeService = new EctypeService();
        this.ContainerService = new ContainerService();
        this.InteractService = new InteractService();
        this.TeamService = new TeamService();
        this.FriendService = new FriendService();
        this.EquipStrengthenService = new EquipStrengthenService();
        this.TradeService = new TradeService();
        this.ChatService = new ChatService();
        this.EmailService = new EmailService();
    }
    private static NetServiceManager m_instance;
    public static NetServiceManager Instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new NetServiceManager();
                }
                return m_instance;
            }
        }
    public void RegisterService()
    {
        //CommonTrace.Log("BoxService Register");
        //初始化网络配置信息
        //IpManager.InitServiceConfig();

        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_LOGIN, this.LoginService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_SELECTACTOR, this.LoginService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_THING, this.EntityService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_FIGHT, this.BattleService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_ECTYPE, this.EctypeService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_CONTAINER, this.ContainerService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_INTERACT, this.InteractService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_TEAM, this.TeamService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_FRIEND, this.FriendService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_GOODSOPERATE, this.EquipStrengthenService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_TRADE,this.TradeService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_ERROR, this.CommonService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_CHAT, this.ChatService);
        ServiceManager.RegistService((byte)MasterMsgType.NET_ROOT_EMAIL, this.EmailService);
    }
    protected override void RegisterEventHandler()
    {
        
    }
}
