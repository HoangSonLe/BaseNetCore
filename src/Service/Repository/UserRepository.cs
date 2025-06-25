using Application.Interfaces;
using Infrastructure.DBContexts;
using Infrastructure.Entities;

namespace Application.Repository
{
    public class UserRepository : RepositoryGenerator<User>, IUserRepository
    {
        public UserRepository(SampleDBContext context, SampleReadOnlyDBContext readOnlyDBContext) : base(context, readOnlyDBContext)
        {
        }
    }
}
