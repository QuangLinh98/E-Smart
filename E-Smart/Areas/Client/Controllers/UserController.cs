using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Repository;
using E_Smart.Areas.Client.Service;
using E_Smart.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace E_Smart.Areas.Client.Controllers
{
	[Area("Client")]
	public class UserController : Controller
	{
		private readonly UserService _userService;
		private readonly DatabaseContext _dbContext;
		private readonly IPasswordHasher<User> _passwordHasher;
		private readonly IDistributedCache _cache;
		public UserController(UserService userService, DatabaseContext dbContext, IPasswordHasher<User> passwordHasher, IDistributedCache cache)
		{
			_userService = userService;
			_dbContext = dbContext;
			_passwordHasher = passwordHasher;
			_cache = cache;
		}

		public async Task<IActionResult> Index()
		{
			var users = await _userService.GetAllUser();
			return View(users);
		}

		public IActionResult Create()
		{
			return View();
		}

		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Register(User user)
		{
			if (ModelState.IsValid)
			{
				//Kiểm tra email có tồn tại hay không 
				var ExistingEmail = await _dbContext.Users.AnyAsync(u => u.Email == user.Email);
				if (ExistingEmail == null)
				{
					ModelState.AddModelError("Email", "Email already exist");
					return View(user);
				}

				user.Password = _passwordHasher.HashPassword(user, user.Password);

				await _userService.AddUser(user);
				string otp = _userService.GenerateOtp();
				await _userService.SendOtpEmail(user.Email, otp);

				// Lưu trữ OTP trong bộ đệm với thời gian hết hạn (ví dụ: 10 phút)
				await _cache.SetStringAsync($"Otp:{user.Email}", otp, new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
				});

				// Lưu trữ email in TempData
				TempData["Email"] = user.Email;

				TempData["Message"] = "Registration successful! Please check your email to confirm your account.";
				return RedirectToAction("VerifyOtp", new { email = user.Email });
			}
			return View(user);
		}

		public IActionResult VerifyOtp()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> VerifyOtp(string otp)
		{
			var email = TempData["Email"]?.ToString();
			if (email == null)
			{
				ModelState.AddModelError("", "OTP verification failed. Please try again.");
				return View();
			}

			var cachedOtp = await _cache.GetStringAsync($"Otp:{email}");
			if (cachedOtp == null)
			{
				ModelState.AddModelError("", "OTP has expired. Please request a new one.");
				return View();
			}

			if (otp == cachedOtp)
			{
				//OTP chính xác, đánh dấu email của người dùng là đã xác nhận
				var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
				if (user != null)
				{
					user.EmailComfirmed = true;
					await _dbContext.SaveChangesAsync();

					TempData["Message"] = "Email confirmed successfully!";
					return RedirectToAction("Login");
				}
			}
			else
			{
				ModelState.AddModelError("", "Invalid OTP. Please try again.");
			}

			return View();
		}

		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(User user, string email, string password)
		{
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				ModelState.AddModelError("", "Email and Password are required.");
				return View();
			}

			//Kiểm tra người dùng có lưu trữ trong Cache hay không
			var cachedUser = await _cache.GetStringAsync($"User:{email}");
			if (cachedUser != null)
			{
				// Người dùng đã có trong cache
				user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(cachedUser);
			}
			else
			{
				// Người dùng chưa có trong cache, lấy từ cơ sở dữ liệu
				user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
				if (user == null)
				{
					ModelState.AddModelError("", "Invalid login attempt.");
					return View();
				}

				//Lưu trữ thông tin người dùng vào Cache với thời gian hết hạn 
				cachedUser = Newtonsoft.Json.JsonConvert.SerializeObject(user);
				await _cache.SetStringAsync($"User:{email}", cachedUser, new DistributedCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(60)

				});
			}

			//Kiểm tra mật khẩu
			var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
			if (passwordVerificationResult != PasswordVerificationResult.Success)
			{
				ModelState.AddModelError("", "Invalid login attempt.");
				return View();
			}

			//Kiểm tra email đã được xác nhận chưa
			if (!user.EmailComfirmed)
			{
				ModelState.AddModelError("", "Please confirm your email before logging in.");
				return View();
			}

			//Lưu thông tin người dùng vào session
			HttpContext.Session.SetString("Id", user.Id.ToString());
			HttpContext.Session.SetString("UserName", user.UserName.ToString());

			TempData["Message"] = "Login successful!";
			return Redirect("~/Home/Index");
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			HttpContext.Session.Clear();
			return Redirect("~/Home/Index");
		}

		public async Task<IActionResult> ForgotPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> ForgotPassword(string email)
		{
			try
			{
                if (string.IsNullOrEmpty(email))
                {
					ModelState.AddModelError("Email", "Email is required.");
					return View();
				}

				var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
				if (user != null)
				{

				}
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
			}
			return View();
		}

	}
}
