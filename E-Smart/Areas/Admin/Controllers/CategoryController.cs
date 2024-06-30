using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace E_Smart.Areas.Admin.Controllers
{
	[Area("admin")]
	public class CategoryController : Controller
	{
		private readonly ICategoryRepository _categoryRepository;
		public CategoryController(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}
		public async Task<IActionResult> Index()
		{
			var categories = await _categoryRepository.GetAllCategory();
			return View(categories);
		}

		public IActionResult Create() 
		{
		  return View();
		}

		[HttpPost]
		public async Task<IActionResult> Create(Category category)
		{
			try
			{
                if (ModelState.IsValid)
                {
                    await _categoryRepository.AddCategory(category);
                    return RedirectToAction("Index");
                }
                return View(category);
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
                return View(category);
            }
            
        }

		public async Task<IActionResult> Edit(int id)
		{
			var cateID = await _categoryRepository.GetCategory(id);
			return View(cateID);
		}

		[HttpPost]
        public async Task<IActionResult> Edit(int id, Category category)
        {
			try
			{
                if (ModelState.IsValid)
                {
					await _categoryRepository.UpdateCategory(category);
					return RedirectToAction("Index");
                }
                return View(category);
            }
			catch (Exception ex)
			{
				ModelState.AddModelError("", ex.Message);
				return View(category);
			}
        }
    }
}
