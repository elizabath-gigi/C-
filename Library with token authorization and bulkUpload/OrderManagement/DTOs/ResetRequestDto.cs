using System.ComponentModel.DataAnnotations;

namespace OrderManagement.DTOs
{
    public class ResetRequestDto
    {
        
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }
    }
}
