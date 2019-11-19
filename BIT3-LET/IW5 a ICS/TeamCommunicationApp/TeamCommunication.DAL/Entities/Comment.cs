using System;
using System.Collections.Generic;


namespace TeamCommunication.DAL.Entities
{
    public class Comment : EntityBase
    {
        public string text { get; set; }
        public DateTime date { get; set; }

        public virtual User User { get; set; }
        public virtual Topic Topic { get; set; }

        private sealed class CommentEqualityComparer : IEqualityComparer<Comment>
        {
            public bool Equals(Comment x, Comment y)
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
                return string.Equals(x.text, y.text) &&
                       string.Equals(x.date, y.date) &&
                       User.UserComparer.Equals(x.User, y.User) &&
                       Topic.WithoutCommentComparer.Equals(x.Topic, y.Topic) &&
                       x.Id.Equals(y.Id);
            }

            public int GetHashCode(Comment obj)
            {
                unchecked
                {
                    var hashCode = (obj.text != null ? obj.text.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.date != null ? obj.date.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.User != null ? obj.User.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ (obj.Topic != null ? obj.Topic.GetHashCode() : 0);
                    hashCode = (hashCode * 397) ^ obj.Id.GetHashCode();
                    return hashCode;
                }
            }
        }

        public static IEqualityComparer<Comment> CommentComparer { get; } = new CommentEqualityComparer();
    }
}