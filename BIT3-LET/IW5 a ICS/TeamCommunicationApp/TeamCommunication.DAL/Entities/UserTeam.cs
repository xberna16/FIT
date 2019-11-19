using System;
using System.Collections.Generic;
using System.Text;

namespace TeamCommunication.DAL.Entities
{
    public class UserTeam : EntityBase
    {
        public Guid idUser { get; set; }
        public Guid idTeam { get; set; }

        public virtual User User { get; set; }
        public virtual Team Team { get; set; }

        private sealed class UserTeamEqualityComparer : IEqualityComparer<UserTeam>
        {
            public bool Equals(UserTeam x, UserTeam y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }
                return string.Equals(x.idUser, y.idUser) &&
                       string.Equals(x.idTeam, y.idTeam) &&
                       Team.WithoutUserComparer.Equals(x.Team, y.Team) &&
                       User.UserComparer.Equals(x.User, y.User) &&
                       x.Id.Equals(y.Id);
            }

            public int GetHashCode(UserTeam obj)
            {
                unchecked
                {
                    var hashCode = (obj.idUser != null ? obj.idUser.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.idTeam != null ? obj.idTeam.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Id.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<UserTeam> UserTeamComparer { get; } = new UserTeamEqualityComparer();
    }
}