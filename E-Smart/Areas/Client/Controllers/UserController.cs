using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Repository;
using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Areas.Client.Controllers
{
	public class UserController : Controller
	{
		private readonly IUserRepository _userRepository;
		public UserController(IUserRepository userRepository)
		{
			_userRepository = userRepository;
		}

		public async Task<IActionResult> Index()
		{
			var users = await _userRepository.GetAllUser();
			return View(users);
		}

		public IActionResult Create()
		{
			return View();
		}
	}
}
