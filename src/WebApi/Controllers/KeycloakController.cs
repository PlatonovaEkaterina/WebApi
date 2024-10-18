using System.Net.Http;
using FS.Keycloak.RestApiClient.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using WebApi.Entity;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class KeycloakController : ControllerBase
    {
        private readonly ILogger<KeycloakUsersService> _logger;
        private readonly IKeycloakUsersService _usersService;
        private readonly IKeycloakRolesService _rolesService;

        public KeycloakController(IKeycloakUsersService usersService, IKeycloakRolesService rolesService, ILogger<KeycloakUsersService> logger)
        {
            _logger = logger;
            _usersService = usersService;
            _rolesService = rolesService;
        }

        [OutputCache(Duration = 2678400)]
        [HttpGet("users")]
        public async Task<ActionResult<User>> GetUsers([FromQuery] int first = 1, [FromQuery] int max = 3)
        {
            try
            {
                var users = await _usersService.GetUsers(first, max);
                return Ok(users);
            }
            catch(HttpRequestException ex)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get users", ex);

                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message
                });
            }
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        { 
            try
            {
               /*Когда появится норм модель, надо создать UserDTO и мапить на User*/
                var id = await _usersService.CreateUser(user);

                return Ok(id);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to create user", ex);

                return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message
                });
            }
        }

        [HttpPut("users")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            try
            {
                //Когда появится норм модель, созать UserDB и мапить на User
                await _usersService.UpdateUser(user);

                return Ok();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to update user", ex);

                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }
        }

        [HttpDelete("users/{userKeycloakId}")]
        public async Task<IActionResult> DeleteUser(string userKeycloakId) //Когда появится норм модель, надо создать UserDTO и мапить на User
        { 
            try
            {
                await _usersService.DeleteUser(userKeycloakId);

                return Ok();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to delete user", ex);

               return StatusCode(500, new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = ex.Message
                });
            }
        }

        [OutputCache(Duration = 2678400)]
        [HttpGet("realm/roles")]
        public async Task<ActionResult<Role>> GetRealmRoles()
        {
            try
            {
                var roles = await _rolesService.GetRealmRoles();
                return Ok(roles);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get realm roles", ex);

                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }
        }

        [OutputCache(Duration = 2678400)]
        [HttpGet("client/roles")]
        public async Task<ActionResult<Role>> GetClientRole()
        {
            try
            {
                var roles = await _rolesService.GetClientRoles();

                return Ok(roles);
            }
            catch(HttpRequestException ex)
            {
                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to get client roles", ex);

                return StatusCode(500, new ProblemDetails { Status = 500, Title = "Internal Server Error", Detail = ex.Message });
            }

        }
    }
}