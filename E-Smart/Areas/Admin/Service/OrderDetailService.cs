using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Admin.Service
{
	public class OrderDetailService : IOrderDetailRepository
	{
		private readonly DatabaseContext _dbContext;
		public OrderDetailService(DatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task DeleteOrderDetail(int id)
		{
			var existingOrderDetail = await GetOneOrderDetail(id);
			if (existingOrderDetail != null)
			{
				_dbContext.OrderDetails.Remove(existingOrderDetail);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<OrderDetail>> GetAllOrderDetail()
		{
			var orderDetails = await _dbContext.OrderDetails.Include(od => od.Product).ToListAsync();
			return orderDetails;
		}


		public async Task<OrderDetail> GetOneOrderDetail(int id)
		{
			var existingOrderDetail = await _dbContext.OrderDetails.FindAsync(id);
			return existingOrderDetail;
		}
	}
}
