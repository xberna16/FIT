using System;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Mappers
{
    public static class TopicMapper
    {

        public static TopicListModel MapTopicEntityToListModel(Topic entity)
        {
            return new TopicListModel()
            {
                Id = entity.Id,
                heading = entity.heading,
                date = entity.date
            };
        }


        public static TopicDetailModel MapTopicEntityToDetailModel(Topic entity)
        {
            Guid Teamid = Guid.Empty;
            Guid Userid = Guid.Empty;
            if (entity.Team != null)
                Teamid = entity.Team.Id;
            if (entity.User != null)
                Userid = entity.User.Id;
            return new TopicDetailModel()
            {
                Id = entity.Id,
                date = entity.date,
                heading = entity.heading,
                TeamId = Teamid,
                text = entity.text,
                UserId = Userid
            };
        }

        public static Topic MapTopicDetailModelToEntity(TopicDetailModel model)
        {
            return new Topic()
            {
                Id = model.Id,
                date = model.date,
                heading = model.heading,
                text = model.text
            };
        }

    }
}
