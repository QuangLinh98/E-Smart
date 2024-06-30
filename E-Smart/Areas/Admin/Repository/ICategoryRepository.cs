using E_Smart.Areas.Admin.Models;

namespace E_Smart.Areas.Admin.Repository
{
	public interface ICategoryRepository
	{
		Task<IEnumerable<Category>> GetAllCategory();
		Task<Category> GetCategory(int id);
		Task AddCategory(Category category);
		Task UpdateCategory(Category category);
	}
}
