using System;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public interface ICommentRepository
    {
        CommentDetailModel GetById(Guid id);
        void Update(CommentDetailModel comment);
        CommentDetailModel Add(CommentDetailModel comment);
        void Remove(Guid id);
    }
}
