using System;

namespace TeamCommunication.BL.Messages
{
    public class UserSelectedMessage : IMessage
    {
        public Guid? Id { get; set; }
    }
}
