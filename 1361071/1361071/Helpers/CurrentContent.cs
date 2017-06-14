using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using _1361071.Models;

namespace _1361071.Helpers
{
    public class CurrentContent
    {
        public static bool IsLogged()
        {
            var flag = HttpContext.Current.Session["isLogin"];
            if(flag == null || (int)flag == 0)
            {

                if(HttpContext.Current.Request.Cookies["userId"] != null)
                {
                    int userIdCookie = Convert.ToInt32(HttpContext.Current.Request.Cookies["userId"]);

                    using (var ctx = new QLBHEntities())
                    {
                        var user = ctx.Users
                            .Where(u => u.f_ID == userIdCookie)
                            .FirstOrDefault();

                        HttpContext.Current.Session["isLogin"] = 1;
                        HttpContext.Current.Session["user"] = user;
                    }
                    return true;
                }
                return false;
            }
            return true;
        }

        public static User GetCurUser() 
        {
            return (User)HttpContext.Current.Session["user"];
        }

        public static void destroy()
        {
            HttpContext.Current.Session["isLogin"] = 0;
            HttpContext.Current.Session["user"] = null;

            HttpContext.Current.Response.Cookies["userId"].Expires = DateTime.Now.AddDays(-1);
        }
    }
}