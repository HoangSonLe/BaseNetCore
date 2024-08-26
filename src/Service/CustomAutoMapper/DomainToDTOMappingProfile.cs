using AutoMapper;
using Core.Models.ViewModels;
using Infrastructure.Entitites;

namespace Application.CustomAutoMapper
{
    public class DomainToDTOMappingProfile : Profile
    {
        public DomainToDTOMappingProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(dest => dest.UpdatedBy, opts => opts.MapFrom(i => i.UpdatedBy != null ? i.UpdatedBy : i.CreatedBy))
                .ForMember(dest => dest.UpdatedDate, opts => opts.MapFrom(i => i.UpdatedDate != null ? i.UpdatedDate : i.CreatedDate));
            //.ForMember(dest => dest.RoleName, opts => opts.MapFrom(i => string.Join(",", i.RoleList.Select(j => j.Name))));
            CreateMap<UserViewModel, User>();
            CreateMap<Role, RoleViewModel>();
            CreateMap<RoleViewModel, Role>();
        }
    }
}
