using System.ComponentModel.DataAnnotations;

namespace BranchCrudAPI_JWT.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

        public User()
        {

        }
    }
}
