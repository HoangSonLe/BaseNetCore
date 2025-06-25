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
        public RepositoryAsNoTracking<TEntity> ReadOnlyRepository { get; }

    }
    public class RepositoryGenerator<TEntity> : IRepositoryGenerator<TEntity> where TEntity : class
    {
        private DbContext _dbContext;
        private DbContext _dbReadOnlyContext;
        private readonly Repository<TEntity> _Repository;
        private readonly RepositoryAsNoTracking<TEntity> _ReadOnlyRepository;

        public RepositoryGenerator(DbContext dbContext, DbContext dBReadOnlyContext)
        {
            _dbContext = dbContext;
            _dbReadOnlyContext = dBReadOnlyContext;
            _Repository = new Repository<TEntity>(_dbContext);
            _ReadOnlyRepository = new RepositoryAsNoTracking<TEntity>(_dbReadOnlyContext);
        }
        public Repository<TEntity> Repository => _Repository;
        public RepositoryAsNoTracking<TEntity> ReadOnlyRepository => _ReadOnlyRepository;
    }


    public class RepositoryAsNoTracking<TEntity> : Repository<TEntity> where TEntity : class
    {
        public RepositoryAsNoTracking(DbContext context) : base(context)
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

        // Static separator for string splitting
        private static readonly char[] _separators = new char[] { ',' };

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
            foreach (var includeProperty in includeProperties.Split(_separators, StringSplitOptions.RemoveEmptyEntries))
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
            foreach (var includeProperty in includeProperties.Split(_separators, StringSplitOptions.RemoveEmptyEntries))
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
            ArgumentNullException.ThrowIfNull(entityToDelete, nameof(entityToDelete));
            DbSet.Remove(entityToDelete);
            return await _SaveChangesAsync().ConfigureAwait(false);
        }
        private async Task<int> _SaveChangesAsync()
        {
            return await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        #endregion
        #region PUBLIC
        public async Task<List<T>> GetAsync<T>(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            PagingParameters paging = null,
            string includeProperties = "",
            Expression<Func<TEntity, T>> selector = null,
            CancellationToken cancellationToken = default) where T : class
        {
            var query = _Get(filter, orderBy, includeProperties, paging).QueryPaging;

            if (selector == null && typeof(T) == typeof(TEntity))
            {
                // If no selector and T is TEntity, return list of TEntity
                return await query.Cast<T>().ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            else if (selector != null)
            {
                // If selector is provided, use it to transform to type T
                return await query.Select(selector).ToListAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException($"Cannot convert from {typeof(TEntity).Name} to {typeof(T).Name} without a selector");
            }
        }

        // Non-generic method for backward compatibility
        public async Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            PagingParameters paging = null,
            string includeProperties = "",
            CancellationToken cancellationToken = default)
        {
            return await GetAsync<TEntity>(filter, orderBy, paging, includeProperties, null, cancellationToken).ConfigureAwait(false);
        }
        public async Task<PagedResponse<T>> GetWithPagingAsync<T>(
            PagingParameters paging,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "",
            Expression<Func<TEntity, T>> selector = null) where T : class
        {
            ArgumentNullException.ThrowIfNull(paging, nameof(paging));

            if (selector == null && typeof(T) == typeof(TEntity))
            {
                // If no selector and T is TEntity, return list of TEntity
                var query = _Get(filter, orderBy, includeProperties, paging);
                var totalRecords = await query.QueryNoPaging.CountAsync().ConfigureAwait(false);
                var data = await query.QueryPaging.ToListAsync().ConfigureAwait(false);
                return new PagedResponse<T>(data.Cast<T>().ToList(), paging.PageNumber, paging.PageSize, totalRecords);
            }
            else if (selector != null)
            {
                // If selector is provided, use it to transform to type T
                var query = _Get(selector, filter, orderBy, includeProperties, paging);
                var totalRecords = await query.QueryNoPaging.CountAsync().ConfigureAwait(false);
                var data = await query.QueryPaging.ToListAsync().ConfigureAwait(false);
                return new PagedResponse<T>(data, paging.PageNumber, paging.PageSize, totalRecords);
            }
            else
            {
                throw new InvalidOperationException($"Cannot convert from {typeof(TEntity).Name} to {typeof(T).Name} without a selector");
            }
        }

        // Non-generic method for backward compatibility
        public async Task<PagedResponse<TEntity>> GetWithPagingAsync(
            PagingParameters paging,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        )
        {
            return await GetWithPagingAsync<TEntity>(paging, filter, orderBy, includeProperties);
        }
        public async Task<TEntity> FindAsync(object id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            return await DbSet.FindAsync(id).ConfigureAwait(false);
        }
        public async Task<T> FirstOrDefaultAsync<T>(
            Expression<Func<TEntity, bool>> predicate,
            string includeProperties = "",
            Expression<Func<TEntity, T>> selector = null) where T : class
        {
            var query = _dbSet;

            // Add includes
            foreach (var includeProperty in includeProperties.Split(_separators, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Apply predicate
            query = query.Where(predicate);

            if (selector == null && typeof(T) == typeof(TEntity))
            {
                // If no selector and T is TEntity, return entity directly
                return await query.FirstOrDefaultAsync().ConfigureAwait(false) as T;
            }
            else if (selector != null)
            {
                // If selector is provided, use it to transform to type T
                return await query.Select(selector).FirstOrDefaultAsync().ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException($"Cannot convert from {typeof(TEntity).Name} to {typeof(T).Name} without a selector");
            }
        }

        // Non-generic method for backward compatibility
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, string includeProperties = "")
        {
            return await FirstOrDefaultAsync<TEntity>(predicate, includeProperties);
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

            return await orderBy(query).LastOrDefaultAsync().ConfigureAwait(false);
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            await DbSet.AddAsync(entity).ConfigureAwait(false);
            return await _SaveChangesAsync().ConfigureAwait(false);
        }
        public async Task<int> AddRangeAsync(List<TEntity> entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (entity.Count == 0)
                throw new ArgumentException("Entity list cannot be empty", nameof(entity));

            await DbSet.AddRangeAsync(entity).ConfigureAwait(false);
            return await _SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> UpdateAsync(TEntity entityToUpdate)
        {
            ArgumentNullException.ThrowIfNull(entityToUpdate, nameof(entityToUpdate));
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return await _SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> UpdateRangeAsync(List<TEntity> entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (entity.Count == 0)
                throw new ArgumentException("Entity list cannot be empty", nameof(entity));

            DbSet.UpdateRange(entity);
            return await _SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<int> DeleteAsync(object id)
        {
            ArgumentNullException.ThrowIfNull(id, nameof(id));
            var entity = await FindAsync(id).ConfigureAwait(false);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
            return await _SaveChangesAsync().ConfigureAwait(false);
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
            return await _SaveChangesAsync().ConfigureAwait(false);
        }

        // Transaction-friendly methods (don't auto-save)
        public async Task AddWithoutSaveAsync(TEntity entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            await DbSet.AddAsync(entity).ConfigureAwait(false);
        }

        public async Task AddRangeWithoutSaveAsync(List<TEntity> entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (entity.Count == 0)
                throw new ArgumentException("Entity list cannot be empty", nameof(entity));

            await DbSet.AddRangeAsync(entity);
        }

        public void UpdateWithoutSave(TEntity entityToUpdate)
        {
            ArgumentNullException.ThrowIfNull(entityToUpdate, nameof(entityToUpdate));
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void UpdateRangeWithoutSave(List<TEntity> entity)
        {
            ArgumentNullException.ThrowIfNull(entity, nameof(entity));
            if (entity.Count == 0)
                throw new ArgumentException("Entity list cannot be empty", nameof(entity));

            DbSet.UpdateRange(entity);
        }

        public void DeleteWithoutSave(TEntity entityToDelete)
        {
            ArgumentNullException.ThrowIfNull(entityToDelete, nameof(entityToDelete));
            DbSet.Remove(entityToDelete);
        }

        public void DeleteRangeWithoutSave(List<TEntity> entityToDelete)
        {
            ArgumentNullException.ThrowIfNull(entityToDelete, nameof(entityToDelete));
            if (entityToDelete.Count == 0)
                return;

            DbSet.RemoveRange(entityToDelete);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _SaveChangesAsync().ConfigureAwait(false);
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
    }}
