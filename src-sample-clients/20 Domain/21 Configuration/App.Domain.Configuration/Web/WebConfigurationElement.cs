using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Web
{
    public sealed class WebConfigurationElement
    {
        /// <summary>
        /// Web endpoing url
        /// </summary>
        [Required]
        public UrlFragment Url { get; set; }

        /// <summary>
        /// CORS allowed origins
        /// </summary>
        [Required]
        public List<string> AllowedOrigins { get; set; }

        /// <summary>
        /// Maximum page size that can be returned by the API
        /// </summary>
        [Required]
        public int MaxPageSize { get; set; }
    }

    public class UrlFragment
    {
        /// <summary>
        /// Raw url
        /// </summary>
        [Required]
        public string RawUrl { get; set; }

        /// <summary>
        /// Raw url as Uri
        /// </summary>
        public Uri Url => new Uri(RawUrl);

        /// <summary>
        /// Append path to url
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Path(string path) => $"{RawUrl.TrimEnd('/')}/{path}";
    }
}