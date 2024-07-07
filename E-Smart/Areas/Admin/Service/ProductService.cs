using E_Smart.Areas.Admin.Models;
using E_Smart.Areas.Admin.Repository;
using E_Smart.Data;
using Microsoft.EntityFrameworkCore;

namespace E_Smart.Areas.Admin.Service
{
    public class ProductService : IProductRepository
    {
        private readonly DatabaseContext _dbContext;
        public ProductService(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddProduct(Product product)
        {
            await _dbContext.AddAsync(product);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteProduct(int id)
        {
            var productID = await GetProduct(id);
            if (productID != null)
            {
                 _dbContext.Products.Remove(productID);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Product>> GetAllProduct()
        {
            var products = await _dbContext.Products.Include(p=>p.Category).OrderByDescending(p => p.ProductId).ToListAsync();
            return products;
        }

        public async Task<Product> GetProduct(int id)
        {
            var productID = await _dbContext.Products.FindAsync(id);
            return productID;
        }

        public async Task UpdateProduct(Product product)
        {
            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();
        }
    }
}
