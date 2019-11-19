using System;
using System.Collections.Generic;
using System.Linq;


namespace TeamCommunication.DAL.Entities
{
    public class Topic : EntityBase
    {
        public string heading { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }
        public virtual Team Team { get; set; }
        public virtual User User { get; set; }

        private sealed class TopicEqualityComparer : IEqualityComparer<Topic>
        {
            public bool Equals(Topic x, Topic y)
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
                return string.Equals(x.heading, y.heading) &&
                       string.Equals(x.text, y.text) &&
                       DateTime.Equals(x.date, y.date) &&
                       Team.WithoutUserComparer.Equals(x.Team, y.Team) &&
                       User.UserComparer.Equals(x.User, y.User) &&
                       x.Comments.OrderBy(idComment => idComment.Id).SequenceEqual(y.Comments.OrderBy(idComment => idComment.Id), Comment.CommentComparer) &&
                       x.Id.Equals(y.Id);
            }


            public int GetHashCode(Topic obj)
            {
                unchecked
                {
                    var hashCode = (obj.heading != null ? obj.heading.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.text != null ? obj.text.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.date != null ? obj.date.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Team != null ? obj.Team.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.User != null ? obj.User.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Comments != null ? obj.Comments.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Id.GetHashCode();
                    return hashCode;
                }
            }
        }
        public static IEqualityComparer<Topic> TopicComparer { get; } = new TopicEqualityComparer();

        private sealed class WithoutCommentEqualityComparer : IEqualityComparer<Topic>
        {
            public bool Equals(Topic x, Topic y)
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
                return string.Equals(x.heading, y.heading) &&
                       string.Equals(x.text, y.text) &&
                       DateTime.Equals(x.date, y.date) &&
                       Team.WithoutUserComparer.Equals(x.Team, y.Team) &&
                       User.UserComparer.Equals(x.User, y.User) &&
                       x.Id.Equals(y.Id);
            }


            public int GetHashCode(Topic obj)
            {
                unchecked
                {
                    var hashCode = (obj.heading != null ? obj.heading.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.text != null ? obj.text.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.date != null ? obj.date.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Team != null ? obj.Team.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.User != null ? obj.User.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Id.GetHashCode();
                    return hashCode;
                }
            }
        }
        public static IEqualityComparer<Topic> WithoutCommentComparer { get; } = new WithoutCommentEqualityComparer();
    }
}