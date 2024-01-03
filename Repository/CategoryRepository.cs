using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Store.Data;
using Store.Interfaces;
using Store.Models;

namespace Store.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        public AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Category> UpdateAsync(Category category)
        {
            _appDbContext.Update(category);
            await _appDbContext.SaveChangesAsync();
            return category;
        }
        public async Task Create(Category entity)
        {
            await _appDbContext.AddAsync(entity);
            await Save();
        }

        public async Task Delete(Category entity)
        {
            _appDbContext.Remove(entity);
            await Save();
        }
        public async Task<bool> DoesExist(Expression<Func<Category, bool>>? filter = null)
        {
            IQueryable<Category> query = _appDbContext.Categories;
            return await query.AnyAsync(filter);
        }

        public async Task<Category> Get(Expression<Func<Category, bool>>? filter = null, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<Category> query = _appDbContext.Categories;
            if (!tracked)
            {
                query = query.AsNoTracking();
            }
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Category>> GetAll(Expression<Func<Category, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Category> query = _appDbContext.Categories;

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return await query.ToListAsync();
        }

        public async Task Save()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}
