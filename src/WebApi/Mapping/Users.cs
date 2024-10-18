using AutoMapper;
using FS.Keycloak.RestApiClient.Model;
using WebApi.Entity;

namespace Mapping
{
    public class Users : Profile
    {
        public Users()
        {
            CreateMap<UserRepresentation, User>()
                .ForMember(x=>x.KeycloakId, m=>m.MapFrom(x=>x.Id))
                .ReverseMap();
        }
    }
}