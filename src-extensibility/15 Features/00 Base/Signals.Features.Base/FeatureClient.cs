using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using Signals.Features.Base.Configurations.Feature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Signals.Features.Base
{
    /// <summary>
    /// Feature client proxy
    /// </summary>
    public class FeatureClient : DispatchProxy, IFeature
    {
        private BaseFeatureConfiguration _featureConfiguration;
        private object _feature;

        private object objectEmpty = new object();
        private Dictionary<string, List<string>> methodParamNames = new Dictionary<string, List<string>>();

        /// <summary>
        /// Invoke proxy creation
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (_featureConfiguration?.MicroServiceConfiguration?.SendRequestsToMicroService == true)
            {
                var payload = new Dictionary<string, object>();

                if (!methodParamNames.ContainsKey(targetMethod.Name))
                    methodParamNames.Add(targetMethod.Name, targetMethod.GetParameters().Select(x => x.Name).ToList());

                var @params = methodParamNames[targetMethod.Name];

                int i = 0;
                foreach (var param in @params)
                {
                    payload.Add(param, args[i++]);
                }

                return SendRequestToMicroService(targetMethod.Name, targetMethod.ReturnType, payload);
            }
            else
            {
                return targetMethod.Invoke(_feature, args);
            }
        }

        internal static object CreateProxy(object decorated, Type interfaceType, BaseFeatureConfiguration featureConfiguration)
        {
            var type = typeof(FeatureClient);
            object proxy = type
                .GetMethod("Create", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .MakeGenericMethod(interfaceType, type)
                .Invoke(null, null);

            ((FeatureClient)proxy).SetDecorated(decorated);
            ((FeatureClient)proxy).SetConfioguration(featureConfiguration);

            return proxy;
        }

        private void SetDecorated<T>(T feature)
        {
            _feature = feature;
        }

        private void SetConfioguration(BaseFeatureConfiguration featureConfiguration)
        {
            _featureConfiguration = featureConfiguration;
        }

        private object SendRequestToMicroService(string methodName, Type returnType = null, Dictionary<string, object> parameters = null)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(_featureConfiguration.MicroServiceConfiguration.Url);
            httpClient.DefaultRequestHeaders.Add(nameof(_featureConfiguration.MicroServiceConfiguration.ApiKey), _featureConfiguration.MicroServiceConfiguration.ApiKey);
            httpClient.DefaultRequestHeaders.Add(nameof(_featureConfiguration.MicroServiceConfiguration.ApiSecret), _featureConfiguration.MicroServiceConfiguration.ApiSecret);

            var body = new StringContent(parameters?.SerializeJson(), Encoding.UTF8, "application/json");
            var response = httpClient.PostAsync(methodName, body).GetAwaiter().GetResult();
            
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(response.ReasonPhrase);
            }

            if (returnType.IsNull() || returnType == typeof(void))
            {
                return objectEmpty;
            }
            else
            {
                return response?
                    .Content?
                    .ReadAsStringAsync()?
                    .GetAwaiter()
                    .GetResult()?
                    .Deserialize(returnType);
            }
        }
    }
}
