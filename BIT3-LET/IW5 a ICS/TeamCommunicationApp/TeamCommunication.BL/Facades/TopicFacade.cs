using System;
using System.Collections.Generic;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Facades
{
    public class TopicFacade
    {
        private readonly TopicRepository repository;
        private readonly GetTopicsQuery query;

        public TopicFacade(TopicRepository repository, GetTopicsQuery query)
        {
            this.repository = repository;
            this.query = query;
        }

        public TopicDetailModel CreateNew()
        {
            return new TopicDetailModel() { Id = Guid.Empty };
        }

        public TopicDetailModel GetTopic(Guid id)
        {
            return repository.GetById(id);
        }

        public TopicDetailModel Save(TopicDetailModel model)
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

        public ICollection<TopicListModel> GetAll()
        {
            return query.Execute();
        }

    }
}
