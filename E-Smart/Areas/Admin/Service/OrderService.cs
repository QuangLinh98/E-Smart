using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Admin.Service
{
	public class OrderService : IOrderRepository
	{
		private readonly DatabaseContext _dbContext;
		public OrderService(DatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task DeleteOrder(int id)
		{
			var existingOrder = await GetOneOrder(id);
			if (existingOrder != null)
			{
				_dbContext.Orders.Remove(existingOrder);
				await _dbContext.SaveChangesAsync();
			}
		}


		public async Task<IEnumerable<Order>> GetAllOrder()
		{
			var orders = await _dbContext.Orders.ToListAsync();
			return orders; ;
		}

		
		public async Task<Order> GetOneOrder(int id)
		{
			var existingOrder = await _dbContext.Orders.FindAsync(id);
			return existingOrder;
		}
	}
}
