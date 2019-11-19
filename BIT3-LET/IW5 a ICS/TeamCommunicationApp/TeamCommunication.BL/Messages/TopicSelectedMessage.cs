using System;

namespace TeamCommunication.BL.Messages
{
    public class TopicSelectedMessage : IMessage
    {
        public Guid? Id { get; set; }
    }
}
