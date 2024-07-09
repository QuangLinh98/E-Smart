using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Smart.Areas.Admin.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public DateTime Order_date { get; set; }
        public string Order_description { get; set; }
		public string CustomerPhone { get; set; }

        public string Status { get; set; } = "Pending";

		// Phương thức cập nhật trạng thái đơn hàng khi admin duyệt đơn
		public void ApproveOrder()
		{
			Status = "Approved";
		}

		[ForeignKey("CustomerId")]
		public Customer? Customer { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }


    }
}
