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
    public class ChanceController : Controller
    {
        /// <summary>
        /// 设置默认概率
        /// </summary>
        /// <param name="id">客户端ID</param>
        /// <param name="chance">概率</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<string> SendDefaultChance(DefaultChanceSetMessage defaultChanceSetMessage)
        {
            try
            {
                defaultChanceSetMessage.MType = MessageType.SetDefaultChance;
                return JsonExtensionsApi.JsonOtherStatus(
                    await MessageManager.GetMessageManager()
                    .Send(defaultChanceSetMessage));
            }
            catch (WebExceptionModel em)
            {
                return JsonExtensionsApi.JsonOtherStatus(em.ExceptionParam);
            }
            catch (Exception)
            {
                return JsonExtensionsApi.JsonErrorStatus;
            }
        }
    
    }
}
