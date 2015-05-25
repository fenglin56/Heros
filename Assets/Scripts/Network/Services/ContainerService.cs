using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.InteropServices;

public class ContainerService : Service
{

    //static ContainerService m_Instance;
    //public ContainerService()
    //{
    //    m_Instance = this;
    //}

    //public static ContainerService Instance { get { return m_Instance; } }

    public override void SaveResponseHandleToInvoke(byte[] dataBuffer, int socketId, byte cmdtype)
    {
         Package package;
        MasterMsgType masterMsgType = (MasterMsgType)cmdtype;
        package = PackageHelper.ParseReceiveData(dataBuffer);
        //TraceUtil.Log("EntityService 收到主消息:" + masterMsgType + "  收到子消息：" + package.GetSubMsgType());
        switch (masterMsgType)
        {
            case MasterMsgType.NET_ROOT_CONTAINER:
                switch (package.GetSubMsgType())
                {
                    case ContainerDefineManager.MSG_CONTAINER_CREATE:  //S发送创建客户端物品篮到C 
                        //Debug.LogWarning("收到创建物品栏消息");
                        this.AddToInvoker(CreateContainerHandle, package.Data, socketId);
                        break;
                    case ContainerDefineManager.MSG_CONTAINER_SYNC:  //S发送背包栏物品位置同步到C
                        //Debug.LogWarning("收到物品位置同步消息");
                        this.AddToInvoker(ContainerSyncHandle, dataBuffer, socketId);
                        break;
                    case ContainerDefineManager.MSG_CONTAINER_CHANGESIZE://s发送修改背包容量
                        //Debug.LogWarning("收到背包容量修改消息");
                        this.AddToInvoker(ContainerChangeSize, package.Data, socketId);
                        break;
					case ContainerDefineManager.MSG_CONTAINER_DOFF://物品出售回复
				this.AddToInvoker(ReceiveContainerDoffMsg,package.Data,socketId);
						break;
                    default:
                        //TraceUtil.Log("NET_ROOT_CONTAINER:" + package.GetSubMsgType() + "  " + dataBuffer.Length);
                        break;
                }
                break;
            //case MasterMsgType.NET_ROOT_THING:
            //    switch (package.GetSubMsgType())
            //    {
            //        case ContainerDefineManager.MSG_ACTION_USEMEDICAMENTRESULT:
            //            break;
            //        case ContainerDefineManager.MSG_CONTAINER_USE:
            //            break;
            //        default:
            //            //TraceUtil.Log("NET_ROOT_CONTAINER:" + package.GetSubMsgType());
            //            break;
            //    }
            //    break;
            default:
                //TraceUtil.Log("不能识别的主消息:" + package.GetMasterMsgType());
                break;
        }
    }

	CommandCallbackType ReceiveContainerDoffMsg(byte[] dataBuffer,int socketId)
	{
		SMsgContainerDoff_SC ContainerDoffMsg = SMsgContainerDoff_SC.ParsePackage(dataBuffer);
		ContainerDoffMsg.GetItemList.ApplyAllItem(C=>UI.GoodsMessageManager.Instance.Show(C.Key,C.Value));
		return CommandCallbackType.Continue;
	}

