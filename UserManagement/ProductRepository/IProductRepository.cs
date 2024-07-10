using UserManagement.Models;

namespace UserManagement.Repo
{
    public interface IProductRepository
    {
        public List<Product> GetAllProducts();
    }
}
