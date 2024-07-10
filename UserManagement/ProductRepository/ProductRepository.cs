using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Repo
{
    public class ProductRepository:IProductRepository
    {
        private readonly ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db)
        {
                _db = db;
        }
        public List<Product> GetAllProducts()
        {
            var products = _db.Products.ToList();
            return products;
        }
    }
}
