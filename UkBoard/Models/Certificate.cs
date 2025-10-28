using System.ComponentModel.DataAnnotations;

namespace UkBoard.Models
{
    public class Certificate
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Certificate ID is required")]
        [Display(Name = "Certificate ID")]
        public string CertificateId { get; set; }

        [Required(ErrorMessage = "Certificate image is required")]
        public string ImagePath { get; set; }
    }
}
