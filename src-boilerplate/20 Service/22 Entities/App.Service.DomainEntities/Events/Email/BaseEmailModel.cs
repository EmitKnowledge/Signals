namespace App.Service.DomainEntities.Events.Email

{
    /// <summary>
    /// Represents base email model with web domain
    /// </summary>
    public abstract class BaseEmailModel
    {
        /// <summary>
        /// Base web domain
        /// </summary>
        public string Domain { get; set; }
    }
}