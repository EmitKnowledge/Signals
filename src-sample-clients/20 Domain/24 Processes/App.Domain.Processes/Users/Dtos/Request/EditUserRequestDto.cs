using App.Domain.Entities.Users;
using Ganss.XSS;
using Signals.Core.Common.Instance;
using Signals.Core.Processing.Input;

namespace App.Domain.Processes.Users.Dtos.Request
{
    public class EditUserRequestDto : IDtoData
    {
        /// <summary>
        /// Represents the user id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the full name of the user
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the email of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Represents the username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User type
        /// </summary>
        public UserType Type { get; set; }

        public void Sanitize(HtmlSanitizer sanitizer)
        {
            Name = Name.IsNullOrEmpty() ? Name : sanitizer.Sanitize(Name);
            Email = Email.IsNullOrEmpty() ? Email : sanitizer.Sanitize(Email);
            Username = Name.IsNullOrEmpty() ? Username : sanitizer.Sanitize(Username);
        }
    }
}