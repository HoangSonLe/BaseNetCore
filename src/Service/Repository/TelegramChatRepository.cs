using Application.Interfaces;
using Infrastructure.DBContexts;
using Infrastructure.Entitites;

namespace Application.Repository
{
    public class TelegramChatRepository : RepositoryGenerator<TelegramChat>, ITelegramChatRepository
    {
        public TelegramChatRepository(SampleDBContext context, SampleReadOnlyDBContext readOnlyDBContext) : base(context, readOnlyDBContext)
        {
        }
    }
}
