using System;

namespace TeamCommunication.BL.Models
{
    public class TopicDetailModel :BaseModel
    {
        public string heading { get; set; }
        public string text { get; set; }
        public DateTime date { get; set; }
        
        public  Guid TeamId { get; set; }
        public  Guid UserId { get; set; }
    }
}
