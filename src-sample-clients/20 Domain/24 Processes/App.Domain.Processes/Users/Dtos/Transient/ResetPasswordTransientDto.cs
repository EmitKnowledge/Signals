namespace App.Domain.Processes.Users.Dtos.Transient
{
    public class ResetPasswordTransientDto
    {
        /// <summary>
        /// Id of the user
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// New generated password
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// Username of the user
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Email of the user
        /// </summary>
        public string Email { get; set; }
    }
}