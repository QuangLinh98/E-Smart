using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Client.Models
{
    public class CartItem
    {
 
        public int Product_Id { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public decimal Total {
            get { return Quantity * Price; }
        
        }

        // Đầu tiên cho Cart = "" . Vì khi cusotmer order product thì CartItem sẽ tự động tăng item lên
        public CartItem() { }


        //KHi Customer add product thì CartItem tạo Item trong giỏ hàng
		public CartItem(Product product) 
        {
            Product_Id = product.ProductId;
            Name = product.Product_name;
            Price = product.Product_price;
            Quantity = 1;    // Cho quantity mỗi lần thêm mặc định = 1, 
            Image = product.Product_imagePaths;
        }

        
	}
}
