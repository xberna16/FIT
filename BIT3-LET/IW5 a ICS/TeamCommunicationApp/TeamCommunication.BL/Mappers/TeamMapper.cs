using TeamCommunication.BL.Models;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Mappers
{
    public class TeamMapper
    {
        public static TeamDetailModel MapTeamEntityToDetailModel(Team entity)
        {
            return new TeamDetailModel
            {
                description = entity.description,
                Id = entity.Id,
                name = entity.name
            };
        }

        public static TeamListModel MapTeamEntityToListModel(Team entity)
        {
            return new TeamListModel
            {
                Id = entity.Id,
                name = entity.name
            };
        }

        public static Team MapTeamDetailModelToEntity(TeamDetailModel model)
        {
            return new Team
            {
                Id = model.Id,
                description = model.description,
                name = model.name
            };
        }

    }
}
