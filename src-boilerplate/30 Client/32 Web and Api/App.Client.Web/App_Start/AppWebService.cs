using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using App.Client.Web.Infrastructure.Bundiling;
using App.Client.Web.Infrastructure.ModelBinder;
using App.Client.Web.Infrastructure.Views;
using App.Common.Base.Dates;
using App.Common.Base.Logging;
using App.Common.Helpers.Serialization;
using App.Common.Helpers.Serialization.Convertors;
using App.Common.Logging.Extensibility;
using App.Service.ControllerInstance;
using Emit.ExtensibilityProvider.Concrete;
using Trackado.Client.Web.App_Start;

namespace App.Client.Web
{
    public class AppWebService
    {
        public static readonly AppWebService Instance;

        [LoggerImport]
        public ILogger Logger { get; set; }

        public SystemBootstrapper Bootstrapper { get; set; }

        static AppWebService()
        {
            //BsonSerializer.RegisterSerializer(typeof(EmitDateTime), new EmitDateTimeSerializer());
            var config = GlobalConfiguration.Configuration;
            var jsonSettings = config.Formatters.JsonFormatter.SerializerSettings;
            jsonSettings.Converters.Add(new EmitDateTimeConverter());

            try
            {
                Instance = new AppWebService();
            }
            catch (Exception ex)
            {

            }
        }

        private AppWebService()
        {
            Bootstrapper = new SystemBootstrapper();
            Bootstrapper.Execute(this);
        }

        /// <summary>
        /// Start initializing the container (by invoking a method static constructor will be called)
        /// </summary>
        public static void Init()
        {
            // init the service container
            AppControllerService.Init();

            ViewEngines.Engines.Clear();
            var ve = new RazorViewEngine();
            ve.ViewLocationCache = new TwoLevelViewCache(ve.ViewLocationCache);
            ViewEngines.Engines.Add(ve);
        }

        /// <summary>
        /// Register all areas
        /// </summary>
        public static void RegisterAllAreas()
        {
            AreaRegistration.RegisterAllAreas();
        }

        /// <summary>
        /// Register system routes
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        /// <summary>
        /// Register API routing
        /// </summary>
        public static void RegisterApiRoutes()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }

        /// <summary>
        /// Register global action filters
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        /// <summary>
        /// Register automatic mappings
        /// </summary>
        public static void RegisterAutoMapperMaps()
        {
            MapperConfig.RegisterEntityMaps();
        }

        /// <summary>
        /// Register custom model binders
        /// </summary>
        public static void RegisterModelBinders()
        {
            ModelBinders.Binders.Add(typeof(EmitDateTime), new EmitDateTimeModelBinder());
        }

        /// <summary>
        /// Register oauth providers
        /// </summary>
        public static void RegisterOAuth()
        {
            AuthConfig.RegisterAuth();
        }

        /// <summary>
        /// Register oauth providers
        /// </summary>
        public static void RegisterBundles()
        {
            var instances = (from t in Assembly.GetExecutingAssembly().GetTypes()
                             where t.IsClass &&
                                   typeof(BaseBundle).IsAssignableFrom(t) &&
                                   t.GetConstructor(Type.EmptyTypes) != null
                             select Activator.CreateInstance(t) as BaseBundle).ToList();

            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] != null) instances[i].Register();
            }
        }

        /// <summary>
        /// Strip MVC response header
        /// </summary>
        public static void StripHeaders()
        {
            MvcHandler.DisableMvcResponseHeader = true;
        }
    }
}