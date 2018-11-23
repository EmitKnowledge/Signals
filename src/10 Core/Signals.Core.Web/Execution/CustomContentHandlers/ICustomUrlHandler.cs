using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Execution.CustomContentHandlers
{
    public interface ICustomUrlHandler
    {
        MiddlewareResult RenderContent(IHttpContextWrapper context);
    }
}
