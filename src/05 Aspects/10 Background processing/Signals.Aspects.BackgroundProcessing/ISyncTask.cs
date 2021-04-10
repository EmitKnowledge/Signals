namespace Signals.Aspects.BackgroundProcessing
{
    /// <summary>
    /// Task that will run in background
    /// </summary>
    public interface ISyncTask
    {
        /// <summary>
        /// Task execution handler
        /// </summary>
        void Execute();
    }
}
