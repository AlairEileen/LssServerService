using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceServer.Models;
using LSSServiceApi.AppData;
using Microsoft.AspNetCore.Mvc;
using WebTools;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LSSServiceApi.Controllers
{
    [Route("api/[controller]")]
    public class ClientController : BaseController<ClientData, ClientModel>
    {
      

        // GET: api/<controller>
        [HttpGet]
        public string Get()
        {
            try
            {
                var list = thisData.GetAllClient();
                return list.ToJsonSuccessWithLimit(new string[]
                {
                "JsonData",
                "ID",
                "ClientID",
                "CreateTime",
                "LastChangeTime",
                "Authorized",
                "Connected"
                });
            }
            catch (Exception)
            {
                return JsonExtensionsApi.JsonErrorStatus;
            }
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public string Put(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return JsonExtensionsApi.JsonOtherStatus(ResponseStatus.请求参数不正确);
                }
                thisData.ChangeAuthorize(id, true);
                return JsonExtensionsApi.JsonSuccessStatus;
            }
            catch (Exception)
            {
                return JsonExtensionsApi.JsonErrorStatus;
            }
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public string Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return JsonExtensionsApi.JsonOtherStatus(ResponseStatus.请求参数不正确);
                }
                thisData.ChangeAuthorize(id, false);
                return JsonExtensionsApi.JsonSuccessStatus;
            }
            catch (Exception)
            {
                return JsonExtensionsApi.JsonErrorStatus;
            }
        }
    }
}
