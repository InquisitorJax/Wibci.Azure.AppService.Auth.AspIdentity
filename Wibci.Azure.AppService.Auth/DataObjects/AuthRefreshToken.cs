using System;
using System.ComponentModel.DataAnnotations;

namespace Wibci.Azure.AppService.Auth.DataObjects
{
    public class AuthRefreshToken
    {
        [Required]
        [MaxLength(50)]
        public string ClientId { get; set; }

        public DateTime ExpiresUtc { get; set; }

        [Key]
        public string Id { get; set; }

        public DateTime IssuedUtc { get; set; }

        [Required]
        public string ProtectedTicket { get; set; }

        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }
    }
}