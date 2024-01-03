using System.Linq.Expressions;
using Store.Models;

namespace Store.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product> UpdateAsync(Product product);
        Task<List<Product>> GetAll(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null);
        Task<Product> Get(Expression<Func<Product, bool>> filter = null, bool tracked = true, string? includeProperties = null);
        Task<bool> DoesExist(Expression<Func<Product, bool>> filter = null);

        Task Create(Product entity);
        Task Delete(Product entity);
        Task Save();
    }
}
