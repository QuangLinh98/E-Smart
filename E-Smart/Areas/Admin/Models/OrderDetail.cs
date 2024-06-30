using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Admin.Models
{
    public class OrderDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailId { get; set; }
        public int Order_Id { get; set; }
        public int Product_Id { get; set; }
        public decimal Unit_Price { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
        public Order? Order { get; set; }
    }
}
