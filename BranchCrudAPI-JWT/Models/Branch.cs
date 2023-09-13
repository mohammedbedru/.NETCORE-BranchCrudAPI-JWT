namespace BranchCrudAPI_JWT.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }

        public Branch()
        {

        }
    }
}
