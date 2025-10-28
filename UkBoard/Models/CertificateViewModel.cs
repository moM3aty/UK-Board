using System.ComponentModel.DataAnnotations;

namespace UkBoard.Models
{
    public class CertificateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Certificate ID is required")]
        [Display(Name = "Certificate ID")]
        public string CertificateId { get; set; }

        public string? ExistingImagePath { get; set; }


        [Display(Name = "Certificate Image")]
        public IFormFile? CertificateImage { get; set; }
    }
}

