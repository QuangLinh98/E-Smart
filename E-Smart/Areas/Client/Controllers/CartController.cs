﻿
using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Models.ViewModels;
using E_Smart.Data;
using E_Smart.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using System.Net;
using System.Net.Mail;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;
using E_Smart.Service;
using Microsoft.EntityFrameworkCore.Metadata.Internal;




namespace E_Smart.Areas.Client.Controllers
{
	[Area("client")]
	public class CartController : Controller
	{
		private readonly EmailService _emailService;
		private readonly DatabaseContext _dbContext;
		private readonly IProductRepository _productRepository;
		private readonly IVnPayService _vnPayService;

		public CartController(IProductRepository productRepository, DatabaseContext dbContext, EmailService emailService, IVnPayService vnPayService)
		{
			_productRepository = productRepository;
			_dbContext = dbContext;
			_emailService = emailService;
			_vnPayService = vnPayService;
		}

		// Giỏ hàng
		public IActionResult Index()
		{
			List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			//Lấy ra CartItemViewModel
			CartItemViewModel cartVM = new()
			{
				CartItems = cartItems,
				GrandTotal = cartItems.Sum(c => c.Quantity * c.Price),
			};
			return View(cartVM);    // Trả về trang CartItemViewModel
		}


		public async Task<IActionResult> Add(int id)
		{
			// Get All Product
			Product product = await _productRepository.GetProduct(id);

			//Create the cart list  
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			CartItem cartItems = cart.Where(c => c.Product_Id == id).FirstOrDefault();
			if (cartItems == null)
			{
				cart.Add(new CartItem(product));
			}
			else
			{
				cartItems.Quantity += 1;
			}
			//Lưu trữ dữ liệu cart trong Session
			HttpContext.Session.SetJson("Cart", cart);


			// Return JSON response
			return Json(new { success = true, message = "Item added to cart successfully!" });
		}

		//HÀm xử lý tăng số lượng 
		public async Task<IActionResult> Increase(int id)
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
			CartItem cartItem = cart.Where(c => c.Product_Id == id).FirstOrDefault();

			// Nếu item > 1 thì ta sẽ giảm số lượng xuống
			if (cartItem.Quantity >= 1)
			{
				++cartItem.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.Product_Id == id);
			}

			//Nếu mà cart không tồn tại item nào  
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
			return RedirectToAction("Index");
		}

		//HÀm xử lý giảm số lượng 
		public async Task<IActionResult> Decrease(int id)
		{
			//Lấy tất cả sản phẩm 
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
			CartItem cartItem = cart.Where(c => c.Product_Id == id).FirstOrDefault();

			// Nếu item > 1 thì ta sẽ giảm số lượng xuống
			if (cartItem.Quantity > 1)
			{
				--cartItem.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.Product_Id == id);
			}

			//Nếu mà cart không tồn tại item nào  
			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}
			return RedirectToAction("Index");
		}

		//HÀm xử lý xóa 1 sản phẩm 
		public async Task<IActionResult> Remove(int id)
		{
			//Lấy tất cả sản phẩm
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
			cart.RemoveAll(c => c.Product_Id == id);

			if (cart.Count == 0)
			{
				HttpContext.Session.Remove("Cart");
			}
			else
			{
				HttpContext.Session.SetJson("Cart", cart);
			}

			return RedirectToAction("Index");
		}

		//HÀm xử lý xóa toàn bộ giỏ hàng 
		public async Task<IActionResult> Clear()
		{
			HttpContext.Session.Remove("Cart");
			return RedirectToAction("Index");
		}

		// Hàm xử lý hiển thị quantity lên biểu tượng cart phần Home Index
		public async Task<IActionResult> GetCartCount()
		{
			// Get cart items from session
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

			// Calculate total cart quantity
			int cartCount = cart.Sum(item => item.Quantity);

			return Json(new { cartCount }); // Return JSON response with cart count
		}

		public ActionResult ShoppingSuccess()
		{
			return View();
		}

		public ActionResult Checkout()
		{
			return View();
		}

		//hàm xử lý Checkout
		[HttpPost]
		public async Task<ActionResult> Checkout(IFormCollection form , string payment = "COD")
		{
			try
			{
				List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

				//Thanh toán trước bằng VnPay
				if (payment == "Checkout VnPay")
                {
					var vnPayModel = new VnPaymentRequestModel
					{
						Amount = cart.Sum(p => p.Total),
						CreatedDate = DateTime.Now,
						FullName = cart[0].Name,
						OrderId = new Random().Next(1000,10000)

					};
					return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                

				//Lưu thông tin vào bảng Order
				Order order = new Order();
				order.Order_date = DateTime.Now;
				order.CustomerCode = int.Parse(form["Code_Customer"]);   // Lấy mã code tại fomr Checkout, vì Code_Customer là dạng số lên cần ép kiểu để lưu sang dạng chuôi
				order.Order_description = form["Address_Delivery"];    // Lấy địa chỉ bên form checkout 
				order.Status = "Pending";

				_dbContext.Orders.Add(order);   // Add Order vào database
				_dbContext.SaveChanges();


				// Lưu vào bảng OrderDetail
				foreach (var item in cart)
				{
					OrderDetail orderDetail = new OrderDetail();

					//Lấy cái OrderId để lưu vào bảng OrderDetail
					orderDetail.Order_Id = order.OrderId;
					orderDetail.Product_Id = item.Product_Id;
					orderDetail.Unit_Price = item.Price;
					orderDetail.Quantity = item.Quantity;

					_dbContext.OrderDetails.Add(orderDetail);    // Add OrderDetail vào database
				}

				_dbContext.SaveChanges();
				
				HttpContext.Session.SetJson("Cart", cart);     // Sau khi clear cần cập nhật lại giỏ hàng có trong session

				// SEND MAIL LOGIC
				// Tìm khách hàng bằng CustomerCode
				var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Customer_code == order.CustomerCode);
				if (customer == null)
				{
					return Content("Customer not found. Please check the customer code.");
				}

				var subject = "Confirm your order.";
				//var body = $"Thank you for your order! Your order number is {order.OrderId}";
				var body = $"Dear {customer.Customer_name},<br/><br/>Thank you for your order!<br/><br/>Order details:<br/><br/>";
				body += $"Order number: {order.OrderId}<br/>";
				body += "-------------------------<br/>";
				foreach (var item in cart)
				{
					body += $"{item.Name} ,Quantity: {item.Quantity}, Price: {item.Price:C}<br/>";
				}
				body += "-------------------------<br/><br/>";
				body += "Total amount: " + cart.Sum(item => item.Quantity * item.Price).ToString("C") + "<br/><br/>";
				body += "We will notify you once your order has been processed.<br/><br/>";
				body += "Thank you for shopping with us!<br/><br/>Best regards,<br/>Your Store Team";

				await _emailService.SendEmail(customer.Customer_email, subject, body);

				cart.Clear();    // Sau khi checkout thì sẽ xóa hết giỏ hàng
			
				// Chuyển hướng đến trang xác nhận đặt hàng thành công
				return RedirectToAction("ShoppingSuccess", "Cart");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}

		//Trang thông báo lỗi
		public IActionResult PaymentFail()
		{
			return View();
		}


		public IActionResult PaymentCallBack()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
            if (response == null || response.VnPayResponseCode != "00")
            {
				TempData["Message"] = $"Checkout VnPay error! {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
            }

			// Lưu đơn hàng vào database

			TempData["Message"] = $"Checkout VnPay successfully";
			return RedirectToAction("ShoppingSuccess");
		}

	}
}



