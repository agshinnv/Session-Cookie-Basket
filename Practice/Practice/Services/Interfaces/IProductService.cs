using Practice.Models;

namespace Practice.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product> GetById(int id);
    }
}
