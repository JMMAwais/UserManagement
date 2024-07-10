using Microsoft.AspNetCore.Mvc;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Repo;

namespace UserManagement.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductRepository _repository;
        private readonly ApplicationDbContext _db;
        public ProductController(IProductRepository productRepository, ApplicationDbContext db)
        {
            _repository = productRepository;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllProducts()
        {
            var products = _repository.GetAllProducts();
            return View(products);
        }


        public IActionResult GetProductById(int Id)
        {
            var product = _db.Products.SingleOrDefault(p => p.Id == Id);
            return View(product);
        }

        public IActionResult AddProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                _db.Products.Add(model);
                _db.SaveChanges();
                RedirectToAction("Index");
            }
            return View(model);
        }

        public IActionResult EditProduct(int Id, Product model)
        {
            if (ModelState.IsValid)
            {
                var product=_db.Products.SingleOrDefault(p => p.Id==Id);
                if(product != null)
                {
                  product.Title = model.Title;
                  product.Description = model.Description;
                  product.price=model.price;
                  _db.Products.Update(product);
                  _db.SaveChanges();
                }             
            }
            return View(model);
        }

        public IActionResult DeleteProduct(int Id)
        {
            if(Id !=0 )
            {
                var product= _db.Products.SingleOrDefault(  x => x.Id==Id); 
                _db.Products.Remove(product);
            }
            return RedirectToAction("Index");

        }

    }
}
