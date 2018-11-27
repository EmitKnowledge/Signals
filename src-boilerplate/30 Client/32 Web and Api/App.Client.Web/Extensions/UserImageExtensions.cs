using System.Web.Mvc;
using App.Client.Web.Controllers;
using App.Service.DomainEntities.Users;

namespace App.Client.Web.Extensions
{
    public static class UserImageExtensions
    {
        /// <summary>
        /// Return an url to the invoice profile picture
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string InvoiceLogo(this HtmlHelper helper, UserImageDimensionsEnum size)
        {
            var controller = helper.ViewContext.Controller as _BaseController;
            return helper.InvoiceLogo(controller.CurrentUser, size);
        }

        /// <summary>
        /// Return an url to the invoice profile picture
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="user"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string InvoiceLogo(this HtmlHelper helper, User user, UserImageDimensionsEnum size)
        {
            var sizeAsInt = (int) size;
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.Action(@"Image", @"InvoiceLogo", new
            {
                uid = user.Id,
                size = sizeAsInt,
                area = ""
            }, helper.ViewContext.RequestContext.HttpContext.Request.Url.Scheme);
            return url;
        }

        /// <summary>
        /// Return an url to the guest invoice profile picture
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string InvoiceGuestLogo(this HtmlHelper helper, string name, UserImageDimensionsEnum size)
        {
            var sizeAsInt = (int)size;
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var url = urlHelper.Action(@"Avatar", @"Client", new
            {
                name = name,
                size = sizeAsInt,
                area = ""
            }, helper.ViewContext.RequestContext.HttpContext.Request.Url.Scheme);
            return url;
        }
    }
}