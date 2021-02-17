namespace App.Domain.Processes.Users.Dtos.Email
{
    public class AddUserEmailDto
    {
        /// <summary>
        /// Username
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Newly generated password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Reset password url
        /// </summary>
        public string ResetPasswordUrl { get; set; }
    }
}