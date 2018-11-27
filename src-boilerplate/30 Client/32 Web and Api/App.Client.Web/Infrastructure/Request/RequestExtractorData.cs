using System;
using System.Runtime.Serialization;

namespace App.Client.Web.Infrastructure.Request
{
    [Serializable]
    [DataContract]
    public class RequestExtractorData
    {
        public RequestExtractorData()
        {
            RecordedOn = DateTime.UtcNow;
        }

        /// <summary>
        /// Username if available in the context
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Record if the request is authenticated
        /// </summary>
        [DataMember]
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Resource path that the entity try to access
        /// </summary>
        [DataMember]
        public string RawUri { get; set; }

        /// <summary>
        /// Raw url without query strings
        /// </summary>
        [DataMember]
        public string BaseUri { get; set; }

        /// <summary>
        /// Query strings used in the raw url
        /// </summary>
        [DataMember]
        public string QueryString { get; set; }

        /// <summary>
        /// HttpMethod of the request
        /// </summary>
        [DataMember]
        public string HttpMethod { get; set; }

        /// <summary>
        /// Ip of the entity accessing the resource
        /// </summary>
        [DataMember]
        public string IpAddress { get; set; }

        /// <summary>
        /// Resource that reffered to our resource
        /// </summary>
        [DataMember]
        public string Refferer { get; set; }

        /// <summary>
        /// UserAgent data of the entity
        /// </summary>
        [DataMember]
        public string UserAgent { get; set; }

        /// <summary>
        /// Browser data of the entity
        /// </summary>
        [DataMember]
        public string Browser { get; set; }

        /// <summary>
        /// Browser version data of the entity
        /// </summary>
        [DataMember]
        public string BrowserVersion { get; set; }

        /// <summary>
        /// Platform data of the entity
        /// </summary>
        [DataMember]
        public string Platform { get; set; }

        /// <summary>
        /// Indicate if the request was sent by a crawler
        /// </summary>
        [DataMember]
        public bool IsCrawler { get; set; }

        /// <summary>
        /// Identify http/s
        /// </summary>
        [DataMember]
        public bool IsSecureResource { get; set; }

        /// <summary>
        /// When was the tracking record created
        /// </summary>
        [DataMember]
        public DateTime RecordedOn { get; set; }
    }
}