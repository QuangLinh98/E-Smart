
using System.Text.Json;
using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using E_Smart.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace E_Smart.Areas.Admin.Controllers
{
    [Area("admin")]
    public class ProductController : Controller
    {
        private readonly DatabaseContext _dbContext;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment, ICategoryRepository categoryRepository, DatabaseContext dbContext)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
            _categoryRepository = categoryRepository;
            _dbContext = dbContext;
        }

        //Hiển thị danh sách kèm chức năng phân trang
        public async Task<IActionResult> Index(int? page)
        {
            int pageSize = 5; 
            var products = await _productRepository.GetAllProduct();

			var paginatedProducts = products.ToPagedList(page ?? 1, pageSize);   //?? nêu bị null sẽ gán = 1
			return View(paginatedProducts);
        }


		public async Task<IActionResult> Create()
        {
            var categories = await _categoryRepository.GetAllCategory();
            ViewBag.Category = new SelectList(categories, "CategoryId", "Category_name", "CategoryId");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (product.Product_imageFiles != null && product.Product_imageFiles.Any())
                    {
                        string subFolder = "product_images";
                        List<string> uploadedImagePaths = new List<string>();

                        foreach (var file in product.Product_imageFiles)
                        {
                            string uploadedImagePath = await FileUpload.SaveImage(subFolder, file);
                            uploadedImagePaths.Add(uploadedImagePath);
                        }

                        product.Product_imagePaths = JsonSerializer.Serialize(uploadedImagePaths);
                    }
                    await _productRepository.AddProduct(product);
                    return RedirectToAction("Index");
                }
                var categories = await _categoryRepository.GetAllCategory();
                ViewBag.Category = new SelectList(categories, "CategoryId", "Category_name", product.Category_Id);
                return View(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var categories = await _categoryRepository.GetAllCategory();
                ViewBag.Category = new SelectList(categories, "CategoryId", "Category_name", product.Category_Id);
                return View(product);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {

            // Get categoryID 
			var categories = await _categoryRepository.GetAllCategory();
			ViewBag.Category = new SelectList(categories, "CategoryId", "Category_name", "CategoryId");

            // Find productID and show data form
			var productID = await _productRepository.GetProduct(id);
            return View(productID);
        }

        [HttpPost]
        public async Task<IActionResult>Edit(Product product, int id)
        {
            try
            {
				if (id != product.ProductId)
				{
					return NotFound();
				}

				if (ModelState.IsValid)
                {
                    try
                    {
                        var existingProduct = await _productRepository.GetProduct(id);
                        
                        //Update product infomation
                        existingProduct.Product_name = product.Product_name;
                        existingProduct.Product_price = product.Product_price;
                        existingProduct.Product_quantity = product.Product_quantity;
                        existingProduct.Product_description = product.Product_description;
                        existingProduct.Category_Id = product.Category_Id;

                        //Handle Image
                        if (product.Product_imageFiles != null && product.Product_imageFiles.Any())
                        {
                            //Remove old image
                            FileUpload.DeleteImage(existingProduct.Product_imagePaths);

                            // Save new images and update imagePaths 
                            List<string> newImagePaths = new List<string>();
                            foreach (var file in product.Product_imageFiles)
                            {
								string subFolder = "product_images";
                                string uploadedImagePaths = await FileUpload.SaveImage(subFolder, file);
                                newImagePaths.Add(uploadedImagePaths);
							}
                            existingProduct.Product_imagePaths = JsonSerializer.Serialize(newImagePaths);
                        }

						//Update products in database
						await _productRepository.UpdateProduct(existingProduct);

						return RedirectToAction(nameof(Index));
					}
					catch (Exception ex)
                    {
						ModelState.AddModelError("", ex.Message);
					}
                }
                return View(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }
        }

        public async Task<IActionResult>Delete(int id)
        {
            try
            {
                var existingProduct = await _productRepository.GetProduct(id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

				//Delete image in folder 
				if (!string.IsNullOrEmpty(existingProduct.Product_imagePaths))
                {
                    FileUpload.DeleteImage(existingProduct.Product_imagePaths);
                }

                await _productRepository.DeleteProduct(id);
                return RedirectToAction("Index");

			}
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Index");
            }
        }

        #region  API CALLS

        [HttpGet]
        [Route("api/products")]
        public IEnumerable<Product> GetProducts() 
        {
          return (IEnumerable<Product>)_productRepository.GetAllProduct();
        }

		#endregion

	}
}
