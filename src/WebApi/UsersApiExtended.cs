using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Client;
using FS.Keycloak.RestApiClient.Model;

namespace WebApi
{
    public class UsersApiExtended : UsersApi, IApiAccessor, IUsersApi, IUsersApiSync, IUsersApiAsync
    {
        public UsersApiExtended(HttpClient client, Configuration configuration, HttpClientHandler handler = null): base(client, configuration, handler)
        {
           
        }

        public async Task<UserRepresentation> PostUser(string realm, UserRepresentation userRepresentation)
        {
            return new UserRepresentation();
        }
    }
}