using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _1361071.Helpers;

namespace _1361071.Filters
{
    public class CheckLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(CurrentContent.IsLogged() == false)
            {
                //string controller = filterContext.RouteData.Values["controller"].ToString();
                //string action = filterContext.RouteData.Values["action"].ToString();

                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

    }
}