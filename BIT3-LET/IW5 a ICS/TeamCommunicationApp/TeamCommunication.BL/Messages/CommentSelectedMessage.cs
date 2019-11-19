using System;

namespace TeamCommunication.BL.Messages
{
    public class CommentSelectedMessage : IMessage
    {
        public Guid? Id { get; set; }
    }
}
