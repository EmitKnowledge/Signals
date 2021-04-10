namespace Signals.Core.Processing.Exceptions
{
    /// <summary>
    /// Application error information
    /// </summary>
    public interface IErrorInfo
    {
        /// <summary>
        /// System fault message
        /// </summary>
        string FaultMessage { get; set; }

        /// <summary>
        /// User visible error message. What the user see
        /// </summary>
        string UserVisibleMessage { get; }
    }
}