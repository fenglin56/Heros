using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace NetworkCommon
{
    public class IpManager
    {
        public const int TcpPort=8000;
        public const int UdpPort = 6616;
        public const int ListenSocketId = -100;
        public static IPAddress GetIpAddress()
        {
            IPAddress ipAddress = null;
            var ipaddr = Network.player.ipAddress;
            IPAddress[] addressList = Dns.GetHostEntry(Environment.MachineName).AddressList;
            // Get endpoint for the listener.
            for (int i = 0; i < addressList.Length; i++)
            {
                if (addressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ipAddress = addressList[i];
                    break;
                }
            }

            return ipAddress;
        }
        public static string GetMobleIpAddress()
        {
            var ipaddr = Network.player.ipAddress;
            return ipaddr;
        }
        public static void InitServiceConfig()
        {
            InitServiceConfig(null);
        }
        public static void InitServiceConfig(string serverIp)
        {
            InitServiceConfig(serverIp,TcpPort);
        }
        public static void InitServiceConfig(string serverIp,int port)
        {
            if (string.IsNullOrEmpty(serverIp))
            {
                serverIp = GetMobleIpAddress();
            }
            IpInfo tcpInfo = new IpInfo();
            tcpInfo.baseType = BASESOCKET_TYPE.SOCKET_TCP;
            tcpInfo.ip = serverIp;

            tcpInfo.port = port;
            ServiceManager.SetConfig(SocketInfoType.TCPServer, tcpInfo);

            IpInfo udpInfo = new IpInfo();
            udpInfo.baseType = BASESOCKET_TYPE.SOCKET_UDP;
            udpInfo.ip = serverIp;

            udpInfo.port = UdpPort;
            ServiceManager.SetConfig(SocketInfoType.UDPServer, udpInfo);
        }
        public static string Domain2Ip(string str)
        {
            string _return = "";
            try
            {
                IPHostEntry hostinfo = Dns.GetHostEntry(str);
                IPAddress[] aryIP = hostinfo.AddressList;
                _return = aryIP[0].ToString();

            }
            catch (Exception e)
            {
                _return = e.Message;
            }
            return _return;
        }
    }
}
