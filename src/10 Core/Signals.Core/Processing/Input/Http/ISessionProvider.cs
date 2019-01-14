namespace Signals.Core.Processing.Input.Http
{
    /// <summary>
    /// Wrapper around real session
    /// </summary>
    public interface ISessionProvider
    {
        /// <summary>
        /// Sets session value
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void Set(string name, object value);

        /// <summary>
        /// Removes session value
        /// </summary>
        /// <param name="name"></param>
        void Remove(string name);

        /// <summary>
        /// Gets session value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T Get<T>(string name) where T : class;
    }
}