    /// <summary>
    /// S发送创建客户端物品篮到C
    /// </summary>
    /// <param name="dataBuffer"></param>
    CommandCallbackType CreateContainerHandle(byte[] dataBuffer, int socketId)
    {
        var sBuildContainerClientContext = SBuildContainerClientContext.ParsePackage(dataBuffer);
        UI.MainUI.ContainerInfomanager.Instance.CreatContainerClientContext(sBuildContainerClientContext);
        TraceUtil.Log("收到创建物品篮消息：" + sBuildContainerClientContext.dwContainerName);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// S发送背包栏物品位置同步到C
    /// </summary>
    /// <param name="dataBuffer"></param>
    CommandCallbackType ContainerSyncHandle(byte[] dataBuffer, int socketId)
    {
        Package package = PackageHelper.ParseReceiveData(dataBuffer);

        SMsgContainerCSCHead sMsgContainerCSCHead = SMsgContainerCSCHead.ParsePackage(package);

        List<SSyncContainerGoods_SC> sSyncContainerGoods_SCs = new List<SSyncContainerGoods_SC>();
        int headerLength = Marshal.SizeOf(sMsgContainerCSCHead);
        int singleLength = Marshal.SizeOf(typeof(SSyncContainerGoods_SC)) - headerLength;
        int offset = headerLength;
        //TraceUtil.Log("headerLength:" + headerLength + "  singleLength:" + singleLength);
        //TraceUtil.Log("BufferLength:" + dataBuffer.Length + "  dwSysGoodsNum:" + sMsgContainerCSCHead.dwSysGoodsNum);
        for (int i = 0; i < sMsgContainerCSCHead.dwSysGoodsNum; i++)
        {
            sSyncContainerGoods_SCs.Add(SSyncContainerGoods_SC.ParsePackage(package.Data, offset, sMsgContainerCSCHead));
            offset += singleLength;
        }
        //TraceUtil.Log("收到背包栏物品位置同步消息：" + sSyncContainerGoods_SCs.Count + " 约定个数:" + sMsgContainerCSCHead.dwSysGoodsNum);
        //sSyncContainerGoods_SCs.ApplyAllItem(P=>TraceUtil.Log(P.nPlace+","+P.byNum));
        UI.MainUI.ContainerInfomanager.Instance.SetContainerGoodsPosition(sSyncContainerGoods_SCs);
        //TraceUtil.Log("Set 完成");
        return CommandCallbackType.Continue;
    }

    CommandCallbackType ContainerChangeSize(byte[] dataBuffer, int socketID)
    {
        SMsgContainerChangeSize_SC smsgContainerChangeSize = SMsgContainerChangeSize_SC.ParsePackage(dataBuffer);
        //UI.MainUI.ContainerListBoardManager.Instance.UnlockMyPack();
        TraceUtil.Log("收到修改背包容量消息,背包容量：" + smsgContainerChangeSize.vMaxSize);
        UI.MainUI.ContainerInfomanager.Instance.UnlockPackBox(smsgContainerChangeSize);
        return CommandCallbackType.Continue;
    }
    /// <summary>
    /// 发送位置同步到服务器
    /// </summary>
    public void SendSSyncContainerGoods(SSyncContainerGoods_CS sSyncContainerGoods_CS)
    {
        this.Request(sSyncContainerGoods_CS.GeneratePackage());
    }

    /// <summary>
    /// 发送使用物品请求
    /// </summary>
    public void SendContainerUse(SMsgContainerUse_CS dataStruct)
    {
        if(dataStruct.byGoodsNum==0)
        {
            dataStruct.byGoodsNum=1;
        }

        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER, ContainerDefineManager.MSG_CONTAINER_USE);
        this.Request(pkg);
    }
    /// <summary>
    /// 发送背包整理请求
    /// </summary>
    public void SendContainerTidy(SMsgContainerTidy_CS dataStruct)
    {

        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER, ContainerDefineManager.MSG_CONTAINER_TIDY);

        this.Request(pkg);
    }
    /// <summary>
    /// 发送观察背包请求
    /// </summary>
    public void SendContainerObserver(SMsgContainerObserver_CS dataStruct)
    {

        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER, ContainerDefineManager.MSG_CONTAINER_OBSERVER);

        this.Request(pkg);
    }
    /// <summary>
    /// 发送丢弃物品请求
    /// </summary>
    public void SendContainerDoff(SMsgContainerDoff_CS dataStruct)
    {

        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER, ContainerDefineManager.MSG_CONTAINER_DOFF);

        this.Request(pkg);
    }
    /// <summary>
    /// 发送拾取物品到背包请求
    /// </summary>
    public void SendContainerDoodsAdd(SMsgContainerAdd_CS dataStruct)
    {

        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER, ContainerDefineManager.MSG_CONTAINER_GOODS_ADD);

        this.Request(pkg);
    }
    /// <summary>
    /// 发送修改背包容量
    /// </summary>
    public void SendContainerChangeSize(long dwSrcContainerID)
    {
        SmsgContainerChangeSize dataStruct = new SmsgContainerChangeSize { dwSrcContainerID = dwSrcContainerID };
        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER,ContainerDefineManager.MSG_CONTAINER_CHANGESIZE);
        this.Request(pkg);
    }

	/// <summary>
	/// 发送物品已经浏览过的消息
	/// </summary>
	/// <param name="dwContainerID">Dw container I.</param>
	/// <param name="dwPlace">Dw place.</param>
	public void SendUpdateContainerGoodsNewStatu(int dwContainerID, byte dwPlace)
	{
		SMsgContainerGoodsNew_CS sMsgContainerGoodsNew_CS = new SMsgContainerGoodsNew_CS();
		sMsgContainerGoodsNew_CS.dwContainerID_Heard = dwContainerID;
		sMsgContainerGoodsNew_CS.dwContainerID = dwContainerID;
		sMsgContainerGoodsNew_CS.dwPlace = dwPlace;
		this.Request(sMsgContainerGoodsNew_CS.GeneratePackage());
	}

    /// <summary>
    /// 发送红蓝药连接请求
    /// </summary>
//    public void SendContainerMedicamentLink(SMsgContainerMedicamentLink_CS dataStruct)
//    {
//        Package pkg = dataStruct.GeneratePackage(MasterMsgType.NET_ROOT_CONTAINER,ContainerDefineManager.MSG_CONTAINER_GOODS_LINK);
//        this.Request(pkg);
//    }
    
}
