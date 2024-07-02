using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Admin.Repository
{
	public interface IOrderRepository
	{
		Task<IEnumerable<Order>> GetAllOrder();
		Task<Order> GetOneOrder(int id);
		Task DeleteOrder(int id);
		Task UpdateOrderStatus(int id, string status);
		Task<IEnumerable<OrderDetail>> GetOrderDetails(int orderID);   // This method is get all OrderDetail by OrderID
	}
}
