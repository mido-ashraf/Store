using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Linq;
using Store.Data;
using Store.Interfaces;
using Store.Models;

namespace Store.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _appDbContext;
        public ProductRepository(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _appDbContext.Update(product);
            await _appDbContext.SaveChangesAsync();
            return product;
        }
        public async Task Create(Product entity)
        {
            await _appDbContext.AddAsync(entity);
            await Save();
        }

        public async Task Delete(Product entity)
        {
            _appDbContext.Remove(entity);
            await Save();
        }
        public async Task<bool> DoesExist(Expression<Func<Product, bool>>? filter = null)
        {
            IQueryable<Product> query = _appDbContext.Products;
            return await query.AnyAsync(filter);
        }

        public async Task<Product> Get(Expression<Func<Product, bool>>? filter = null, bool tracked = true, string? includeProperties = null)
        {
            IQueryable<Product> query = _appDbContext.Products;
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

        public async Task<List<Product>> GetAll(Expression<Func<Product, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<Product> query = _appDbContext.Products;

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
