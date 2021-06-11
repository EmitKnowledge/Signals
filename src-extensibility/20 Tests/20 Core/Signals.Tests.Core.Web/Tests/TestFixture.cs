using Signals.Core.Common.Instance;
using Signals.Core.Common.Serialization;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Tests.Core.Web.Tests
{
    public class TestFixture : IDisposable
    {
        string url = "http://localhost:5000/";
        private CancellationTokenSource CTS;

        public void Init()
        {
            CTS = new CancellationTokenSource();
            Task.Run(() =>
            {
                Program.Main(url);
            }, CTS.Token);
        }

        public void Dispose()
        {
            CTS.Cancel();
        }

        protected T MockRequest<T>(string path, object body = null)
        {
            var client = new WebClient();
            client.BaseAddress = url;

            string response;
            if (body.IsNull())
            {
                response = client.DownloadString(path);
            }
            else
            {
                response = client.UploadString(path, body.SerializeJson());
            }

            return response.Deserialize<T>();
        }
    }
}
