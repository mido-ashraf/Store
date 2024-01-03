using System.Linq.Expressions;
using Store.Models;

namespace Store.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category> UpdateAsync(Category category);
        Task<List<Category>> GetAll(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null);
        Task<Category> Get(Expression<Func<Category, bool>> filter = null, bool tracked = true, string? includeProperties = null);
        Task<bool> DoesExist(Expression<Func<Category, bool>> filter = null);

        Task Create(Category entity);
        Task Delete(Category entity);
        Task Save();

    }
}
