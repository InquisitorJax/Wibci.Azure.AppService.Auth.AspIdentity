namespace Wibci.Azure.AppService.Auth.Models
{
    public class LoginRequest
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }

    //TODO: Validation
}