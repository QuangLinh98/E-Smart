using E_Smart.Areas.Client.Models;

namespace E_Smart.Areas.Client.Repository
{
	public interface IUserRepository
	{
		Task<IEnumerable<User>> GetAllUser();
		Task<User> GetOneUser(int id);
		Task AddUser(User user);
		Task UpdateUser(User user);
		Task DeleteUser(int id);

		Task<bool> ComfirmEmail(string email, string token);
		Task<bool> ForgotPassword(string email);
		Task<bool> ResetPassword(string email, string token,string newPassword);
	}
}
