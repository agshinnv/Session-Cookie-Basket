using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Practice.Data;
using Practice.Models;
using Practice.Services.Interfaces;
using Practice.ViewModels.Baskets;

namespace Practice.Controllers
{
    public class BasketController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IProductService _productService;
        private readonly IHttpContextAccessor _accessor;

        public BasketController(AppDbContext context,
                                IProductService productService,
                                IHttpContextAccessor accessor)
        {
            _context = context;
            _productService = productService;
            _accessor = accessor;
        }
        public async Task<IActionResult> Index()
        {
            List<Product> products = await _productService.GetAllAsync();

            BasketPageVM model = new()
            {
                Products = products
            };
            

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> BasketProducts(int? id)
        {
            if (id is null) return BadRequest();
            
            List<BasketPageVM> cartProducts = null;


            if (_accessor.HttpContext.Request.Cookies["basket"] is not null)
            {
                cartProducts = JsonConvert.DeserializeObject<List<BasketPageVM>>(_accessor.HttpContext.Request.Cookies["basket"]);
            }
            else
            {
                cartProducts = new List<BasketPageVM>();
            }


            var dbProduct = await _context.Products.FirstOrDefaultAsync(m => m.Id == (int)id);


            var existProduct = cartProducts.FirstOrDefault(m => m.Id == (int)id);

            if (existProduct is not null)
            {
                existProduct.Count++;
            }
            else
            {
                cartProducts.Add(new BasketPageVM
                {
                    Id = (int)id,
                    Count = 1,
                    Price = dbProduct.Price
                });
            }


            _accessor.HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cartProducts));





            int count = cartProducts.Sum(m => m.Count);
            decimal total = cartProducts.Sum(m => m.Count * m.Price);


            return Ok(new { count, total });



        }
    }
}
