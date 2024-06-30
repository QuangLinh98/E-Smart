using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Admin.Repository
{
	public interface IOrderDetailRepository
	{
		Task<IEnumerable<OrderDetail>> GetAllOrderDetail();
		Task<OrderDetail> GetOneOrderDetail(int id);
		Task DeleteOrderDetail(int id);
	}
}
