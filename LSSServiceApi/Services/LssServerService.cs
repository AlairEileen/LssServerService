using DeviceServer.Managers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace LSSServiceApi.Services
{
    public class LssServerService : WebHostService
    {
        private ILogger _logger;
        public LssServerService(IWebHost host) : base(host)
        {
            _logger = host.Services.GetRequiredService<ILogger<LssServerService>>();
        }
        protected override void OnStarting(string[] args)
        {
            _logger.LogDebug("OnStarting method called.");
            base.OnStarting(args);
        }
        protected override void OnStarted()
        {
            _logger.LogDebug("OnStarted method called.");
            base.OnStarted();
            MessageManager.GetMessageManager();

        }
        protected override void OnStopping()
        {
            _logger.LogDebug("OnStopping method called.");
            base.OnStopping();
        }
    }
    public static class WebHostServiceExtensions
    {
        public static void RunAsLssServerService(this IWebHost host)
        {
            var webHostService = new LssServerService(host);
            ServiceBase.Run(webHostService);
        }
    }
}
