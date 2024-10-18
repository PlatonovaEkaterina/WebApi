
namespace WebApi.Settings
{
    public class Settings
    {
        public const string KeycloakSettings = "KeycloakSettings";

        public const string ShopSettings = "ShopSettings";

        public string Url { get; set; }

        public string Realm { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Guid ClientUuid { get;set;}
    }
}
