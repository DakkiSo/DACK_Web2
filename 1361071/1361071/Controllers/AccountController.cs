using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using _1361071.Models;
using _1361071.Helpers;
using BotDetect.Web.Mvc;
using _1361071.Filters;

namespace _1361071.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
        }

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            string encPwd = StringUtils.Md5(model.RawPWD);

            using (QLBHEntities ctx = new QLBHEntities())
            {
                var user = ctx.Users
                    .Where(u => u.f_Username == model.Username && u.f_Password == encPwd)
                    .FirstOrDefault();

                if (user != null)
                {
                    Session["isLogin"] = 1;
                    Session["user"] = user;

                    if(model.Remember)
                    {
                        Response.Cookies["x"].Value = user.f_ID.ToString();
                        Response.Cookies["x"].Expires = DateTime.Now.AddDays(7);
                    }

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ErrorMsg = "Đăng nhập thất bại";
                    return View();
                }
                   
            }

        }

        //
        // POST: /Account/Logout
        [HttpPost]
        public ActionResult Logout()
        {
            CurrentContent.destroy();
            return RedirectToAction("Index", "Home");

        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [CaptchaValidation("CaptchaCode", "ExampleCaptcha", "Incorrect CAPTCHA code!")]
        public ActionResult Register(RegisterVM model)
        {

            if (!ModelState.IsValid)
            {
                // TODO: Captcha validation failed, show error message

                ViewBag.ErrorMsg = "Incorrect CAPTCHA code!";
            }
            else
            {
                // TODO: Captcha validation passed, proceed with protected action

                User u = new User
                {
                    f_Username = model.Username,
                    f_Email = model.Email,
                    f_Name = model.Name,
                    f_Password = StringUtils.Md5(model.RawPWD),
                    f_Permission = 0,
                    f_DOB = DateTime.ParseExact(model.DOB, "d/m/yyyy", null)
                };

                using (QLBHEntities ctx = new QLBHEntities())
                {
                    ctx.Users.Add(u);
                    ctx.SaveChanges();
                }  
            }
            return View();
            
        }


        //
        // GET: /Account/Profile
        [CheckLogin]
        public ActionResult Profile()
        {

            return View();
        }
    

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}