using System;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public interface ITopicRepository
    {
        TopicDetailModel GetById(Guid id);
        void Update(TopicDetailModel topic);
        TopicDetailModel Add(TopicDetailModel topic);
        void Remove(Guid id);
    }
}
