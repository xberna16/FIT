using System;
using System.Collections.Generic;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Facades
{
    public class CommentFacade
    {
        private readonly CommentRepository repository;
        private readonly GetCommentsQuery query;

        public CommentFacade(CommentRepository repository, GetCommentsQuery query)
        {
            this.repository = repository;
            this.query = query;
        }

        public CommentDetailModel CreateNew()
        {
            return new CommentDetailModel() { Id = Guid.Empty };
        }

        public CommentDetailModel GetComment(Guid id)
        {
            return repository.GetById(id);
        }

        public CommentDetailModel Save(CommentDetailModel model)
        {
            if (model.Id == Guid.Empty)
            {
                return repository.Add(model);
            }
            else
            {
                repository.Update(model);
                return model;
            }
        }

        public void Remove(Guid id)
        {
            repository.Remove(id);
        }

        public ICollection<CommentListModel> GetAll()
        {
            return query.Execute();
        }

        public ICollection<CommentListModel> GetCommentsByTopicId(Guid id)
        {
            return repository.GetCommentsByTopicId(id);
        }
    }
}
