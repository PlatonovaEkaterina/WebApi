namespace WebApi.Entity
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public RoleAttributes Attributes { get; set; }
    }

    public class RoleAttributes
    {
        public string SecondName { get; set; }

        public string SecondDescription { get; set; }
    }
}
