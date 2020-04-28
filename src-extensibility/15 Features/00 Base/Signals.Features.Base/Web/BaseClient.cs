using Signals.Core.Common.Serialization;
using Signals.Features.Base.Configurations;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Signals.Features.Base.Web
{
    public abstract class BaseClient
    {
        private readonly IFeatureConfiguration _featureConfiguration;

        public BaseClient(IFeatureConfiguration featureConfiguration)
        {
            _featureConfiguration = featureConfiguration;
        }

        private TResult SendRequestToMicroService<TResult>(string methodName, Dictionary<string, object> parameters = null) where TResult : class
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_featureConfiguration.MicroServiceConfiguration.Url);
            httpClient.DefaultRequestHeaders.Add(nameof(_featureConfiguration.MicroServiceConfiguration.ApiKey), _featureConfiguration.MicroServiceConfiguration.ApiKey);
            httpClient.DefaultRequestHeaders.Add(nameof(_featureConfiguration.MicroServiceConfiguration.ApiSecret), _featureConfiguration.MicroServiceConfiguration.ApiSecret);

            var body = new StringContent(parameters?.SerializeJson(), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(methodName, body).GetAwaiter().GetResult();

            return response?
                .Content?
                .ReadAsStringAsync()?
                .GetAwaiter()
                .GetResult()?
                .Deserialize<TResult>();
        }

        private void SendRequestToMicroService(string methodName, Dictionary<string, object> parameters = null)
        {
            SendRequestToMicroService<object>(methodName, parameters);
        }

        protected void Process(string methodName, Dictionary<string, object> parameters, Action action)
        {
            Process(methodName, parameters, () => { action(); return new object(); });
        }

        protected TResult Process<TResult>(string methodName, Dictionary<string, object> parameters, Func<TResult> action) where TResult : class
        {
            if (_featureConfiguration.MicroServiceConfiguration?.SendRequestsToMicroService == true)
                return SendRequestToMicroService<TResult>(methodName, parameters);
            else
                return action();
        }
    }
}
