using Application.Interfaces;
using Core.CommonModels.BaseModels;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;

namespace Application.Repository
{
    public interface IRepositoryGenerator<TEntity> where TEntity : class
    {
        public Repository<TEntity> Repository { get; }
        public RespositoryAsNoTracking<TEntity> ReadOnlyRespository { get; }

    }
    public class RepositoryGenerator<TEntity> : IRepositoryGenerator<TEntity> where TEntity : class
    {
        private DbContext _dbContext;
        private DbContext _dbReadOnlyContext;
        private readonly Repository<TEntity> _Repository;
        private readonly RespositoryAsNoTracking<TEntity> _ReadOnlyRespository;

        public RepositoryGenerator(DbContext dbContext, DbContext dBReadOnlyContext)
        {
            _dbContext = dbContext;
            _dbReadOnlyContext = dBReadOnlyContext;
            _Repository = new Repository<TEntity>(_dbContext);
            _ReadOnlyRespository = new RespositoryAsNoTracking<TEntity>(_dbReadOnlyContext);
        }
        public Repository<TEntity> Repository => _Repository;
        public RespositoryAsNoTracking<TEntity> ReadOnlyRespository => _ReadOnlyRespository;
    }


    public class RespositoryAsNoTracking<TEntity> : Repository<TEntity> where TEntity : class
    {
        public RespositoryAsNoTracking(DbContext context) : base(context)
        {
            _dbSet = DbSet.AsNoTracking();
        }
        public async Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }
        public virtual int SaveChanges()
        {
            throw new NotImplementedException();
        }
    }
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;
        protected readonly DbSet<TEntity> DbSet;
        protected IQueryable<TEntity> _dbSet;
        public Repository(DbContext context)
        {
            _context = context;
            if (_context != null)
            {
                DbSet = _context.Set<TEntity>();
                _dbSet = DbSet;
            }
        }
        public class PagingQuery
        {
            public IQueryable<TEntity> QueryPaging { get; set; }
            public IQueryable<TEntity> QueryNoPaging { get; set; }
        }

        public class PagingQuery<T>
        {
            public IQueryable<T> QueryPaging { get; set; }
            public IQueryable<T> QueryNoPaging { get; set; }
        }
        #region PRIVATE
        private PagingQuery _Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", PagingParameters paging = null)
        {
            var result = new PagingQuery();
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }
            result.QueryNoPaging = query;
            result.QueryPaging = query;
            if (paging != null)
            {
                result.QueryPaging = result.QueryPaging.Skip((paging.PageNumber - 1) * paging.PageSize).Take(paging.PageSize);
            }
            return result;
        }

        private PagingQuery<TResult> _Get<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", PagingParameters paging = null)
        {
            var result = new PagingQuery<TResult>();
            IQueryable<TEntity> query = _dbSet.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Apply the selector to both queries
            result.QueryNoPaging = query.Select(selector);

            // Apply paging if needed
            if (paging != null)
            {
                result.QueryPaging = query.Skip((paging.PageNumber - 1) * paging.PageSize)
                                         .Take(paging.PageSize)
                                         .Select(selector);
            }
            else
            {
                result.QueryPaging = query.Select(selector);
            }

            return result;
        }
        private async Task<int> _DeleteAsync(TEntity entityToDelete)
        {
            if (entityToDelete == null)
                throw new ArgumentNullException("entityToDelete");
            DbSet.Remove(entityToDelete);
            return await this._SaveChangesAsync();

        }
        private async Task<int> _SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        #endregion
        #region PUBLIC
        public async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, PagingParameters paging = null,
            string includeProperties = "")
        {
            var query = _Get(filter, orderBy, includeProperties, paging).QueryPaging;
            return await query.ToListAsync();
        }

        public async Task<List<TResult>> GetAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            PagingParameters paging = null,
            string includeProperties = "")
        {
            var query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (paging != null)
            {
                query = query.Skip((paging.PageNumber - 1) * paging.PageSize).Take(paging.PageSize);
            }

            return await query.Select(selector).ToListAsync();
        }
        public async Task<PagedResponse<TEntity>> GetWithPagingAsync(
            PagingParameters paging,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        )
        {
            if (paging == null)
            {
                throw new ArgumentNullException(nameof(paging));
            }
            var query = _Get(filter, orderBy, includeProperties, paging);
            var totalRecords = await query.QueryNoPaging.CountAsync();
            var data = await query.QueryPaging.ToListAsync();
            return new PagedResponse<TEntity>(data, paging.PageNumber, paging.PageSize, totalRecords);
        }

        public async Task<PagedResponse<TResult>> GetWithPagingAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            PagingParameters paging,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        )
        {
            if (paging == null)
            {
                throw new ArgumentNullException(nameof(paging));
            }

            var query = _dbSet.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var totalRecords = await query.CountAsync();

            var pagedQuery = query.Skip((paging.PageNumber - 1) * paging.PageSize)
                                 .Take(paging.PageSize);

            var data = await pagedQuery.Select(selector).ToListAsync();

            return new PagedResponse<TResult>(data, paging.PageNumber, paging.PageSize, totalRecords);
        }
        public async Task<TEntity> FindAsync(object id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            return await DbSet.FindAsync(id);
        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            var query = _dbSet;
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<TResult> FirstOrDefaultAsync<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate,
            string includeProperties = "")
        {
            var query = _dbSet;
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return await query.Where(predicate).Select(selector).FirstOrDefaultAsync();
        }
        public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy == null)
            {
                throw new ArgumentNullException(nameof(orderBy), "Order By cannot be null for LastOrDefaultAsync");
            }

            return await orderBy(query).LastOrDefaultAsync();
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            await DbSet.AddAsync(entity);
            return await _SaveChangesAsync();
        }
        public async Task<int> AddRangeAsync(List<TEntity> entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (entity.Count == 0)
                throw new ArgumentException("Entity list cannot be empty", nameof(entity));

            await DbSet.AddRangeAsync(entity);
            return await _SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(TEntity entityToUpdate)
        {
            ArgumentNullException.ThrowIfNull(entityToUpdate, nameof(entityToUpdate));
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return await _SaveChangesAsync();
        }

        public async Task<int> UpdateRangeAsync(List<TEntity> entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (entity.Count == 0)
                throw new ArgumentException("Entity list cannot be empty", nameof(entity));

            DbSet.UpdateRange(entity);
            return await _SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(object id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            var entity = await FindAsync(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
            return await _SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            ArgumentNullException.ThrowIfNull(filter, nameof(filter));
            var entityToDelete = _dbSet.Where(filter).FirstOrDefault();
            if (entityToDelete == null)
                return 0;

            return await _DeleteAsync(entityToDelete);
        }

        public async Task<int> DeleteRangeAsync(List<TEntity> entityToDelete)
        {
            ArgumentNullException.ThrowIfNull(entityToDelete, nameof(entityToDelete));
            if (entityToDelete.Count == 0)
                return 0;

            DbSet.RemoveRange(entityToDelete);
            return await _SaveChangesAsync();
        }
        #endregion

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

            }
            disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
