using Signals.Core.Processing.Input.Http;
using Signals.Core.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Core.Web.Execution.CustomContentHandlers
{
    /// <summary>
    /// Handler for custom urls that differ from convention
    /// </summary>
    public interface ICustomUrlHandler
    {
        /// <summary>
        /// Put content into http response
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        MiddlewareResult RenderContent(IHttpContextWrapper context);
    }
}
