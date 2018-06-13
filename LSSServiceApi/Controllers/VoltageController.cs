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
    public class VoltageController : Controller
    {
        /// <summary>
        /// 设置默认电压
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        public async Task<string> SendDefaultVoltage(DefaultVoltageSetMessage defaultVoltageSetMessage)
        {
            try
            {
                defaultVoltageSetMessage.MType = MessageType.SetDefaultVoltage;
                return JsonExtensionsApi.JsonOtherStatus(
                    await MessageManager.GetMessageManager()
                    .Send(defaultVoltageSetMessage));
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
