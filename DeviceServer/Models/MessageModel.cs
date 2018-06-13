using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using NTTools.DB;
using NTTools.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DeviceServer.Models
{
    /// <summary>
    /// 消息类
    /// </summary>
    public class MessageModel
    {
        /// <summary>
        /// 将byte[]转为该实例
        /// </summary>
        /// <param name="clientModel">接受消息的客户端</param>
        /// <param name="data">需要转换的byte[]</param>
        /// <param name="dataSize">需要转换的数据大小</param>
        /// <param name="messageExecutor">转换完成后的处理函数</param>
        public static void ConvertToMsg(ClientModel clientModel, byte[] dataB, int dataSize, MessageExecutor messageExecutor)
        {

            if (clientModel == null || !clientModel.Connected)
            {
                return;
            }

            var dataArray = dataB.GetMessageByte(dataSize);
            foreach (var data in dataArray)
            {

                var bMsg = GetTypeAndMessageContent(data);
                switch (bMsg.Item1)
                {
                    case MessageType.Heart:
                        ExecuteMessage<MessageModel>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteHeart);
                        break;
                    case MessageType.OutCoinFinish:
                        ExecuteMessage<MessageModel>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteOutCoinFinish);
                        break;
                    case MessageType.ExecuteFailed:
                        ExecuteMessage<MessageModel>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteFailed);
                        break;
                    case MessageType.SetDefaultChanceFinish:
                        ExecuteMessage<DefaultChanceSetMessage>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteSetDefaultChanceFinish);
                        break;
                    case MessageType.SetDefaultVoltagSuccess:
                        ExecuteMessage<DefaultVoltageSetMessage>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteSetDefaultVoltageSuccess);
                        break;
                    case MessageType.ReportLog:
                        ExecuteMessage<LogMessage>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteLog);
                        break;
                    case MessageType.ReportBug:
                        ExecuteMessage<BugMessage>(clientModel, bMsg.Item2, dataSize, bMsg.Item1, messageExecutor.ExecuteBug);
                        break;
                    default:
                        break;
                }
            }

            clientModel.RefreshDB();
        }

        private static byte[][] GetAllTypeByteArray()
        {
            var fields = typeof(MessageType).GetFields(BindingFlags.Static | BindingFlags.Public);
            var array = new List<byte[]>();

            foreach (var fi in fields)
                array.Add(BitConverter.GetBytes((int)fi.GetValue(null)));
            return array.ToArray();
        }

        private static (MessageType, string) GetTypeAndMessageContent(byte[] data)
        {
            var ht = GetHeaderValue(GetAllTypeByteArray(), data);
            var v = "";
            if (ht.Length < data.Length)
            {
                v = Encoding.ASCII.GetString(data, ht.Length, data.Length - ht.Length);
            }
            return ((MessageType)BitConverter.ToInt32(ht, 0), v);
        }


        /// <summary>
        /// 从数据包中获取指令
        /// </summary>
        /// <param name="v">所有指令数组</param>
        /// <param name="data">当前数据包</param>
        /// <returns></returns>
        private static byte[] GetHeaderValue(byte[][] v, byte[] data)
        {

            byte[] b = null;
            foreach (var item in v)
            {
                var flag = true;
                for (int i = 0; i < item.Length; i++)
                {
                    if (item[0] != data[0])
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                {
                    b = item;
                    break;
                }
            }
            return b;
        }


        /// <summary>
        /// 执行该类型消息
        /// </summary>
        /// <typeparam name="T">需要转换消息的最终类型</typeparam>
        /// <param name="clientModel">接受消息的客户端</param>
        /// <param name="data">需要转换的byte[]</param>
        /// <param name="dataSize">需要转换的数据大小</param>
        /// <param name="msgType">消息类型</param>
        /// <param name="execute">转换完成后的处理函</param>
        private static void ExecuteMessage<T>(ClientModel clientModel, string json, int dataSize, MessageType msgType, Action<T> execute) where T : MessageModel
        {
            var msg = JsonConvert.DeserializeObject<T>(json);
            msg.MType = msgType;
            clientModel.ClientID = msg.ClientID;
            execute(msg);
        }



        /// <summary>
        /// 该消息的DB集合
        /// </summary>
        private static IMongoCollection<MessageModel> collection = new MongoDBTool().GetMongoCollection<MessageModel>();

        /// <summary>
        /// 保存该实例到DB
        /// </summary>
        public void Save()
        {
            collection.InsertOne(this);
        }

        /// <summary>
        /// 向DB更新该实例
        /// </summary>
        public void UpDate()
        {
            collection.UpdateOne(x => x.MessageID.Equals(MessageID), Builders<MessageModel>
                .Update
                .Set(x => x.MStatus, MStatus));
        }

        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType MType { get; set; }

        /// <summary>
        /// 消息状态
        /// </summary>
        public MessageStatus MStatus { get; set; }

        /// <summary>
        /// 消息设备ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 消息ID
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId MessageID { get; set; }

        /// <summary>
        /// 将该实例的类型转为byte[]
        /// </summary>
        /// <returns>该实例类型的byte[]</returns>
        private byte[] GetTypeByteArray()
        {
            var type = (int)MType;

            return BitConverter.GetBytes(type);
        }

        /// <summary>
        /// 获取该实例的Json
        /// </summary>
        /// <returns>该实例的Json</returns>
        public virtual string GetCurrentJson()
        {
            return this.JsonWithLimit(new string[] {
                "MessageID"
            });

        }

        internal byte[] ToMessageByte()
        {
            var jsonData = GetCurrentJson();
            var coreData = Encoding.ASCII.GetBytes(jsonData);
            var data = new byte[coreData.Length + GetTypeByteArray().Length];
            GetTypeByteArray().CopyTo(data, 0);
            coreData.CopyTo(data, GetTypeByteArray().Length);
            return data;
        }


        /// <summary>
        /// 将该实例转为byte[]
        /// </summary>
        /// <returns>该实例的byte[]</returns>
        internal byte[] ToByteArray()
        {
            return this.ToPackage();
        }
    }

    /// <summary>
    /// 消息状态枚举
    /// </summary>
    public enum MessageStatus
    {
        /// <summary>
        /// 创建完成
        /// </summary>
        Created = 1,
        /// <summary>
        /// 已发送
        /// </summary>
        Sent = 2,
        /// <summary>
        /// 已完成
        /// </summary>
        finish = 3,
        /// <summary>
        /// 失败
        /// </summary>
        failed = 4
    }

    /// <summary>
    /// 默认概率消息
    /// </summary>
    public class DefaultChanceSetMessage : MessageModel
    {
        public int Chance { get; set; }
        /// <summary>
        /// 获取当前实例的Json
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentJson()
        {
            return this.JsonWithLimit(new string[] {
                "MessageID",
                "Chance"
            });
        }
    }

    /// <summary>
    /// 默认电压消息
    /// </summary>
    public class DefaultVoltageSetMessage : MessageModel
    {
        public int Voltage { get; set; }
        /// <summary>
        /// 获取当前实例的Json
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentJson()
        {
            return this.JsonWithLimit(new string[] {
                "MessageID",
                "Voltage"
            });
        }
    }

    /// <summary>
    /// 出币消息类
    /// </summary>
    public class OutCoinMessageModel : DefaultChanceSetMessage
    {
        /// <summary>
        /// 出币数量
        /// </summary>
        public int CoinCount { get; set; }
        /// <summary>
        /// 出币电压
        /// </summary>
        public int Voltage { get; set; }

        /// <summary>
        /// 获取当前实例的Json
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentJson()
        {
            return this.JsonWithLimit(new string[] {
                "MessageID",
                "CoinCount",
                "Voltage",
                "Chance"
            });
        }
    }

    /// <summary>
    /// 日志消息
    /// </summary>
    public class LogMessage : MessageModel
    {
        public LogModel LogInfo { get; set; }

        /// <summary>
        /// 获取当前实例的Json
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentJson()
        {
            return this.JsonWithLimit(new string[] {
                 "MessageID",
                "LogInfo",
                "Content"
            });
        }
    }

    /// <summary>
    /// 错误消息
    /// </summary>
    public class BugMessage : MessageModel
    {
        public BugModel BugInfo { get; set; }
        /// <summary>
        /// 获取当前实例的Json
        /// </summary>
        /// <returns></returns>
        public override string GetCurrentJson()
        {
            return this.JsonWithLimit(new string[] {
                 "MessageID",
                "LogInfo",
                "Content"
            });
        }
    }


    /// <summary>
    /// 消息类型枚举
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 心跳
        /// </summary>
        Heart = 1000,
        /// <summary>
        /// 出币
        /// </summary>
        OutCoin = 2000,
        /// <summary>
        /// 出币完成
        /// </summary>
        OutCoinFinish = 2001,
        /// <summary>
        /// 执行失败
        /// </summary>
        ExecuteFailed = 4000,
        /// <summary>
        /// 上报日志
        /// </summary>
        ReportLog = 7000,
        /// <summary>
        /// 上报故障
        /// </summary>
        ReportBug = 7001,
        /// <summary>
        /// 设置默认概率
        /// </summary>
        SetDefaultChance = 9000,
        /// <summary>
        /// 设置默认概率成功
        /// </summary>
        SetDefaultChanceFinish = 9001,
        /// <summary>
        /// 设置默认电压
        /// </summary>
        SetDefaultVoltage = 9002,
        /// <summary>
        /// 设置默认电压成功
        /// </summary>
        SetDefaultVoltagSuccess = 9003
    }

    /// <summary>
    /// 消息执行接口
    /// </summary>
    public interface MessageExecutor
    {
        /// <summary>
        /// 执行心跳
        /// </summary>
        /// <param name="message"></param>
        void ExecuteHeart(MessageModel message);

        /// <summary>
        /// 执行出币完成
        /// </summary>
        /// <param name="message"></param>
        void ExecuteOutCoinFinish(MessageModel message);

        /// <summary>
        /// 成功设置默认概率
        /// </summary>
        /// <param name="message"></param>
        void ExecuteSetDefaultChanceFinish(DefaultChanceSetMessage message);

        /// <summary>
        /// 成功设置默认概率
        /// </summary>
        /// <param name="message"></param>
        void ExecuteSetDefaultVoltageSuccess(DefaultVoltageSetMessage message);

        /// <summary>
        /// 成功设置默认概率
        /// </summary>
        /// <param name="message"></param>
        void ExecuteLog(LogMessage message);
        /// <summary>
        /// 成功设置默认概率
        /// </summary>
        /// <param name="message"></param>
        void ExecuteBug(BugMessage message);

        /// <summary>
        /// 任务执行失败
        /// </summary>
        /// <param name="message"></param>
        void ExecuteFailed(MessageModel message);
    }
}
