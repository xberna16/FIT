using System;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public interface IUserRepository
    {
        UserDetailModel GetById(Guid id);
        void Update(UserDetailModel user);
        UserDetailModel Add(UserDetailModel user);
        void Remove(Guid id);
    }
}
