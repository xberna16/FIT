using System;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Mappers
{
    public class CommentMapper
    {
        public static CommentDetailModel MapCommentEntityToDetailModel(Comment entity)
        {
            Guid Topicid = Guid.Empty;
            Guid Userid = Guid.Empty;
            if (entity.Topic != null)
                Topicid = entity.Topic.Id;
            if (entity.User != null)
                Userid = entity.User.Id;
            return new CommentDetailModel
            {
                date = entity.date,
                Id = entity.Id,
                text = entity.text,
                TopicId = Topicid,
                UserId = Userid
            };
        }

        public static CommentListModel MapCommentEntityToListModel(Comment entity)
        {
            return new CommentListModel
            {
                Id = entity.Id,
                date = entity.date,
                text = entity.text
            };
        }

        public static Comment MapCommentDetailModelToEntity(CommentDetailModel model)
        {
            return new Comment
            {
                Id = model.Id,
                date = model.date,
                text = model.text,
            };
        }
    }
}
