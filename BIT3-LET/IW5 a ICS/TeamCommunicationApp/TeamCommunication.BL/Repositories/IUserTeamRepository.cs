using System;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public interface IUserTeamRepository
    {
        UserTeamDetailModel GetById(Guid id);
        void Update(UserTeamDetailModel detail);
        UserTeamDetailModel Add(UserTeamDetailModel detail);
        void Remove(Guid id);
    }
}
