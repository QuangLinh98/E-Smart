using System.Diagnostics;
using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Controllers
{
	public class HomeController : Controller
	{
		private readonly IProductRepository _productRepository;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger,IProductRepository productRepository , ICategoryRepository categoryRepository)
		{
			_logger = logger;
			_categoryRepository = categoryRepository;
			_productRepository = productRepository;
		}

		public async Task<IActionResult> Index()
		{
			IEnumerable<Product> prouductList = await _productRepository.GetAllProduct();
			return View(prouductList);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
