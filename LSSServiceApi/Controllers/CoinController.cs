using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceServer.Managers;
using DeviceServer.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NTTools.Models;
using WebTools;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LSSServiceApi.Controllers
{
    [Route("api/[controller]")]
    public class CoinController : Controller
    {
        // GET: api/<controller>
        /// <summary>
        /// 出币
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> SendOutCoin(OutCoinMessageModel outCoinMessageModel)
        {
            try
            {
                outCoinMessageModel.MType =MessageType.OutCoin;
                outCoinMessageModel.MessageID = ObjectId.GenerateNewId();
                return JsonExtensionsApi.JsonOtherStatus(
                    await MessageManager.GetMessageManager()
                    .Send(outCoinMessageModel));
            }
            catch (WebTools.WebExceptionModel em)
            {
                return JsonExtensionsApi. JsonOtherStatus(em.ExceptionParam);
            }
            catch (Exception)
            {
                return JsonExtensionsApi.JsonErrorStatus;
            }
        }

    }
}
