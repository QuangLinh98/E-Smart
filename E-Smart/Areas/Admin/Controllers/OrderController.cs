using E_Smart.Areas.Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IOrderRepository _orderRepository;

		public OrderController(IOrderRepository orderRepository)
		{
			_orderRepository = orderRepository;
		}

		public async Task<IActionResult> Index()
		{
			var orders = await _orderRepository.GetAllOrder();
			return View(orders);
		}

		public async Task<IActionResult>Delete(int id)
		{
			try
			{
				var existingOrder = await _orderRepository.GetOneOrder(id);
				if (existingOrder != null)
				{
					await _orderRepository.DeleteOrder(id);
					return RedirectToAction("Index");
				}
				return View();
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}
	}
}
