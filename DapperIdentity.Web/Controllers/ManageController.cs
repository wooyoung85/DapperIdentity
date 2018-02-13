using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DapperIdentity.Data.Repositories;
using DapperIdentity.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using DapperIdentity.Core.Identity;

namespace DapperIdentity.Web.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ManageController()
        {
            _userManager = new UserManager<IdentityUser>(new UserRepository<IdentityUser>());
        }

       //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword()
            };
            return View(model);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
                
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userManager?.Dispose();
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

        private bool HasPassword()
        {
            var user = _userManager.FindById(User.Identity.GetUserId());
            return user?.PasswordHash != null;
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess
            Error
        }

        private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

        private async Task SignInAsync(IdentityUser user, bool isPersistent)
        {
            var identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }
        #endregion
    }
}