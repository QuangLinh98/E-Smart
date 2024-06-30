using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Smart.Areas.Admin.Models
{
    public class Customer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CustomerId { get; set; }

        [Required]
        [StringLength(50,MinimumLength = 2 ,ErrorMessage = "Please name must be between [2-50] character!")]
        public string Customer_name { get; set; }

        [Required]
        public int Customer_code { get; set; }

        [Required]
        [EmailAddress]
        public string Customer_email { get; set; }

        [Required]
        [StringLength(200)]
        public string Customer_address { get; set; }

        [Required]
        [Phone]
        public string Customer_phone { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }
}
