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
