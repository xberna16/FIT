using System;
using System.Collections.Generic;
using System.Linq;


namespace TeamCommunication.DAL.Entities
{
    public class Team : EntityBase
    {
        public string name { get; set; }
        public string description { get; set; }

        public virtual ICollection<UserTeam> UserTeams { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }

        private sealed class TeamEqualityComparer : IEqualityComparer<Team>
        {
            public bool Equals(Team x, Team y)
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

                return x.Id.Equals(y.Id) &&
                       string.Equals(x.name, y.name) &&
                       string.Equals(x.description, y.description) &&
                       x.UserTeams.OrderBy(idUser => idUser.Id).SequenceEqual(y.UserTeams.OrderBy(idUser => idUser.Id), UserTeam.UserTeamComparer) &&
                       x.Topics.OrderBy(idTopic => idTopic.Id).SequenceEqual(y.Topics.OrderBy(idTopic => idTopic.Id), Topic.TopicComparer) &&
                x.Id.Equals(y.Id);
            }

            public int GetHashCode(Team obj)
            {
                unchecked
                {
                    var hashCode = obj.Id.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.name != null ? obj.name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.description != null ? obj.description.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.UserTeams != null ? obj.UserTeams.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Topics != null ? obj.Topics.GetHashCode() : 0);
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<Team> TeamComparer { get; } = new TeamEqualityComparer();

        private sealed class WithoutUserEqualityComparer : IEqualityComparer<Team>
        {
            public bool Equals(Team x, Team y)
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
                return string.Equals(x.name, y.name) && string.Equals(x.description, y.description) && x.Id.Equals(y.Id);
            }


            public int GetHashCode(Team obj)
            {
                unchecked
                {
                    var hashCode = (obj.name != null ? obj.name.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.description != null ? obj.description.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Id.GetHashCode();
                    return hashCode;
                }
            }
        }
        public static IEqualityComparer<Team> WithoutUserComparer { get; } = new WithoutUserEqualityComparer();
    }
}