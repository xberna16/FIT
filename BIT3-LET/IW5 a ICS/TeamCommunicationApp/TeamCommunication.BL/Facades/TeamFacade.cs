using System;
using System.Collections.Generic;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Facades
{
    public class TeamFacade
    {
        private readonly TeamRepository repository;
        private readonly GetTeamsQuery query;

        public TeamFacade(TeamRepository repository, GetTeamsQuery query)
        {
            this.repository = repository;
            this.query = query;
        }

        public TeamDetailModel CreateNew()
        {
            return new TeamDetailModel() { Id = Guid.Empty };
        }

        public TeamDetailModel GetTeam(Guid id)
        {
            return repository.GetById(id);
        }

        public TeamDetailModel Save(TeamDetailModel model)
        {
            if(model.Id == Guid.Empty)
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

        public ICollection<TeamListModel> GetAll()
        {
            return query.Execute();
        }
    }
}
