namespace TestApp.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class, new()
    {
        IQueryable<T> Query();

        Task<int> CreateAsync(T entity, bool isSave = true);

        Task<int> DeleteAsync(int id, bool isSave = true);

        Task<T?> GetByIdAsync(int id);

        Task<int> SaveChangesToDbAsync();
    }
}
