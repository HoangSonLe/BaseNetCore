using Application.Interfaces;
using Infrastructure.DBContexts;
using Infrastructure.Entities;

namespace Application.Repository
{
    public class TelegramChatRepository : RepositoryGenerator<TelegramChat>, ITelegramChatRepository
    {
        public TelegramChatRepository(SampleDBContext context, SampleReadOnlyDBContext readOnlyDBContext) : base(context, readOnlyDBContext)
        {
        }
    }
}
