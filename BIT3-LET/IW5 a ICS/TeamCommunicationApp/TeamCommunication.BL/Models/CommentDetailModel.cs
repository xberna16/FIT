using System;

namespace TeamCommunication.BL.Models
{
    public class CommentDetailModel : BaseModel
    {
        public string text { get; set; }
        public DateTime date { get; set; }

        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }
    }
}
