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

		public async Task<IActionResult> Approve(int id)
		{
			try
			{
				var existingOrder = await _orderRepository.GetOneOrder(id);
				if (existingOrder != null && existingOrder.Status == "Pending") 
				{
					await _orderRepository.UpdateOrderStatus(id, "Approved");
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

		public async Task<IActionResult>ShipOrder(int id)
		{
			try
			{
				var existingOrder = await _orderRepository.GetOneOrder(id);
                if (existingOrder != null && existingOrder.Status == "Approved")
                {
					await _orderRepository.UpdateOrderStatus(id, "Delivering");
					return RedirectToAction("Index");
                }
				return View();
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("",ex.Message);
				return View();
			}
		}


		public async Task<IActionResult>ShowViewOrderDetail(int id)
		{
			var orderDetail = await _orderRepository.GetOrderDetails(id);
			return View(orderDetail);
		}
	}
}
