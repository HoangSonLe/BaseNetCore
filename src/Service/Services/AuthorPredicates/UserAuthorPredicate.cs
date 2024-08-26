using Core.Enums;
using Infrastructure.Entitites;
using LinqKit;
using System.Linq.Expressions;

namespace Application.Services.AuthorPredicates
{
    public static class UserAuthorPredicate
    {
        public static Expression<Func<User, bool>> GetUserAuthorPredicate(Expression<Func<User, bool>> predicate, List<ERoleType> roleList, int currentUserId = -1)
        {
            var predicateInner = PredicateBuilder.New(predicate);
            //Query theo từng chùa

            //Phân quyền theo role
            if (roleList.Contains(ERoleType.SystemAdmin))
            {
                predicateInner = predicateInner.And(i => i.RoleIdList.Contains((int)ERoleType.Admin));
            }
            else if (roleList.Contains(ERoleType.User))
            {
                predicateInner = predicateInner.And(i => false);
            }
            else if (roleList.Contains(ERoleType.Admin))
            {
                predicateInner = predicateInner.And(i => i.RoleIdList.Contains((int)ERoleType.Reporter) || i.RoleIdList.Contains((int)ERoleType.User));
            }
            else if (roleList.Contains(ERoleType.Reporter))
            {
                predicateInner = predicateInner.And(i => i.RoleIdList.Contains((int)ERoleType.User));
            }
            return predicateInner;
        }
    }
}
