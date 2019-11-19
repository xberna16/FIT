using System;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public interface ITeamRepository
    {
        TeamDetailModel GetById(Guid id);
        void Update(TeamDetailModel team);
        TeamDetailModel Add(TeamDetailModel team);
        void Remove(Guid id);
    }
}
