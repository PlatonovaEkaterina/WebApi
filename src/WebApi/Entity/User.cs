using System.Text.Json.Serialization;

namespace WebApi.Entity
{
    public class User
    {
        public string KeycloakId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public List<string> ClientsRoles { get; set; }
        public List<string> RealmsRoles { get; set; }

        public Dictionary<string, List<string>> Attributes { get; set; }
    }

   /* public class Attributes
    {
        public string Birthdate { get; set; }

        public string Gender { get; set; }

        public string PhoneNumber { get; set; }

        public string MiddleName { get; set; }

        public string MaidenName { get; set; }

    }*/
}
