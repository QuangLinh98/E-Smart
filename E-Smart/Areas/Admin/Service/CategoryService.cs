using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Admin.Service
{
	public class CategoryService : ICategoryRepository
	{
		private readonly DatabaseContext _dbContext;
		public CategoryService(DatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task AddCategory(Category category)
		{
			await _dbContext.Categories.AddAsync(category);
			await _dbContext.SaveChangesAsync();
		}

		public async Task<IEnumerable<Category>> GetAllCategory()
		{
			var categories = await _dbContext.Categories.ToListAsync();
			return categories;
		}

        public async Task<Category> GetCategory(int id)
        {
			var cateID = await _dbContext.Categories.FindAsync(id);
			return cateID;
        }

        public async Task UpdateCategory(Category category)
		{
			_dbContext.Categories.Update(category);
            await _dbContext.SaveChangesAsync();
        }
	}
}
