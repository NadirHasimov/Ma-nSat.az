using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarController.Models
{
    public class UserModel
    {
        [Required(ErrorMessage = "Soyadınızı daxil edin!")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Adınızı daxil edin!")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Username-i daxil edin!")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Email adresinizi daxil edin!")]
        [EmailAddress(ErrorMessage = "Bu email düzgün deyil! Yenidən daxil edin!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Parolu daxil edin!")]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Parol minimum 5 simvoldan ibarət olmalıdı!")]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Telefon nömrəvizi daxil edin!")]
        public string PhoneNumber { get; set; }
    }
}