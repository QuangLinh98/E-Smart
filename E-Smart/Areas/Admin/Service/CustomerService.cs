using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Admin.Service
{
	public class CustomerService : ICustomerRepository
	{
		private readonly DatabaseContext _dbContext;
		public CustomerService(DatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddCustomer(Customer customer)
		{
			await _dbContext.Customers.AddAsync(customer);
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteCustomer(int id)
		{
			var existingCustomer = await GetOneCustomer(id);
			if (existingCustomer != null)
			{
				_dbContext.Customers.Remove(existingCustomer);
				await _dbContext.SaveChangesAsync();
			}
		}

		public async Task<IEnumerable<Customer>> GetAllCustomer()
		{
			var customers = await _dbContext.Customers.ToListAsync();
			return customers;
		}

		public async Task<Customer> GetOneCustomer(int id)
		{
			var existingCustomer = await _dbContext.Customers.FindAsync(id);
			return existingCustomer;
		}
	}
}
