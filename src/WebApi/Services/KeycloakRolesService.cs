using AutoMapper;
using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.ClientFactory;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.Keycloak.RestApiClient.Client;
using Microsoft.Extensions.Options;
using System.Net;
using WebApi.Entity;

namespace WebApi.Services
{
    public class KeycloakRolesService: IKeycloakRolesService
    {
        private const string UsersCacheKey = "users";
        private const string RealmRolesCacheKey = "realm_roles";
        private const string ClientRolesCacheKey = "client_roles";
        private readonly string realm = "master";

        private readonly IConfiguration Configuration;
        private IMapper _mapper;
        private ILogger _logger;
        private Settings.Settings _settings;
        private PasswordGrantFlow _credentials;

        public KeycloakRolesService(IConfiguration configuration, IOptionsSnapshot<Settings.Settings> namedOptionsAccessor,
          IMapper mapper, ILogger<KeycloakUsersService> logger)
        {
            Configuration = configuration;
            _settings = namedOptionsAccessor.Get(Settings.Settings.KeycloakSettings);
            _mapper = mapper;
            _logger = logger;

            _credentials = new PasswordGrantFlow
            {
                KeycloakUrl = _settings.Url,
                Realm = _settings.Realm,
                UserName = _settings.Username,
                Password = _settings.Password,
            };
        }

        /// <summary>
        /// Получение Realm ролей (ролей области, в которой находится наш clinet (приложение))
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Role>> GetRealmRoles()
        {
            using var httpClient = AuthenticationHttpClientFactory.Create(_credentials);
            using var rolesApi = ApiClientFactory.Create<RolesApi>(httpClient);

            var keycloakRoles = await rolesApi.GetRolesAsync(realm);

            var roles = _mapper.Map<List<Role>>(keycloakRoles);

            _logger.LogInformation("Realm roles received successfully");

            return roles;
        }

        /// <summary>
        /// Получение клиентских ролей (ролей нашего приложения)
        /// </summary>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<Role>> GetClientRoles()
        {
            using var httpClient = AuthenticationHttpClientFactory.Create(_credentials);
            using var rolesApi = ApiClientFactory.Create<RolesApi>(httpClient);

            var keycloakRoles =
                await rolesApi.GetClientsRolesByClientUuidAsync(realm, _settings.ClientUuid.ToString());

            var roles = _mapper.Map<List<Role>>(keycloakRoles);

            _logger.LogInformation("Client roles received successfully");

            return roles;
        }
    }
}
