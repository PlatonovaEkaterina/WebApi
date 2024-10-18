using System.Runtime.ExceptionServices;
using FS.Keycloak.RestApiClient.Model;
using WebApi.Entity;

namespace WebApi.Services
{
    public interface IKeycloakUsersService
    {
        public Task<IReadOnlyCollection<User>> GetUsers(int? first, int? max);

        public Task<string> CreateUser(User user);
        public Task UpdateUser(User user);

        public Task DeleteUser(string userKeycloakId);

        public Task SyncUsers(List<User> user);
    }
}