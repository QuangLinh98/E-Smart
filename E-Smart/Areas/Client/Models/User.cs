using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Smart.Areas.Client.Models
{
	public class User
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int Id { get; set; }

		[Required]
		[StringLength(250, ErrorMessage = "User Name cannot be longer than 250 characters")]
		public string UserName { get; set; }


		[Required]
		[StringLength(100, ErrorMessage = "Password cannot be longer than 100 characters")]
		public string Password { get; set; }


		[Required(ErrorMessage = "Email is required")]
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		[StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters")]
		public string Email { get; set; }

		[StringLength(200, ErrorMessage = "Address cannot be longer than 200 characters")]
		public string Address { get; set; }

		[Phone(ErrorMessage = "Invalid Phone Number")]
		[StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters")]
		public string Phone { get; set; }

        public bool EmailComfirmed { get; set; }

        public string? EmailComfirmationToken { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime ResetPasswordTokenExpiration { get; set; }
    }
}
