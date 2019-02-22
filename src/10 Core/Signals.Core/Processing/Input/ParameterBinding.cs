namespace Signals.Core.Processing.Input
{
    /// <summary>
    /// Represents the parameter binding method when deserializing the request data
    /// </summary>
    public enum ParameterBinding
    {
        /// <summary>
        /// Represents parameter binding from Uri
        /// </summary>
        FromUri,

        /// <summary>
        /// Represents parameter binding from Body
        /// </summary>
        FromBody,

        /// <summary>
        /// Represents parameter binding from From
        /// </summary>
        FromForm
    }
}