using WebApi.Entity;

namespace WebApi.Services
{
    public interface IKeycloakRolesService
    {
        public Task<IReadOnlyCollection<Role>> GetRealmRoles();

        public Task<IReadOnlyCollection<Role>> GetClientRoles();
    }
}
