using Application.Interfaces;
using Core.CommonModels.BaseModels;
using System.Net;

namespace Application.Helpers
{
    public static class LinqHelper
    {
        public static async Task TrySaveChangesAsync<TEntity, TResult>(
            this Acknowledgement ack,
            Func<IRepository<TEntity>, Task<TResult>> repositoryOperation,
            IRepository<TEntity> respository,
            Action<Exception> handleError = null
        ) where TEntity : class
        {
            try
            {
                await repositoryOperation(respository);
                ack.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                handleError?.Invoke(ex);
                ack.StatusCode = HttpStatusCode.BadRequest;
                ack.ExtractMessage(ex);
            }
        }
        public static DateTime GetDateTimeNow()
        {
            return DateTime.Now.ToUniversalTime();
        }
        public static List<T> ToSingleList<T>(this T data)
        {
            return new List<T> { data };
        }
    }
}
