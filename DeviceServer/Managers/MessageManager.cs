using DeviceServer.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using NTTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebTools;

namespace DeviceServer.Managers
{
    public class MessageManager : MessageExecutor
    {
        /// <summary>
        /// 连接管理器对象
        /// </summary>
        private SocketManager socketManager;
        /// <summary>
        /// 消息管理器对象，单例
        /// </summary>
        private static MessageManager messageManager;

        //Dictionary<ObjectId, ResponseStatus> taskStatusDic = new Dictionary<ObjectId, ResponseStatus>();
        //Dictionary<ObjectId, AutoResetEvent> taskEventDic = new Dictionary<ObjectId, AutoResetEvent>();

        /// <summary>
        /// 提示线程信息集合
        /// </summary>
        List<NotifyMessageModel> notifyMessages = new List<NotifyMessageModel>();

        /// <summary>
        /// 构造函数
        /// </summary>
        private MessageManager()
        {
            this.socketManager = new SocketManager(this);
        }

        /// <summary>
        /// 获取消息管理器实例
        /// </summary>
        /// <returns>消息管理器对象</returns>
        public static MessageManager GetMessageManager()
        {
            if (messageManager == null)
            {
                messageManager = new MessageManager();
            }
            return messageManager;
        }

        /// <summary>
        /// 执行心跳
        /// </summary>
        /// <param name="message"></param>
        public void ExecuteHeart(MessageModel message)
        {
            Console.WriteLine("Heart:" + message.ClientID);
        }

        /// <summary>
        /// 出币完成
        /// </summary>
        /// <param name="message">消息实例</param>
        public void ExecuteOutCoinFinish(MessageModel message)
        {
            NotifyTask(message, ResponseStatus.请求成功);
            message.MStatus = MessageStatus.finish;
            message.UpDate();

        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="messageModel">消息实例</param>
        /// <returns></returns>
        public async Task<ResponseStatus> Send(MessageModel messageModel)
        {
            var client = ClientModel.collection.Find(x => x.ClientID.Equals(messageModel.ClientID)).FirstOrDefault();
            if (!client.Authorized)
            {
                return ResponseStatus.设备未开启使用;
            }
            messageModel.MStatus = MessageStatus.Created;
            messageModel.Save();

            return await Task.Run(() =>
            {
                try
                {
                    AutoResetEvent autoResetEvent = new AutoResetEvent(false);
                    notifyMessages.Add(new NotifyMessageModel
                    {
                        MessageID = messageModel.MessageID,
                        Event = autoResetEvent,
                        Status = ResponseStatus.请求超时
                    });
                    //taskStatusDic.Add(messageModel.MessageID, ResponseStatus.请求超时);
                    //taskEventDic.Add(messageModel.MessageID, autoResetEvent);
                    socketManager.Send(messageModel);
                    autoResetEvent.WaitOne(1000 * 10);
                    //var status = taskStatusDic[messageModel.MessageID];
                    var nmm = notifyMessages.Find(x => x.MessageID.Equals(messageModel.MessageID));
                    var status = nmm.Status;
                    //taskStatusDic.Remove(messageModel.MessageID);
                    //taskEventDic.Remove(messageModel.MessageID);
                    notifyMessages.Remove(nmm);
                    return status;
                }
                catch (WebExceptionModel em)
                {
                    return em.ExceptionParam;
                }
                catch (Exception)
                {
                    return ResponseStatus.请求失败;
                }

            });
        }


        /// <summary>
        /// 提醒线程数据
        /// </summary>
        /// <param name="message">消息实例</param>
        /// <param name="resStatus">消息执行状态</param>
        private void NotifyTask(MessageModel message, ResponseStatus resStatus)
        {
            var nmm = notifyMessages.Find(x => x.MessageID.Equals(message.MessageID));
            if (nmm != null)
            {
                nmm.Status = resStatus;
                nmm.Event.Set();
            }
            //ResponseStatus status;
            //if (taskStatusDic.TryGetValue(message.MessageID, out status))
            //{
            //    taskStatusDic[message.MessageID] = resStatus;
            //    AutoResetEvent autoResetEvent;
            //    if (taskEventDic.TryGetValue(message.MessageID, out autoResetEvent))
            //    {
            //        autoResetEvent.Set();
            //    }
            //}
        }

        /// <summary>
        /// 执行设置默认概率成功
        /// </summary>
        /// <param name="message"></param>
        public void ExecuteSetDefaultChanceFinish(DefaultChanceSetMessage message)
        {
            NotifyTask(message, ResponseStatus.请求成功);
            message.MStatus = MessageStatus.finish;
            message.UpDate();
        }

        /// <summary>
        /// 执行失败
        /// </summary>
        /// <param name="message"></param>
        public void ExecuteFailed(MessageModel message)
        {
            NotifyTask(message, ResponseStatus.请求失败);
            message.MStatus = MessageStatus.failed;
            message.UpDate();
        }

        public void ExecuteSetDefaultVoltageSuccess(DefaultVoltageSetMessage message)
        {
            NotifyTask(message, ResponseStatus.请求成功);
            message.MStatus = MessageStatus.finish;
            message.UpDate();
        }

        public void ExecuteLog(LogMessage message)
        {
            var client = new ClientModel().Collection().Find(x => x.ClientID.Equals(message.ClientID)).FirstOrDefault();
            message.LogInfo.Client = client;
            message.LogInfo.Collection().InsertOne(message.LogInfo);
        }

        public void ExecuteBug(BugMessage message)
        {
            var client = new ClientModel().Collection().Find(x => x.ClientID.Equals(message.ClientID)).FirstOrDefault();
            message.BugInfo.Client = client;
            message.BugInfo.Collection().InsertOne(message.BugInfo);
        }
    }

    /// <summary>
    /// 消息提醒类型
    /// </summary>
    public class NotifyMessageModel
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public ObjectId MessageID { get; set; }
        /// <summary>
        /// 消息线程监听事件
        /// </summary>
        public AutoResetEvent Event { get; set; }
        /// <summary>
        /// 消息状态
        /// </summary>
        public ResponseStatus Status { get; set; }
    }
}
