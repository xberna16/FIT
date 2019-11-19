using TeamCommunication.BL.Models;

namespace TeamCommunication.BL
{
    public interface IPasswordControl
    {
        UserDetailModel Registration(UserDetailModel model);
        UserDetailModel Login(UserDetailModel model);
    }
}
