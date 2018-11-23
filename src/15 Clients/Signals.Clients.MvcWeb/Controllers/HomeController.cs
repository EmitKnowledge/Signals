using Signals.Aspects.Auditing;
using Signals.Aspects.Auth;
using Signals.Aspects.BackgroundProcessing;
using Signals.Aspects.Caching;
using Signals.Aspects.CommunicationChannels;
using Signals.Aspects.Configuration;
using Signals.Aspects.ErrorHandling;
using Signals.Aspects.Localization;
using Signals.Aspects.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Signals.Core.Business;
using System.Web.Mvc;

namespace Signals.Clients.MvcWeb.Controllers
{
    public class HomeController : Controller
    {
        public HomeController(Signals.Aspects.Logging.ILogger logger,
            IConfigurationProvider configurationProvider,
            ICache cache,
            IAuditProvider audit,
            ILocalizationProvider localizationProvider,
            IStorageProvider storageProvider,
            IMessageChannel messageChannel,
            IAuthenticationManager authenticationManager,
            IAuthorizationManager authorizationManager,
            ITaskRegistry taskRegistry,
            IStrategyBuilder strategyBuilder)
        {

        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}