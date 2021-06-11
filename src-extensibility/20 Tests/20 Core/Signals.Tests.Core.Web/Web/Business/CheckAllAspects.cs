using Signals.Core.Common.Instance;
using Signals.Core.Processes.Business;
using Signals.Core.Processing.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Signals.Tests.Core.Web.Web.Business
{
    public class CheckAllAspects : BusinessProcess<VoidResult>
    {
        /// <summary>
        /// Auth process
        /// </summary>
        /// <returns></returns>
        public override VoidResult Auth()
        {
            return Ok();
        }

        /// <summary>
        /// Validate process
        /// </summary>
        /// <returns></returns>
        public override VoidResult Validate()
        {
            return Ok();
        }

        /// <summary>
        /// Handle process
        /// </summary>
        /// <returns></returns>
        public override VoidResult Handle()
        {
            Context.Cache.Set("key", "value");
            if (Context.Cache.Get<string>("key") != "value") throw new Exception("Cache failure");

            Context.Storage.Store("/", "test-file", Encoding.UTF8.GetBytes("content"));
            var filesEqual = new StreamReader(Context.Storage.Get("/", "test-file")).ReadToEnd() == "content";
            Context.Storage.Remove("/", "test-file");
            if (!filesEqual) throw new Exception("Storage failure");

            Context.Benchmarker.StartEpic("epic");
            Context.Benchmarker.Bench("point");
            Context.Benchmarker.FlushEpic();
            if (!Context.Benchmarker.GetEpicReport("epic", DateTime.Today.AddDays(-1)).EpicReports.Any()) throw new Exception("Benchmarking failure");

            if (Context.LocalizationProvider.Get("my_translation")?.Value != "translation") throw new Exception("Localization failure");

            var principal = new ClaimsPrincipal();
            principal.AddIdentity(new ClaimsIdentity());
            Context.Authentication.Login(principal);
            Context.Authentication.SetCurrentUser(new User { Name = "user name" });
            if (Context.Authentication.GetCurrentUser<User>().Name != "user name") throw new Exception("Authentication failure");

            return Ok();
        }

        public class User
        {
            public string Name { get; set; }
        }
    }
}
