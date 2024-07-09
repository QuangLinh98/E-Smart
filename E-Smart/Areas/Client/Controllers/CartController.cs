using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Models.ViewModels;
using E_Smart.Data;
using E_Smart.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using E_Smart.Service;
using Microsoft.EntityFrameworkCore;


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

		// GIỎ HÀNG
		public IActionResult Index()
		{
			List<CartItem> cartItems = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

			//Show message 
			var message = TempData["Message"] as string;
			ViewBag.Message = message;

			//Lấy ra CartItemViewModel
			CartItemViewModel cartVM = new()
			{
				CartItems = cartItems,
				GrandTotal = cartItems.Sum(c => c.Quantity * c.Price),
			};
			return View(cartVM);    // Trả về trang CartItemViewModel
		}

		//HÀM XỬ LÝ ADD SẢN PHẨM VÀO GIỎ HÀNG
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

		//HÀM XỬ LÝ TĂNG SỐ LƯỢNG CỦA SẢN PHẨM  (+) 
		public async Task<IActionResult> Increase(int id)
		{
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");
			CartItem cartItem = cart.Where(c => c.Product_Id == id).FirstOrDefault();

			// Nếu item > 1 thì ta sẽ tăng số lượng xuống
			if (cartItem.Quantity >= 1)
			{
				++cartItem.Quantity;
			}
			else
			{
				cart.RemoveAll(p => p.Product_Id == id);
			}

            //Kiểm tra số lượng  tồn kho trước 
            foreach (var item in cart)
            {
                var product = await _productRepository.GetProduct(item.Product_Id);
                if (product == null || product.Product_quantity < item.Quantity)
                {
                    // Thông báo nếu số lượng mua vượt quá số lượng tồn kho
                    TempData["Message"] = $"The quantity for {item.Name} exceeds the available stock.";
                    return RedirectToAction("Index");
                }            
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

 

        //HÀM XỬ LÝ GIẢM SỐ LƯỢNG CỦA SẢN PHẨM  (-)
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

		//HÀM XỬ LÝ XÓA TOÀN 1 SẢN PHẨM GIỎ HÀNG 
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

		//HÀM XỬ LÝ XÓA TOÀN BỘ GIỎ HÀNG 
		public async Task<IActionResult> Clear()
		{
			HttpContext.Session.Remove("Cart");
			return RedirectToAction("Index");
		}

		// HÀM XỬ LÝ HIỂN THỊ QUANTITY LÊN BIỂU TƯỢNG CART PHẦN HOME INDEX
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

		//PHƯƠNG THỨC XỬ LÝ LƯU ĐƠN HÀNG 
		private void SaveOrder(Order order, List<CartItem> cart)
		{
			// Lưu thông tin đơn hàng vào bảng Orders
			_dbContext.Orders.Add(order);
			_dbContext.SaveChanges();

			// Lưu thông tin chi tiết đơn hàng vào bảng OrderDetails
			foreach (var item in cart)
			{
				OrderDetail orderDetail = new OrderDetail
				{
					Order_Id = order.OrderId,
					Product_Id = item.Product_Id,
					Unit_Price = item.Price,
					Quantity = item.Quantity
				};

				_dbContext.OrderDetails.Add(orderDetail);
			}

			_dbContext.SaveChanges();
		}

		//TRANG THÔNG BÁO LỖI
		public IActionResult PaymentFail()
		{
			return View();
		}

		//PHƯƠNG THỨC CHUẨN BỊ THÔNG TIN CHUNG CHO VIỆC THANH TOÁN 
		private void PrepareCheckout(IFormCollection form)
		{
			var customerPhone = form["Phone"];
			var addressDelivery = form["Address_Delivery"];

			// Lưu thông tin khách hàng vào session
			HttpContext.Session.SetString("CustomerPhone",customerPhone);
			HttpContext.Session.SetString("AddressDelivery", addressDelivery);
		}

		// PHƯƠNG THỨC XỬ LÝ THANH TOÁN BẰNG COD
		[HttpPost]
		public async Task<ActionResult> CheckoutCOD(IFormCollection form)
		{
			try
			{
				List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

				// Chuẩn bị thông tin chung cho việc thanh toán
				PrepareCheckout(form);

				//Kiểm tra số lượng  tồn kho trước 
				foreach (var item in cart)
				{
					var product = await _productRepository.GetProduct(item.Product_Id);
					if (product == null || product.Product_quantity < item.Quantity)
					{
						// Thông báo nếu số lượng mua vượt quá số lượng tồn kho
						TempData["Message"] = $"The quantity for {item.Name} exceeds the available stock.";
						return RedirectToAction("Index");
                    }
                }
                //Cập nhật lại giỏ hàng 
                HttpContext.Session.SetJson("Cart", cart);

                // Tạo đối tượng Order và lưu vào bảng Orders
                Order order = new Order
				{
					Order_date = DateTime.Now,
					CustomerPhone = form["Phone"],
					Order_description = form["Address_Delivery"],
					Status = "Pending"
				};

				// Lưu đơn hàng và chi tiết đơn hàng vào cơ sở dữ liệu
				SaveOrder(order, cart);

				// Trừ số lượng tồn kho cho từng sản phẩm trong giỏ hàng
				foreach (var item in cart)
				{
					var product = await _productRepository.GetProduct(item.Product_Id);
					if (product != null)
					{
						product.Product_quantity -= item.Quantity;
						_dbContext.Products.Update(product);
					}
				}

				await _dbContext.SaveChangesAsync();

				HttpContext.Session.SetJson("Cart", new List<CartItem>());

				// Gửi email xác nhận đơn hàng
				var customer = await _dbContext.Customers.FirstOrDefaultAsync(c => c.Customer_phone == order.CustomerPhone);
				if (customer == null)
				{
					return Content("Customer not found. Please check the customer code.");
				}

				var subject = "Confirm your order.";
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

				cart.Clear();

				// Chuyển hướng đến trang xác nhận đặt hàng thành công
				return RedirectToAction("ShoppingSuccess", "Cart");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}

		// PHƯƠNG THỨC XỬ LÝ THANH TOÁN BẰNG VnPay
		[HttpPost]
		public ActionResult CheckoutVnPay()
		{
			try
			{
				List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

				// Chuẩn bị thông tin chung cho việc thanh toán
				PrepareCheckout(HttpContext.Request.Form);

				// Chuẩn bị dữ liệu cho thanh toán bằng VnPay
				var vnPayModel = new VnPaymentRequestModel
				{
					Amount = cart.Sum(p => p.Total),
					CreatedDate = DateTime.Now,
					FullName = cart[0].Name,
					OrderId = new Random().Next(1000, 10000)
				};

				// Chuyển hướng đến cổng thanh toán VnPay
				return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}

		// XỬ LÝ THANH TOÁN THÀNH CÔNG QUA VnPay VÀ CALLBACK VỀ HỆ THỐNG
		public async Task<IActionResult> PaymentCallBack()
		{
			var response = _vnPayService.PaymentExecute(Request.Query);
			if (response == null || response.VnPayResponseCode != "00")   // 00 => mã thành công khi thanh toán 
			{
				TempData["Message"] = $"Checkout VnPay error! {response.VnPayResponseCode}";
				return RedirectToAction("PaymentFail");
			}

			// Lấy giỏ hàng từ session
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

			if (cart == null || !cart.Any())
			{
				TempData["Message"] = "Cart is empty.";
				return RedirectToAction("PaymentFail");
			}

			// Lấy thông tin khách hàng từ session
			var customerPhone = HttpContext.Session.GetString("CustomerPhone");
			var addressDelivery = HttpContext.Session.GetString("AddressDelivery");

			if (customerPhone == null || addressDelivery == null)
			{
				TempData["Message"] = "Customer information is missing.";
				return RedirectToAction("PaymentFail");
			}

            //Kiểm tra số lượng  tồn kho trước 
            foreach (var item in cart)
            {
				var product = await _productRepository.GetProduct(item.Product_Id);
                if (product == null || product.Product_quantity < item.Quantity)
                {
					// Thông báo nếu số lượng mua vượt quá số lượng tồn kho
					TempData["Message"] = $"The quantity for {item.Name} exceeds the available stock.";
					return RedirectToAction("Index");
				}
            }

            // Tạo đối tượng Order và lưu vào bảng Orders
            Order order = new Order
			{
				Order_date = DateTime.Now,
				CustomerPhone= customerPhone,
				Order_description = addressDelivery,
				Status = "Paid-VnPay"   // Đánh dấu đơn hàng đã thanh toán khi callback từ VnPay thành công
			};

			// Lưu đơn hàng và chi tiết đơn hàng vào cơ sở dữ liệu
			SaveOrder(order, cart);

            // Trừ số lượng tồn kho cho từng sản phẩm trong giỏ hàng
            foreach (var item in cart)
            {
				var product = await _productRepository.GetProduct(item.Product_Id);
                if (product != null)
                {
					product.Product_quantity -= item.Quantity;
					_dbContext.Products.Update(product);
                }
            }
			await _dbContext.SaveChangesAsync();

            // Xóa giỏ hàng sau khi thanh toán thành công
            HttpContext.Session.SetJson("Cart", new List<CartItem>());

			return RedirectToAction("ShoppingSuccess");
		}
	}
}



