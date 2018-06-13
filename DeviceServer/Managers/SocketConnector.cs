using DeviceServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceServer.Managers
{
    /// <summary>
    /// 客户端连接器
    /// </summary>
    public class SocketConnector
    {
        /// <summary>
        /// 接收数据容器
        /// </summary>
        private byte[] resultData = new byte[1024 * 1024];

        /// <summary>
        /// 接收数据后回调函数
        /// </summary>
        private Action<int, byte[], ClientModel> receivedData;

        /// <summary>
        /// 连接中断回调函数
        /// </summary>
        private Action<ClientModel> socketException;

        /// <summary>
        /// 心跳线程
        /// </summary>
        private Thread heartThread;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="client">客户端对象</param>
        /// <param name="receivedData">接收到数据回调函数</param>
        /// <param name="socketException">连接异常回调函数</param>
        public SocketConnector(ClientModel client, Action<int, byte[], ClientModel> receivedData, Action<ClientModel> socketException)
        {
            Client = client;
            this.socketException = socketException;
            this.receivedData = receivedData;
            receiverThread = new Thread(Receiver);
            receiverThread.Start();
            heartThread = new Thread(ExecuteHeart);
            heartThread.Start();
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <param name="obj"></param>
        private void ExecuteHeart(object obj)
        {
            while (Client.Connected)
            {
                Thread.Sleep(1000 * 10);
                Send(new MessageModel { MType = MessageType.Heart }.ToByteArray());
            }
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        /// <param name="obj"></param>
        private void Receiver(object obj)
        {
            try
            {
                while (Client.Connected)
                {
                    int dataSize = Client.Socket.Receive(resultData);
                    if (dataSize > 0)
                    {
                        receivedData(dataSize, resultData, Client);
                    }
                }

            }
            catch (SocketException)
            {
                CloseSocket();
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        private void CloseSocket()
        {
            try
            {
                Client.Connected = false;
                Client.RefreshDB();
                socketException(Client);
                Client.Socket.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }
            finally
            {
                Client.Socket.Close();
                Client.Socket.Dispose();
            }

        }

        /// <summary>
        /// 客户端对象
        /// </summary>
        public ClientModel Client { get; set; }

        /// <summary>
        /// 接收数据线程
        /// </summary>
        private Thread receiverThread;

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        internal void Send(byte[] data)
        {
            try
            {
                if (Client.Connected && Client.Socket.Connected)
                {
                    Client.Socket.Send(data);
                }
            }
            catch (SocketException se)
            {
                CloseSocket();
                throw se;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
