using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTTools.Jsons
{
    public static class JsonExtensions
    {
        /// <summary>
        /// success
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="jsonObject">要序列化的JsonResponse</param>
        /// <param name="limitParams">要显示的参数数组</param>
        /// <returns>jsonResult</returns>
        public static string JsonWithLimit(this object obj, string[] limitParams)
        {
            JsonSerializerSettings settings;
            settings = new JsonSerializerSettings
            { ContractResolver = new LimitPropsContractResolver(limitParams) };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
