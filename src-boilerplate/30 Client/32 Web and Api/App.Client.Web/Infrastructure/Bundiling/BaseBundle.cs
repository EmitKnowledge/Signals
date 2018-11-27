using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Web.Mvc;
using App.Common.Helpers.Cryptography;
using SquishIt.Framework;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace App.Client.Web.Infrastructure.Bundiling
{
    [Export]
    public abstract class BaseBundle
    {
        /// <summary>
        /// Default Css min.output dir
        /// </summary>
        protected virtual string CssFilePath => @"~/minified/css";

        /// <summary>
        /// Default Javascript min.output dir
        /// </summary>
        protected virtual string JsFilePath => @"~/minified/script";

        /// <summary>
        /// CSS Bundle
        /// </summary>
        protected CSSBundle CssBundle { get; set; }

        /// <summary>
        /// JavaScript bundle
        /// </summary>
        protected JavaScriptBundle JavaScriptBundle { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public void Register()
        {
            CssBundle = Bundle.Css();
            JavaScriptBundle = Bundle.JavaScript();

            // register css and scripts
            var shouldRegisterCss = RegisterCss();
            var shouldRegisterJavascript = RegisterJavaScript();

            var typeName = this.GetType().FullName.ToLower();
            // configure css and javascript registration
            if (shouldRegisterCss)
            {
                var cssOutputFile = $@"{CssFilePath}/{typeName}.css";
                CssBundle.AsNamed(typeName, cssOutputFile);
            }

            if (shouldRegisterJavascript)
            {
                var javascriptOutputFile = $@"{JsFilePath}/{typeName}.js";
                JavaScriptBundle.AsNamed(typeName, javascriptOutputFile);
            }
        }

        /// <summary>
        /// Register css as bundle
        /// </summary>
        protected abstract bool RegisterCss();

        /// <summary>
        /// Register javascript as bundle
        /// </summary>
        protected abstract bool RegisterJavaScript();

        /// <summary>
        /// Render the minified css
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString RenderCss<T>() where T : BaseBundle
        {
            var typeName = typeof(T).FullName.ToLower();
            return new MvcHtmlString(Bundle.Css().RenderNamed(typeName));
        }

        /// <summary>
        /// Render the minified javascript
        /// </summary>
        /// <returns></returns>
        public static MvcHtmlString RenderJavaScript<T>() where T : BaseBundle
        {
            var typeName = typeof(T).FullName.ToLower();
            return new MvcHtmlString(Bundle.JavaScript().RenderNamed(typeName));
        }
    }
}