using TeamCommunication.BL.Models;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Mappers
{
    public class UserMapper
    {
        public static UserListModel MapUserEntityToListModel(User entity)
        {
            return new UserListModel
            {
                Id = entity.Id,
                nickname = entity.nickname
            };
        }
        public static UserDetailModel MapUserEntityToDetailModel(User entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new UserDetailModel
            {
                Id = entity.Id,
                nickname = entity.nickname,
                email = entity.email,
                firstName = entity.firstName,
                lastName = entity.lastName,
                password = entity.password,
                salt = entity.salt
            };
        }

        public static User MapUserDetailModelToEntity(UserDetailModel model)
        {
            return new User
            {
                Id = model.Id,
                email = model.email,
                firstName = model.firstName,
                lastName = model.lastName,
                nickname = model.nickname,
                password = model.password,
                salt = model.salt
            };
        }
    }
}
