using Newtonsoft.Json;
using NTTools.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTools
{
    /// <summary>
    /// json result extensions methods 
    /// </summary>
    public static class JsonExtensionsApi
    {
        private static JsonSerializerSettings defaultJSS = new JsonSerializerSettings();

        /// <summary>
        /// success 
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="jsonObject">要序列化的JsonResponse</param>
        /// <param name="serializerSettings">json设置</param>
        /// <returns>jsonResult</returns>
        public static string JsonSuccess(this JsonResponse jsonObject, JsonSerializerSettings serializerSettings = null)
        {
            jsonObject.StatusCode = ResponseStatus.请求成功;
            jsonObject.Message = ResponseStatus.请求成功.ToString();
            return JsonConvert.SerializeObject(jsonObject, serializerSettings ?? defaultJSS);
        }

        /// <summary>
        /// success
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="jsonObject">要序列化的JsonResponse</param>
        /// <param name="limitParams">要显示的参数数组</param>
        /// <returns>jsonResult</returns>
        public static string JsonSuccessWithLimit(this JsonResponse jsonObject, string[] limitParams = null)
        {
            JsonSerializerSettings settings;
            if (limitParams == null)
            {
                settings = defaultJSS;
            }
            else
            {
                string[] parmas = new string[limitParams.Length + 2];
                parmas[0] = "StatusCode";
                parmas[1] = "Message";
                limitParams.CopyTo(parmas, 2);
                settings = new JsonSerializerSettings() { ContractResolver = new LimitPropsContractResolver(parmas) };
            }
            jsonObject.StatusCode = ResponseStatus.请求成功;
            jsonObject.Message = ResponseStatus.请求成功.ToString();
            return JsonConvert.SerializeObject(jsonObject, settings);
        }

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


        /// <summary>
        /// error status
        /// </summary>
        public static string JsonErrorStatus
        {
            get => JsonConvert.SerializeObject(new JsonResponse() { StatusCode = ResponseStatus.请求失败, Message = ResponseStatus.请求失败.ToString() }, defaultJSS);
        }

        /// <summary>
        /// success status
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <returns>jsonResult</returns>
        public static string JsonSuccessStatus
        {
            get => JsonConvert.SerializeObject(new JsonResponse() { StatusCode = ResponseStatus.请求成功, Message = ResponseStatus.请求成功.ToString() }, defaultJSS);
        }

        /// <summary>
        /// other stauts
        /// </summary>
        /// <param name="controller">控制器</param>
        /// <param name="actionParams">状态参数</param>
        /// <returns>jsonResult</returns>
        public static string JsonOtherStatus(ResponseStatus actionParams)
        {
            return JsonConvert.SerializeObject(new JsonResponse() { StatusCode = actionParams, Message = actionParams.ToString() }, defaultJSS);
        }

        /// <summary>
        /// jsonResponse convert to jsonResult
        /// </summary>
        /// <param name="obj">object</param>
        /// <param name="controller">current controller</param>
        /// <returns></returns>
        public static string ToJsonSuccess(this object obj, JsonSerializerSettings serializerSettings = null)
        {
            var resp = new JsonResponse1<object> { JsonData = obj };
            return resp.JsonSuccess(serializerSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="controller"></param>
        /// <param name="limitParams"></param>
        /// <returns></returns>
        public static string ToJsonSuccessWithLimit(this object obj, string[] limitParams = null)
        {
            var resp = new JsonResponse1<object> { JsonData = obj };
            return resp.JsonSuccessWithLimit(limitParams);
        }
    }
}
