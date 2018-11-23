using System;
using System.Collections.Generic;
using System.Text;

namespace Signals.Core.Business.Recurring
{
    public interface ISyncLogProvider
    {
        /// <summary>
        /// Insert sync task result in database
        /// </summary>
        /// <param name="log"></param>
        void CreateLog(SyncTaskLog log);

        /// <summary>
        /// Get last sync execution results
        /// </summary>
        /// <param name="take"></param>
        /// <returns></returns>
        List<SyncTaskLog> Last(int take);

        /// <summary>
        /// Get last sync execution result
        /// </summary>
        /// <returns></returns>
        SyncTaskLog Last();
    }
}
