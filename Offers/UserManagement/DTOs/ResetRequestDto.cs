using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class ResetRequestDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? OldPassword { get; set; }
        [Required]
        public string? NewPassword { get; set; }
    }
}
