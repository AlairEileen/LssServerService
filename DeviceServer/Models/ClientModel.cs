using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using NTTools.DB;
using NTTools.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DeviceServer.Models
{
    public class ClientModel:MongoDBExecutor<ClientModel>
    {
        /// <summary>
        /// 客户端数据库保存ID
        /// </summary>
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ID { get; set; }

        /// <summary>
        /// 客户端Socket，不进行持久化
        /// </summary>
        [BsonIgnore]
        public Socket Socket { get; set; }

        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 客户端第一次连接时间
        /// </summary>
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 客户端最后连接时间
        /// </summary>
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastChangeTime { get; set; }

        /// <summary>
        /// 客户端连接状态
        /// </summary>
        public bool Connected { get; set; }

        /// <summary>
        /// 客户端授权状态
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// 客户端数据库集合
        /// </summary>
        public static IMongoCollection<ClientModel> collection =
            new MongoDBTool().GetMongoCollection<ClientModel>();

        /// <summary>
        /// 刷新数据到数据库
        /// </summary>
        public void RefreshDB()
        {
            if (string.IsNullOrEmpty(ClientID))
                return;
            if (collection.Find(x => x.ClientID.Equals(ClientID)).FirstOrDefault() == null)
                SaveClient();
            else
                UpdataClient();
        }

        /// <summary>
        /// 更新当前客户端到数据库
        /// </summary>
        private void UpdataClient()
        {
            LastChangeTime = DateTime.Now;
            collection.UpdateOne(x => x.ClientID.Equals(ClientID), Builders<ClientModel>
                .Update
                .Set(x => x.Connected, Connected)
                .Set(x => x.LastChangeTime, LastChangeTime));
        }

        /// <summary>
        /// 保存当前客户端到数据库
        /// </summary>
        private void SaveClient()
        {
            CreateTime = DateTime.Now;
            LastChangeTime = DateTime.Now;
            collection.InsertOne(this);
        }
    }
}
