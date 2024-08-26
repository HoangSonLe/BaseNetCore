using Core.CommonModels.BaseModels;
using System.Linq.Expressions;

namespace Application.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            PagingParameters paging = null,
            string includeProperties = ""
        );
        Task<PagedResponse<TEntity>> GetWithPagingAsync(PagingParameters paging, Expression<Func<TEntity, bool>> filter = null,
         Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
         string includeProperties = "");
        Task<TEntity> FindAsync(object id);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "");

        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        Task<int> AddAsync(TEntity entity);

        Task<int> AddRangeAsync(List<TEntity> entity);

        Task<int> UpdateAsync(TEntity entityToUpdate);

        Task<int> UpdateRangeAsync(List<TEntity> entity);

        Task<int> DeleteAsync(object id);


        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filter);

        Task<int> DeleteRangeAsync(List<TEntity> entityToDelete);

    }
}
