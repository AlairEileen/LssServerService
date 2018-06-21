using DeviceServer.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTools.AppData;

namespace LSSServiceApi.AppData
{
    public class ClientData:BaseData<ClientModel>
    {
        internal List<ClientModel> GetAllClient()
        {
            var list = collection.Find(Builders<ClientModel>.Filter.Empty).ToList();
            return list;
        }

        internal void ChangeAuthorize(string id, bool authorize)
        {
           collection.UpdateOne(x => x.ID.Equals(new ObjectId(id)),
                Builders<ClientModel>.Update
                .Set(x => x.Authorized, authorize));
        }

        internal void SetChanceAndVoltage(ObjectId objectId, int chance, int voltage)
        {
            collection.UpdateOne(x=>x.ID.Equals(objectId), Builders<ClientModel>.Update
                .Set(x =>x.Chance, chance)
                .Set(x=>x.Voltage,voltage));
        }
    }
}
