using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Signals.Aspects.DI;
using Signals.Aspects.Localization;

namespace App.Client.Web.Extensions
{
    public static class HtmlLocalizationExtension
    {
        public static HtmlString Translate(this IHtmlHelper helper, string key)
        {
            var translation = SystemBootstrapper.GetInstance<ILocalizationProvider>()?.Get(key)?.Value;
            return new HtmlString(translation ?? "Localization provider not initialized");
        }
    }
}
