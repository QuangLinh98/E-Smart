using E_Smart.Areas.Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderDetailController : Controller
	{
		private readonly IOrderDetailRepository _orderDetailRepository;

		public OrderDetailController(IOrderDetailRepository orderDetailRepository)
		{
			_orderDetailRepository = orderDetailRepository;
		}

		public async Task<IActionResult> Index()
		{
			var orderDetails = await _orderDetailRepository.GetAllOrderDetail();
			return View(orderDetails);
		}

		public async Task<IActionResult>Delete(int id)
		{
			try
			{
				var existingOrderDetail = await _orderDetailRepository.GetOneOrderDetail(id);
				if (existingOrderDetail != null)
				{
					await _orderDetailRepository.DeleteOrderDetail(id);
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
