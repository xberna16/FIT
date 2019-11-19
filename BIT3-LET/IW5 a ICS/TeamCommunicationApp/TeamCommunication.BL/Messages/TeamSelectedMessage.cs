using System;

namespace TeamCommunication.BL.Messages
{
    public class TeamSelectedMessage : IMessage
    {
        public Guid? Id { get; set; }
    }
}
