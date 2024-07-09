using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
        public string Customer_code { get; set; }

        [Required]
        [EmailAddress]
        public string Customer_email { get; set; }

        [Required]
        [StringLength(200)]
        public string Customer_address { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Phone number must start with 0 and be 10 to 11 digits long.")]
        public string Customer_phone { get; set; }

        public ICollection<Order>? Orders { get; set; }

        //Tạo Contructor để thưc hiện Generate Customer_code 
        public Customer()
        {
            Customer_code = GenerateCustomerCode();
        }

        // Sử dụng biêyr thức điều kiện đê thực hiện Generate
        private string GenerateCustomerCode()
        {
            DateTime now = DateTime.Now;
            return $"C{now:yyyyMMddHHmmss}";
        }
    }
}
