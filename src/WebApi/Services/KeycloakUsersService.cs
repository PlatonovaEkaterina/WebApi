using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using AutoMapper;
using FS.Keycloak.RestApiClient.Api;
using FS.Keycloak.RestApiClient.Authentication.Client;
using FS.Keycloak.RestApiClient.Authentication.ClientFactory;
using FS.Keycloak.RestApiClient.Authentication.Flow;
using FS.Keycloak.RestApiClient.ClientFactory;
using FS.Keycloak.RestApiClient.Model;
using Mapping;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using WebApi.Entity;
using WebApi.Settings;

namespace WebApi.Services
{
    public class KeycloakUsersService : IKeycloakUsersService
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

        public KeycloakUsersService(IConfiguration configuration, IOptionsSnapshot<Settings.Settings> namedOptionsAccessor,
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
        /// Получение пользователей из Keycloak
        /// </summary>
        /// <param name="first"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public async Task<IReadOnlyCollection<User>> GetUsers(int? first = null, int? max = null)
        {
            using var httpClient = AuthenticationHttpClientFactory.Create(_credentials);
            using var usersApi = ApiClientFactory.Create<UsersApi>(httpClient);
            var keycloakUsers = new List<UserRepresentation>();
            keycloakUsers = await usersApi.GetUsersAsync(realm: realm, first: first, max: max);

            var users = _mapper.Map<List<User>>(keycloakUsers);

            _logger.LogInformation("Users received successfully");

            return users;
        }

        /// <summary>
        /// Создание пользователя в Keycloak. Возвращает Id (KeycloakId) нового пользователя в Keycloak
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> CreateUser(User user)
        {
            try
            {
                using var httpClient = AuthenticationHttpClientFactory.Create(_credentials);
                using var usersApi = ApiClientFactory.Create<UsersApi>(httpClient);

                var keycloakUser = _mapper.Map<UserRepresentation>(user);
                //keycloakUser.Username = $"{keycloakUser.FirstName}{keycloakUser.LastName}";

                var apiResponse = await usersApi.PostUsersWithHttpInfoAsync(realm, keycloakUser);

                var locationHeaderValue = apiResponse.Headers.First(x=>x.Key=="Location").Value;

                var lastIndexOf = locationHeaderValue[0].LastIndexOf('/');
                var guid = locationHeaderValue[0].Substring(lastIndexOf + 1);

                _logger.LogInformation("Users received successfully");

                return guid;
            }
            catch(Exception ex) {
                _logger.LogError("Failed creating user", ex);
                throw ex;
            }
            
        }

        /// <summary>
        /// Обновление инф о пользователе в Keycloak
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task UpdateUser(User user)
        {
            try
            {
                using var httpClient = AuthenticationHttpClientFactory.Create(_credentials);
                using var usersApi = ApiClientFactory.Create<UsersApi>(httpClient);

                var userRepresentation = _mapper.Map<UserRepresentation>(user);
               // await usersApi.PutUsersByUserIdAsync(realm, user.KeycloakId);

                var updatedInfo = await usersApi.PutUsersByUserIdWithHttpInfoAsync(realm, user.KeycloakId, userRepresentation);

                var t = updatedInfo;
                _logger.LogInformation("Users received successfully");
            }
            catch(Exception ex)
            {
                _logger.LogError("Field to update user", ex);
            }
        }

        /// <summary>
        /// Удаление пользовтеля из Keycloak
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task DeleteUser(string userKeycloakId)
        {
            using var httpClient = AuthenticationHttpClientFactory.Create(_credentials);
            using var usersApi = ApiClientFactory.Create<UsersApi>(httpClient);

            await usersApi.DeleteUsersByUserIdAsync(realm, userKeycloakId);
        }

        /// <summary>
        /// Синхронизация пользователей из бд и Keycloak
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task SyncUsers(List<User> dbUser)
        {
            try
            {
                var keycloackUsers = await GetUsers();
                var users = _mapper.Map<List<User>>(keycloackUsers);
                
                //проходимся по пользователям из БД и смотрим, если ли они в Keycloak, нужно ли удалять/обновлять
                foreach (var user in dbUser)
                {
                   // user.Username = user.FirstName.ToLower()+user.LastName.ToLower(); //для теста
                    if (!users.Any(x=>x.Username==user.Username)) //users.Contains(user)
                    {
                        await CreateUser(user);
                    }
                    else
                    {
                        //Добавить проверку: был ли пользователь помечен, как удаленный
                        /*if(user.IsDeleted)
                        {
                            await DeleteUser(user);
                        }*/
                        user.KeycloakId = users.First(x => x.Username == user.Username).KeycloakId; //надо будет убрать (Только для теста)
                        await UpdateUser(user);
                    }
                }
                //проходим по пользователям из keycloak и если какого-то нет в БД, то удалям его из keycloak
                foreach (var user in keycloackUsers)
                {
                    if(!dbUser.Any(x=>x.Username == user.Username) && user.Username!="katya")
                    {
                       // await DeleteUser(user.KeycloakId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to synchronizing users", ex);
            }
            

        }
    }
}