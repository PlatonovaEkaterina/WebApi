using AutoMapper;
using FS.Keycloak.RestApiClient.Model;
using WebApi.Entity;

namespace WebApi.Mapping
{
    public class Roles : Profile
    {
        public Roles()
        {
            CreateMap<RoleRepresentation, Role>()
                .ReverseMap();
        }
    }
}
