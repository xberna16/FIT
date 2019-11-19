using TeamCommunication.BL.Models;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Mappers
{
    public class UserTeamMapper
    {
        public static UserTeamDetailModel MapUserTeamEntityToDetailModel(UserTeam entity)
        {
            return new UserTeamDetailModel
            {
                Id = entity.Id,
                idTeam = entity.idTeam,
                idUser = entity.idUser
            };
        }

        public static UserTeam MapUserTeamDetailModelToEntity(UserTeamDetailModel model)
        {
            return new UserTeam
            {
                Id = model.Id,
                idTeam = model.idTeam,
                idUser = model.idUser
            };
        }
    }
}
