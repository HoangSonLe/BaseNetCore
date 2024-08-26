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
        private async Task<int> _DeleteAsync(TEntity entityToDelete)
        {
            if (entityToDelete == null)
                throw new ArgumentNullException("entityToDelete");
            DbSet.Remove(entityToDelete);
            return await _SaveChangesAsync();

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
        public async Task<PagedResponse<TEntity>> GetWithPagingAsync(
            PagingParameters paging,
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = ""
        )
        {
            if (paging == null)
            {
                throw new ArgumentNullException("Paging parameters can not be null");
            }
            var query = _Get(filter, orderBy, includeProperties, paging);
            var totalRecords = await query.QueryNoPaging.CountAsync();
            var data = await query.QueryPaging.ToListAsync();
            return new PagedResponse<TEntity>(data, paging.PageNumber, paging.PageSize, totalRecords);
        }
        public async Task<TEntity> FindAsync(object id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
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
        public async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            var query = _dbSet;
            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            if (orderBy != null && predicate == null)
            {
                throw new ArgumentNullException("If Where Condition is not null, Order By can not be null");
            }

            return await orderBy(query).LastOrDefaultAsync();
        }
        public async Task<int> AddAsync(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            await DbSet.AddAsync(entity);
            return await _SaveChangesAsync();
        }
        public async Task<int> AddRangeAsync(List<TEntity> entity)
        {
            if (entity == null || entity.Count == 0)
                throw new ArgumentNullException("entity");
            await DbSet.AddRangeAsync(entity);
            return await _SaveChangesAsync();
        }
        public async Task<int> UpdateAsync(TEntity entityToUpdate)
        {
            if (entityToUpdate == null)
                throw new ArgumentNullException("entityToUpdate");
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return await _SaveChangesAsync();
        }
        public async Task<int> UpdateRangeAsync(List<TEntity> entity)
        {
            if (entity == null || entity.Count == 0)
                throw new ArgumentNullException("entity");
            DbSet.UpdateRange(entity);
            return await _SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(object id)
        {
            var entity = await FindAsync(id);
            if (entity != null)
            {
                DbSet.Remove(entity);
            }
            return await _SaveChangesAsync();
        }
        public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> filter)
        {
            var entityToDelete = _dbSet.Where(filter).FirstOrDefault();
            return await _DeleteAsync(entityToDelete);
        }
        public async Task<int> DeleteRangeAsync(List<TEntity> entityToDelete)
        {
            if (entityToDelete == null)
                throw new ArgumentNullException("entityToDelete");
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
