using System.ComponentModel.DataAnnotations;

namespace Wibci.Azure.AppService.Auth.DataObjects
{
    public enum AuthApplicationTypes
    {
        JavaScript = 0,
        NativeConfidential = 1
    };

    public class AuthApiClient
    {
        public bool Active { get; set; }

        [MaxLength(100)]
        public string AllowedOrigin { get; set; }

        public AuthApplicationTypes ApplicationType { get; set; }

        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public int RefreshTokenLifeTime { get; set; }

        [Required]
        public string Secret { get; set; }
    }
}