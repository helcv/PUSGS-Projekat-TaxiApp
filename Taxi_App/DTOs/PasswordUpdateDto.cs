using System;
using System.ComponentModel.DataAnnotations;

namespace Taxi_App.DTOs
{
    public class PasswordUpdateDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
