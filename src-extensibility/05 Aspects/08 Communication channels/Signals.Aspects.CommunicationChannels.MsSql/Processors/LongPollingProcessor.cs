using Signals.Aspects.CommunicationChannels.MsSql.Configurations;
using Signals.Aspects.CommunicationChannels.MsSql.Processors.Base;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Aspects.CommunicationChannels.MsSql.Processors
{
    internal class LongPollingProcessor : BaseProcessor
    {
        private Task LongPollingLoop { get; set; }

        private CancellationTokenSource LongPollingLoopCancellationTokenSource { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public LongPollingProcessor(MsSqlMessageChannel channel)
            : base(channel)
        {
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public override void Dispose()
        {
            LongPollingLoopCancellationTokenSource?.Cancel();
            while (LongPollingLoop?.IsCanceled == false)
                Thread.Sleep(100);
            LongPollingLoop?.Dispose();
        }

        /// <summary>
        /// Init processor
        /// </summary>
        public override void Init()
        {
            // Init only if it hasn't been set before
            if (LongPollingLoop == null && Channel.Configuration.MessageListeningStrategy == MessageListeningStrategy.LongPolling)
            {
                LongPollingLoopCancellationTokenSource = new CancellationTokenSource();
                LongPollingLoop = Task.Run(() =>
                {
                    while (true)
                    {
                        Thread.Sleep(Channel.Configuration.LongPollingTimeout);

                        if (LongPollingLoopCancellationTokenSource.Token.IsCancellationRequested)
                            LongPollingLoopCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        var unprocessedTasks = Channel.GetAllUnprocessedTasks();
                        foreach (var task in unprocessedTasks)
                        {
                            if (LongPollingLoopCancellationTokenSource.Token.IsCancellationRequested)
                                LongPollingLoopCancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var queue = task?.MessageQueue;

                            if (!string.IsNullOrEmpty(queue) && Channel.Subscriptions.ContainsKey(queue))
                            {
                                var message = Channel.GetAndLockSystemMessageById(task.Id);

                                if (message != null)
                                {
                                    Channel.Subscriptions[queue](message.MessagePayload);
                                    Channel.MarkSystemMessageAsProcessed(message.Id);
                                }
                            }
                        }
                    }
                }, LongPollingLoopCancellationTokenSource.Token);
            }
        }
    }
}
