using System.ComponentModel.DataAnnotations;

namespace E_Smart.Areas.Client.Models.ViewModels
{
    public class VerifyOtpViewModel
    {
        [Required(ErrorMessage = "OTP is required.")]
        public string Otp { get; set; }
    }
}
