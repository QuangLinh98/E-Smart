using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Admin.Repository
{
	public interface ICustomerRepository
	{
		Task<IEnumerable<Customer>> GetAllCustomer();
		Task<Customer> GetOneCustomer(int id);
		Task AddCustomer(Customer customer);
		Task DeleteCustomer(int id);
	}
}
