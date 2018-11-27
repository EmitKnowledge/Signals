using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;
using App.Client.Web.Extensions;

namespace App.Client.Web.Infrastructure.Localization
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        private static List<string> _availableLanguages;

        private readonly string _defaultLanguage = @"en";

        private readonly string _selectedLanguage;

        /// <summary>
        /// CTOR
        /// </summary>
        static LocalizationAttribute()
        {
            _availableLanguages = TranslationExtensions.GetAvailableLanguages();
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="selectedLanguage"></param>
        public LocalizationAttribute(string selectedLanguage = @"en")
        {
            _selectedLanguage = selectedLanguage;
        }

        /// <summary>
        /// Called by the ASP.NET MVC framework before the action method executes.
        /// </summary>
        /// <param name="filterContext">The filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string lang = (string)filterContext.RouteData.Values["lang"] ?? _selectedLanguage;
            if (lang != _selectedLanguage)
            {
                if (!_availableLanguages.Contains(lang))
                {
                    lang = string.Copy(_defaultLanguage);
                    var routeData = filterContext.RouteData;
                    routeData.Values[@"lang"] = lang;
                    object routeName = null;
                    routeData.DataTokens.TryGetValue("__RouteName", out routeName);
                    filterContext.Result = new RedirectToRouteResult(routeName as string, routeData.Values);
                    return;
                }

                try
                {
                    Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(lang);
                }
                catch (Exception e)
                {
                    Thread.CurrentThread.CurrentCulture =
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(_defaultLanguage);
                }
            }
        }
    }
}