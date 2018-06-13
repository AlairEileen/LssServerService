using MongoDB.Driver;
using NTTools.DB;
using NTTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceServer.Models
{

    public interface MongoDBExecutor<T>
    {


    }
    public static class MongoDBExecutorExtensions
    {
        public static IMongoCollection<T> Collection<T>(this MongoDBExecutor<T> me) { return new MongoDBTool().GetMongoCollection<T>(); }
        public static UpdateDefinitionBuilder<T> Update<T>(this MongoDBExecutor<T> me) => Builders<T>.Update;
        public static FilterDefinitionBuilder<T> Filter<T>(this MongoDBExecutor<T> me) => Builders<T>.Filter;

    }
    public class ContentModel<T> : BaseModel, MongoDBExecutor<T>
    {
        public ClientModel Client { get; set; }
        public string Content { get; set; }
        //private static MongoDBTool mongoDBTool = new MongoDBTool();
        //public static IMongoCollection<T> Collection { get => mongoDBTool.GetMongoCollection<T>(); }
        //public static UpdateDefinitionBuilder<T> Update { get => Builders<T>.Update; }
        //public static FilterDefinitionBuilder<T> Filter { get => Builders<T>.Filter; }

    }

    public class LogModel : ContentModel<LogModel>
    {
    }
    public class BugModel : ContentModel<BugModel>
    {
    }
}
