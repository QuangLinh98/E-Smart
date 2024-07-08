using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using E_Smart.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class OrderController : Controller
	{
		private readonly IOrderRepository _orderRepository;
		private readonly DatabaseContext _dbContext;
		private readonly IHubContext<OrderHub> _hubContext;  // Sử dụng 

		public OrderController(IOrderRepository orderRepository, IHubContext<OrderHub> hubContext, DatabaseContext dbContext)
		{
			_orderRepository = orderRepository;
			_hubContext = hubContext;
			_dbContext = dbContext;
		}

		public async Task<IActionResult> Index()
		{
			var orders = await _orderRepository.GetAllOrder();
			return View(orders);
		}

		public async Task<IActionResult> Delete(int id)
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

		//Cập nhậ trạng thái đã xac nhận đơn hàng
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

		//Cập nhậ trạng thái đang giao đơn hàng
		public async Task<IActionResult> ShipOrder(int id)
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
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}

		//Cập nhậ trạng thái đã giao đơn hàng
		public async Task<IActionResult> Delivered(int id)
		{
			try
			{
				var existingOrder = await _orderRepository.GetOneOrder(id);
				if (existingOrder != null && existingOrder.Status == "Delivering")
				{
					await _orderRepository.UpdateOrderStatus(id, "Paid-COD");
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

		public async Task<IActionResult> ShowViewOrderDetail(int id)
		{
			var orderDetail = await _orderRepository.GetOrderDetails(id);
			return View(orderDetail);
		}

		//Tạo Page dành cho Shipper có thể xác nhận tình trang đơn hàng 
		public IActionResult ShipperPage()
		{
			var pendingOrders = _dbContext.Orders.Where(o => o.Status == "Approved" || o.Status == "Delivering" ).ToList();
			return View(pendingOrders);
		}

		// Cập trạng thái khi Shipper cập nhật tình trạng đơn hàng cho Admin
		[HttpPost]
		[Route("Admin/Order/ShipperUpdateOrderStatus")]
		public async Task<IActionResult> ShipperUpdateOrderStatus([FromBody] OrderStatusUpdateModel model)
		{
			if (model == null || model.OrderId == 0 || string.IsNullOrEmpty(model.Status))
			{
				return BadRequest("Invalid order data");
			}

			var order = _dbContext.Orders.Find(model.OrderId);
			if (order != null)
			{
				order.Status = model.Status;
				_dbContext.SaveChanges();
				await _hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", model.OrderId, model.Status); // Gửi sự kiện SignalR khi cập nhật thành công
				return Ok();
			}
			return NotFound();
		}
	}

	public class OrderStatusUpdateModel
	{
		public int OrderId { get; set; }
		public string Status { get; set; }
	}
}

