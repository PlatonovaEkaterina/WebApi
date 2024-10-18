using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebApi.Entity;
using WebApi.Services;

namespace WebApi.Workers
{
    public sealed class UsersSynchronizationService(IHttpClientFactory httpClientFactory, IOptionsSnapshot<Settings.Settings> namedOptionsAccessor,
     ILogger<UsersSynchronizationService> logger, IKeycloakUsersService keycloakService) : IScopedProcessingService
    {
        private int _executionCount;
        private Settings.Settings _settings = namedOptionsAccessor.Get(Settings.Settings.ShopSettings);

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var httpClient = httpClientFactory.CreateClient())
                {
                    var response = await httpClient.GetAsync($"{_settings.Url}/api/v1/users", stoppingToken);

                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                            //Нужно будет добавить UserDB (когда появится у Лизы),
                            //десериализовать в List<UserDB> и потом уже смапить на то, что будет храниться в Keycloak

                            var users = response.Content.ReadFromJsonAsync<List<User>>(stoppingToken).Result;
                            await keycloakService.SyncUsers(users);
                 
                    }
                    if(response.StatusCode == HttpStatusCode.InternalServerError)
                    {
                        logger.LogError("Failed to get users from Keycloak");
                    }
                   
                }

                ++_executionCount;

                logger.LogInformation(
                    "{ServiceName} working, execution count: {Count}",
                    nameof(UsersSynchronizationService),
                    _executionCount);

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}