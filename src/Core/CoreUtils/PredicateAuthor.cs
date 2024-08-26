using System.Linq.Expressions;

namespace Core.CoreUtils
{
    public static class PredicateAuthor
    {
        public static Expression<Func<TEntity, bool>> BuildPredicate<TEntity>(string propertyName, object value)
        {
            // Create parameter expression for the entity
            var parameter = Expression.Parameter(typeof(TEntity), "entity");

            // Create expression for accessing the property
            var property = Expression.Property(parameter, propertyName);

            // Create constant expression for the value
            var constant = Expression.Constant(value);

            // Create equality expression
            var equality = Expression.Equal(property, constant);

            // Create and return the lambda expression
            return Expression.Lambda<Func<TEntity, bool>>(equality, parameter);
        }
    }
}
