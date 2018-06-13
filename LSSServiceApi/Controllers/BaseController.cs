using LSSServiceApi.AppData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebTools.AppData;

namespace LSSServiceApi.Controllers
{
    public class BaseController<T, P> : Microsoft.AspNetCore.Mvc.Controller where T : BaseData<P>
    {
        protected T thisData;
        protected bool hasIdentity;
        protected BaseController(bool hasIdentity = false)
        {
            thisData = Activator.CreateInstance<T>();
            this.hasIdentity = hasIdentity;
        }
        public override void OnActionExecuting(Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext context)
        {
            if (hasIdentity)
            {
                //if (!context.HttpContext.Session.HasUserData())
                //{
                //    context.Result = new RedirectToActionResult("Index", "Error", new { errorType = ErrorType.ErrorNoUserOrTimeOut });
                //    return;
                //}
            }
            base.OnActionExecuting(context);
        }

    }
}
