namespace E_Smart.Areas.Client.Models.ViewModels
{
	public class CartItemViewModel
	{
		//Hiển thị tất cả các item trong cart 
		public List<CartItem> CartItems { get; set; }
		public decimal GrandTotal { get; set; }    //Tổng tiền
	}
}
