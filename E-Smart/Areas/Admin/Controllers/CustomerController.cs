using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class CustomerController : Controller
	{
		private readonly ICustomerRepository _customerRepository;

		public CustomerController(ICustomerRepository customerRepository)
		{
			_customerRepository = customerRepository;
		}

		public async Task<IActionResult> Index()
		{
		    var customer = await _customerRepository.GetAllCustomer();
			return View(customer);
		}

		public IActionResult CustomerRegister()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CustomerRegister(Customer customer)
		{
			try
			{
                if (ModelState.IsValid)
                {
					await _customerRepository.AddCustomer(customer);
					return RedirectToAction("Index", "Cart", new {area = "Client"});
                }
				return View();
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View();
			}
		}

		public async Task<IActionResult>Delete(int id)
		{
			try
			{
				var existingCustomer = await _customerRepository.GetOneCustomer(id);
				if (existingCustomer != null)
				{
					await _customerRepository.DeleteCustomer(id);
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
