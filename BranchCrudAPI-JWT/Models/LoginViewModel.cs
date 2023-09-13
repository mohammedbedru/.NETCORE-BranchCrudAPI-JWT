namespace BranchCrudAPI_JWT.Models
{
    public class LoginViewModel
    {
        //selecting fields from User model needed for login 
        //we use this model for login
        public string Username { get; set; }
        public string Password { get; set; }

        public LoginViewModel()
        {
            
        }
    }
}
