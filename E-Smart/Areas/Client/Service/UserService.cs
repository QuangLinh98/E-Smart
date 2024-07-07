using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Repository;
using E_Smart.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Client.Service
{
	public class UserService : IUserRepository
	{
		private readonly DatabaseContext _dbContext;
		public UserService(DatabaseContext dbContext)
		{
			_dbContext = dbContext;
		}
		public async Task AddUser(User user)
		{
			await _dbContext.AddAsync(user);
			await _dbContext.SaveChangesAsync();
		}

		public async Task DeleteUser(int id)
		{
			var user = await GetOneUser(id);
            if (user != null)
            {
				_dbContext.Users.Remove(user);
				await _dbContext.SaveChangesAsync();
            }
        }

		public async Task<IEnumerable<User>> GetAllUser()
		{
			var users = await _dbContext.Users.ToListAsync();
			return users;
		}

		public async Task<User> GetOneUser(int id)
		{
			var user = await _dbContext.Users.FindAsync(id);
			return user;
		}

		public async Task UpdateUser(User user)
		{
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
		}
	}
}
