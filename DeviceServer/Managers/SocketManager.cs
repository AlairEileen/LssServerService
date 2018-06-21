using DeviceServer.Models;
using MongoDB.Driver;
using NTTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebTools;

namespace DeviceServer.Managers
{
    /// <summary>
    /// 连接管理器
    /// </summary>
    public class SocketManager
    {
        /// <summary>
        /// 服务端IP
        /// </summary>
        private const string SERVER_IP = "0.0.0.0";

        /// <summary>
        /// 服务端端口
        /// </summary>
        private const int SERVER_PORT = 7701;

        /// <summary>
        /// 客户端连接器集合
        /// </summary>
        private List<SocketConnector> socketConnectors = new List<SocketConnector>();

        /// <summary>
        /// 服务对象
        /// </summary>
        private Socket server;

        /// <summary>
        /// 消息管理器
        /// </summary>
        private MessageManager messageManager;

        /// <summary>
        /// 构造函数，构造消息管理器，开启服务
        /// </summary>
        /// <param name="messageManager">消息管理器</param>
        public SocketManager(MessageManager messageManager)
        {
            this.messageManager = messageManager;
            StartServer();
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="messageModel">消息对象</param>
        public void Send(MessageModel messageModel)
        {
            try
            {
                var connector = socketConnectors.Find(x => x.Client.ClientID.Equals(messageModel.ClientID));
                if (connector == null || !connector.Client.Connected)
                {
                    throw new WebExceptionModel { ExceptionParam = ResponseStatus.设备离线 };
                }
                if (messageModel.MType == MessageType.OutCoin)
                {
                    var c = ClientModel.collection.Find(x => x.ClientID.Equals(connector.Client.ClientID)).FirstOrDefault();
                    var m = ((OutCoinMessageModel)messageModel);
                    m.Voltage = c.Voltage;
                    m.Chance = c.Chance;
                }
                connector.Send(messageModel.ToByteArray());
                messageModel.MStatus = MessageStatus.Sent;
                messageModel.UpDate();
            }
            catch (ExceptionModel em)
            {
                throw em;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="socketConnector">客户端连接器</param>
        /// <param name="messageModel">消息对象</param>
        public void Send(SocketConnector socketConnector, MessageModel messageModel)
        {
            try
            {
                socketConnector.Send(messageModel.ToByteArray());
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// 开启服务
        /// </summary>
        public void StartServer()
        {
            if (server != null && server.Connected)
            {
                return;
            }
            IPAddress ip = IPAddress.Parse(SERVER_IP);
            server = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(ip, SERVER_PORT));
            server.SendTimeout = 1000 * 10;
            server.Listen(50);

            new Thread(() =>
            {
                while (true)
                {
                    Socket client = server.Accept();
                    var connector = new SocketConnector(new ClientModel { Socket = client, Connected = true }, ReceivedData, OnSocketException);
                    socketConnectors.Add(connector);
                }

            }).Start();
        }

        /// <summary>
        /// 客户端断开连接后
        /// </summary>
        /// <param name="client">客户端对象</param>
        private void OnSocketException(ClientModel client)
        {
            var connector = socketConnectors.Find(x => x.Client == client);
            if (connector == null)
            {
                return;
            }
            socketConnectors.Remove(connector);

        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="dataSize">数据真实大小</param>
        /// <param name="data">数据</param>
        /// <param name="client">客户端对象</param>
        private void ReceivedData(int dataSize, byte[] data, ClientModel client)
        {
            MessageModel.ConvertToMsg(client, data, dataSize, messageManager);
        }
    }
}
