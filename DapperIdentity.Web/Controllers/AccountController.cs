using DapperIdentity.Core.Identity;
using DapperIdentity.Data.Repositories;
using DapperIdentity.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DapperIdentity.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;

        public AccountController()
        {
            userManager = new UserManager<IdentityUser>(new UserRepository<IdentityUser>());
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindAsync(model.Email.TrimEnd(), model.Password);
            
                if (user != null)
                {                    
                    await userManager.UpdateSecurityStampAsync(user.Id);
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Invalid UserName of Password.");
            }

            return View(model);
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {                
                var confirmationToken = Guid.NewGuid().ToString();
                                
                var user = new IdentityUser { UserName = model.Email.TrimEnd(), Nickname = model.Nickname.TrimEnd(), IsConfirmed = true, ConfirmationToken = confirmationToken, CreatedDate = DateTime.UtcNow };

                var result = await userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    await SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }
            
            return View(model);
        }
        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;
        
        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }
        
        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            var identity = await userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                userManager?.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion
    }
}