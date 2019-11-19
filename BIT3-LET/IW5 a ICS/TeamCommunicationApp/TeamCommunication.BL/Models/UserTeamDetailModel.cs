using System;

namespace TeamCommunication.BL.Models
{
    public class UserTeamDetailModel : BaseModel
    {
        public Guid idUser { get; set; }
        public Guid idTeam { get; set; }

    }
}
