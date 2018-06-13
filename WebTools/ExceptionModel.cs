using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using NTTools.DB;
using NTTools.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTools
{
    public class WebExceptionModel : Exception
    {
        [BsonId]
        [JsonConverter(typeof(ObjectIdConverter))]
        public ObjectId ExceptionID { get; set; }
        public string MethodFullName { get; set; }
        public ResponseStatus ExceptionParam { get; set; }
        public string Content { get; set; }
        [JsonConverter(typeof(DateConverterEndMinute))]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime ExceptionDate { get; set; }
        public string EXMessage { get; set; }
        public void Save()
        {
            new MongoDBTool().GetMongoCollection<WebExceptionModel>().InsertOne(this);
        }
    }

    public static class ExceptionExtends
    {
        public static void Save(this Exception e)
        {
            var method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            var em = new WebExceptionModel()
            {
                MethodFullName = $"{method.ReflectedType.FullName}.{method.Name}",
                EXMessage = e.Message,
                ExceptionDate = DateTime.Now
            };
            new MongoDBTool().GetMongoCollection<WebExceptionModel>().InsertOne(em);
        }
    }
}
