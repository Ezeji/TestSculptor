using Microsoft.EntityFrameworkCore;
using TestApp.Data;
using TestApp.Repository.Interfaces;

namespace TestApp.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class, new()
    {
        protected TestAppDbContext _db;

        public GenericRepository(TestAppDbContext db)
        {
            _db = db;
        }

        public IQueryable<T> Query()
        {
            return _db.Set<T>().AsQueryable();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _db.Set<T>().FindAsync(id);

            return entity;
        }

        public async Task<int> CreateAsync(T entity, bool isSave = true)
        {
            if (entity == null)
            {
                return 0;
            }

            _db.Set<T>().Add(entity);

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return 1;
        }

        public async Task<int> DeleteAsync(int id, bool isSave = true)
        {
            T? entity = await GetByIdAsync(id);

            if (entity == null)
            {
                return 0;
            }

            _db.Set<T>().Remove(entity);

            if (isSave)
            {
                return await SaveChangesToDbAsync();
            }

            return 1;
        }

        public async Task<int> SaveChangesToDbAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            return 1;
        }
    }
}
