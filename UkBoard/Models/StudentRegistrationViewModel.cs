using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace UkBoard.Models
{
    public class StudentRegistrationViewModel
    {
        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Full Name as in Passport / ID")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone Number (with Country Code)")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Please select a major")]
        [Display(Name = "Desired Major")]
        public string Major { get; set; }

        [Required(ErrorMessage = "Qualification Certificate is required")]
        [Display(Name = "Qualification Certificate")]
        public IFormFile QualificationImage { get; set; }

        [Required(ErrorMessage = "Identity Document is required")]
        [Display(Name = "Identity Document")]
        public IFormFile IdentityImage { get; set; }

        [Required(ErrorMessage = "Personal Photo is required")]
        [Display(Name = "Personal Photo")]
        public IFormFile PersonalPhoto { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree to the Transparency Charter")]
        public bool IsTransparencyCharterAgreed { get; set; }
    }
}