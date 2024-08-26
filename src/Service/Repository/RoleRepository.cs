using Application.Interfaces;
using Infrastructure.DBContexts;
using Infrastructure.Entitites;

namespace Application.Repository
{
    public class RoleRepository : RepositoryGenerator<Role>, IRoleRepository
    {
        public RoleRepository(SampleDBContext context, SampleReadOnlyDBContext readOnlyDBContext) : base(context, readOnlyDBContext)
        {

        }
    }
}
