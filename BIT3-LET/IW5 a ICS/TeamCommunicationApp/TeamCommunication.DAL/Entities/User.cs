using System;
using System.Collections.Generic;
using System.Linq;


namespace TeamCommunication.DAL.Entities
{
    public class User : EntityBase
    {
        public string email { get; set; }
        public string password { get; set; }
        public string nickname { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string salt { get; set; }

        public virtual ICollection<UserTeam> UserTeams { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        private sealed class UserEqualityComparer : IEqualityComparer<User>
        {
            public bool Equals(User x, User y)
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
                return string.Equals(x.lastName, y.lastName) &&
                       string.Equals(x.nickname, y.nickname) &&
                       x.Id.Equals(y.Id);
            }


            public int GetHashCode(User obj)
            {
                unchecked
                {
                    var hashCode = (obj.lastName != null ? obj.lastName.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.nickname != null ? obj.nickname.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.UserTeams != null ? obj.UserTeams.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Topics != null ? obj.Topics.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Comments != null ? obj.Comments.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Id.GetHashCode();
                    return hashCode;
                }
            }
        }
        public static IEqualityComparer<User> UserComparer { get; } = new UserEqualityComparer();
    }
}