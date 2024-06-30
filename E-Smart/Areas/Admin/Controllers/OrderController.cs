using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Areas.Admin.Controllers
{
	public class OrderController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
