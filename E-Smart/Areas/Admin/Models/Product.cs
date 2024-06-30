using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Smart.Areas.Admin.Models
{
	public class Product
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int ProductId { get; set; }

		[Required]
		[StringLength(50,MinimumLength =2,ErrorMessage = "Please name must be between [2-50] character!")]
		public string Product_name { get; set; }

		[Required]
		[Column(TypeName ="decimal(10,2)")]
		public decimal Product_price { get; set; }

		[Required]
		public int Product_quantity { get; set; }

		
		public string? Product_imagePaths { get; set; }  // Store image paths as a JSON string


        [NotMapped]
        public List<IFormFile>? Product_imageFiles { get; set; } // For multiple file uploads

        [Required]
		[DataType(DataType.MultilineText)]
		public string Product_description { get; set; }
		public int Category_Id { get; set; }
		public Category? Category { get; set; }
		public ICollection<OrderDetail>? OrderDetails { get; set; }
	}
}
