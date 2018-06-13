using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebTools
{
    public enum ResponseStatus
    {
        /// <summary>
        /// 请求成功
        /// </summary>
        请求成功 = 1000,
        /// <summary>
        /// 设备执行成功
        /// </summary>
        设备执行成功 = 1001,
        /// <summary>
        /// 请求失败
        /// </summary>
        请求失败 = 2000,
        /// <summary>
        /// 请求参数不正确
        /// </summary>
        请求参数不正确 = 2001,
        /// <summary>
        /// 网络不给力
        /// </summary>
        网络不给力 = 2004,
        /// <summary>
        /// 设备繁忙
        /// </summary>
        设备繁忙 = 2002,
        /// <summary>
        /// 请求超时
        /// </summary>
        请求超时 = 2003,
        /// <summary>
        /// 数据为空
        /// </summary>
        数据为空 = 3000,
        /// <summary>
        /// 验证失败
        /// </summary>
        验证失败 = 3001,
        /// <summary>
        /// 设备离线
        /// </summary>
        设备离线 = 3003,
        /// <summary>
        /// 设备未开启使用
        /// </summary>
        设备未开启使用 = 3004,
        /// <summary>
        /// 重复异常
        /// </summary>
        重复异常 = 3002,
        /// <summary>
        /// 余额不足
        /// </summary>
        余额不足 = 4000,
        /// <summary>
        /// 该账户已经领取
        /// </summary>
        该账户已经领取 = 5000,
        /// <summary>
        /// 红包已经领取完成
        /// </summary>
        红包已经领取完成 = 5001,
        /// <summary>
        /// 领取人数太多
        /// </summary>
        领取人数太多 = 5002,
        /// <summary>
        /// 领取人数太少
        /// </summary>
        领取人数太少 = 5003,
        /// <summary>
        /// 不是汉字
        /// </summary>
        不是汉字 = 5004
    }
}
