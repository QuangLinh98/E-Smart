using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Admin.Repository
{
	public interface IOrderRepository
	{
		Task<IEnumerable<Order>> GetAllOrder();
		Task<Order> GetOneOrder(int id);
		Task DeleteOrder(int id);
	}
}
