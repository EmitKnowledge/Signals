using System.Web.Mvc;
using App.Client.SecurityProvider.Provider;
using App.Client.Web.Infrastructure.AuthorizeAttributes;

namespace App.Client.Web.Controllers
{
    public class AccountController : _BaseController
    {
        #region Login

        /// <summary>
        /// Logout the user
        /// </summary>
        /// <returns></returns>
        [AuthorizedUser]
        public ActionResult Logout()
        {
            MvcFormsSecurityProvider.Logout();
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
