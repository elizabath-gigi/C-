﻿using System.ComponentModel.DataAnnotations;

namespace UserManagement.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? NameHindi { get; set; }
       
    }
}
