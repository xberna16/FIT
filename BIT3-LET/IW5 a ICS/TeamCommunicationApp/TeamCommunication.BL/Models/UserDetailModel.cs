namespace TeamCommunication.BL.Models
{
    public class UserDetailModel : BaseModel
    {
        public string email { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string salt { get; set; }
    }
}
