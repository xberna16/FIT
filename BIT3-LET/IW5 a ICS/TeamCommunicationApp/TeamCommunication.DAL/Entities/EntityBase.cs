﻿using System;

namespace TeamCommunication.DAL.Entities
{
    public abstract class EntityBase : IEntity
    {
        public Guid Id { get; set; }
    }
}
