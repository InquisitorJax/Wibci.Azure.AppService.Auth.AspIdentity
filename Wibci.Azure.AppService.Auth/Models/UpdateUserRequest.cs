namespace Wibci.Azure.AppService.Auth.Models
{
    public class UpdateUserRequestDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public byte[] ProfileImage { get; set; }

        public string ProfileImageUri { get; set; }

        public string RoleName { get; set; }
    }
}