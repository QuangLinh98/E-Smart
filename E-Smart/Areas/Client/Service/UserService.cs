using System.Security.Cryptography;
using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Repository;
using E_Smart.Data;
using E_Smart.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using static System.Net.WebRequestMethods;

namespace E_Smart.Areas.Client.Service
{
	public class UserService : IUserRepository
	{
		private readonly DatabaseContext _dbContext;
		private readonly IDistributedCache _cache;
		private readonly EmailService _emailService;
		private readonly IPasswordHasher<User> _passwordHasher; 
		public UserService(DatabaseContext dbContext, IDistributedCache cache, EmailService emailService, IPasswordHasher<User> passwordHasher)
		{
			_dbContext = dbContext;
			_cache = cache;
			_emailService = emailService;
			_passwordHasher = passwordHasher;
		}
		public async Task AddUser(User user)
		{
			await _dbContext.AddAsync(user);
			await _dbContext.SaveChangesAsync();
		}

        public async Task<bool> ComfirmEmail(string email, string token)
        {
           var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
			if (user == null)
			{
				string cachedToken = await _cache.GetStringAsync($"EmailComfirmation:{email}");
                if (cachedToken == token)
                {
					user.EmailComfirmed = true;
					await _dbContext.SaveChangesAsync();
					await _cache.RemoveAsync($"EmailComfirmation:{email}");   // Sau khi user xác nhận email thành công sẽ xóa trong cache(Redis) giảm thiểu sự dư thừa 
					return true;
                }
            }
			return false;
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

        public async Task<bool> ForgotPassword(string email)
        {
			var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
			if (user != null)
			{
				string otp = GenerateOtp();
				user.ResetPasswordToken = otp;
				user.ResetPasswordTokenExpiration = DateTime.UtcNow.AddMinutes(10);
				await _dbContext.SaveChangesAsync();

				await _cache.SetStringAsync($"ResetPassword:{email}", otp);
				SendResetPasswordEmail(email, otp);
				return true;

            }
			return false;
        }

        public async Task SendResetPasswordEmail(string email, string token)
        {
            var resetLink = $"https://your-app.com/reset-password?token={token}"; // Thay đổi đường dẫn theo ứng dụng của bạn
            var subject = "Reset Password";
            var body = $"<h1>ResetPassưord</h1><p>Please resetpassword your password by to click link here:</p><a href=\"{resetLink}\">Đặt lại mật khẩu</a>";

            // Gọi phương thức SendEmail để gửi email
            await _emailService.SendEmail(email, subject, body);
        }

        public async Task SendOtpEmail(string email, string otp)
        {
           
            var subject = "Your OTP Code";
            var body = $"<h1>OTP Verification</h1><p>Your OTP code is:</p><h2>{otp}</h2><p>Please use this code to complete your verification. The code is valid for 10 minutes.</p>";

            // Call the SendEmail method from EmailService to send the confirmation email
            await _emailService.SendEmail(email, subject, body);
        }

        public string GenerateOtp(int length = 6)
        {
            // Tạo OTP ngẫu nhiên với độ dài xác định (mặc định là 6 chữ số)
            var random = new Random();
            return random.Next(0, 999999).ToString("D6"); // Tạo OTP gồm 6 chữ số
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

        public async Task<bool> ResetPassword(string email, string token, string newPassword)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(u =>  u.Email == email);
            if (user != null && user.ResetPasswordToken == token && user.ResetPasswordTokenExpiration > DateTime.UtcNow)
            {
				user.Password = _passwordHasher.HashPassword(user, newPassword);
				user.ResetPasswordToken = null;
				user.ResetPasswordTokenExpiration = DateTime.MinValue;
				await _dbContext.SaveChangesAsync();
				await _cache.RemoveAsync($"ResetPassword: {email}");
				return true;
            }
			return false;
        }

        public async Task UpdateUser(User user)
		{
			_dbContext.Users.Update(user);
			await _dbContext.SaveChangesAsync();
		}
	}
}
