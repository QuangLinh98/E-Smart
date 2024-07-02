using Azure;
using System;
using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Areas.Client.Models;
using E_Smart.Areas.Client.Models.ViewModels;
using E_Smart.Data;
using E_Smart.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Smart.Areas.Client.Controllers
{
	[Area("client")]
	public class CartController : Controller
	{

		private readonly DatabaseContext _dbContext;
		private readonly IProductRepository _productRepository;

		public CartController(IProductRepository productRepository, DatabaseContext dbContext)
		{
			_productRepository = productRepository;
			_dbContext = dbContext;
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

			//Hiển thị thông báo Thành công trong folder _NotificationPartial
			//TempData["success"] = "Add item to cart Successfully!";

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
			List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ;

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
		public ActionResult Checkout (IFormCollection form)
		{
			try
			{
				List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ;
				
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
				cart.Clear();    // Sau khi checkout thì sẽ xóa hết giỏ hàng
				HttpContext.Session.SetJson("Cart", cart);     // Sau khi clear cần cập nhật lại giỏ hàng có trong session

				return RedirectToAction("ShoppingSuccess","Cart");
            }
			catch 
			{
				return Content("Error checkout . Please try to check infomation of Customer...");
			}
		}

		
	}
}



