using System;
using SquishIt.Framework.CSS;
using SquishIt.Framework.JavaScript;

namespace App.Client.Web.Infrastructure.Minification
{
    public static class MinificationsExtensions
    {
        /// <summary>
        /// Add the css bundle if the test condition is true
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="path"></param>
        /// <param name="testCondition"></param>
        /// <returns></returns>
        public static CSSBundle AddIfTrue(this CSSBundle bundle, string path, Func<bool> testCondition)
        {
            if (testCondition())
            {
                bundle.Add(path);
            }

            return bundle;
        }

        /// <summary>
        /// Add the css bundle if the test condition is true
        /// </summary>
        /// <param name="bundle"></param>
        /// <param name="path"></param>
        /// <param name="testCondition"></param>
        /// <returns></returns>
        public static JavaScriptBundle AddIfTrue(this JavaScriptBundle bundle, string path, Func<bool> testCondition)
        {
            if (testCondition())
            {
                bundle.Add(path);
            }

            return bundle;
        }
    }
}