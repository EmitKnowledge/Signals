using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Signals.Aspects.CommunicationChannels.MsSql.Processors.Base
{
    internal abstract class BaseProcessor : IDisposable
    {
        protected readonly MsSqlMessageChannel Channel;

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="channel"></param>
        public BaseProcessor(MsSqlMessageChannel channel)
        {
            Channel = channel;
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Init processor
        /// </summary>
        public abstract void Init();
    }
}
