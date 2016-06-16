namespace Wibci.Azure.AppService.Auth.Models
{
    public class CreateUserRequest
    {
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string RoleName { get; set; }
    }
}