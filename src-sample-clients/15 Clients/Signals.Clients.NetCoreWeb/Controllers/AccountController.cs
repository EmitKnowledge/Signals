using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Signals.Aspects.Auth;
using Signals.Aspects.Auth.Attributes;
using Signals.Aspects.Auth.NetCore.Attributes;
using System.Security.Claims;
using System.Threading.Tasks;
using Signals.Core.Processes;

namespace Signals.Clients.NetCoreWeb.Controllers
{
    public class User
    {
        [ClaimType(ClaimTypes.NameIdentifier)]
        [ClaimType(ClaimTypes.Name)]
        public string Name { get; set; }


        [ClaimType(ClaimTypes.DateOfBirth)]
        public int Age { get; set; }
    }

    public class AccountController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IAuthorizationManager _authorizationManager;

        public AccountController(IAuthenticationManager authenticationManager, IAuthorizationManager authorizationManager)
        {
            _authenticationManager = authenticationManager;
            _authorizationManager = authorizationManager;
        }

        public ActionResult LoginExternal(string provider)
        {
            _authenticationManager.Logout();

            var redirectUrl = Url.Action(nameof(AccountController.LoginCallback), "Account", new { provider = provider });

            return Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl }, provider);
        }

        [HttpGet]
        public async Task<ActionResult> LoginCallback(string provider)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(provider);

            _authenticationManager.Login(
                authenticateResult.Principal,
                new User
                {
                    Name = "Vojdan",
                    Age = 25
                });


            return RedirectToAction("Index");
        }

        public ActionResult Login()
        {
            _authenticationManager.Login(
                new ClaimsPrincipal(new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme)),
                new User
                {
                    Name = "Vojdan",
                    Age = 25
                });
            _authorizationManager.AddRoles("Admin", "SuperAdmin");

            return RedirectToAction("Index");
        }

        public ActionResult Logout()
        {
            _authenticationManager.Logout();
            return RedirectToAction("Index");
        }

        [SignalsAuthorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var user = _authenticationManager.GetCurrentUser<User>();

            return View();
        }
    }
}
