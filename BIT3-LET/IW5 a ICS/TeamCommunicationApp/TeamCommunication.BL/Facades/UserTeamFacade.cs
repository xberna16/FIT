using System;
using System.Collections.Generic;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Facades
{
    public class UserTeamFacade
    {
        private readonly UserTeamRepository repository;
        private readonly GetUserTeamsQuery query;

        public UserTeamFacade(UserTeamRepository repository, GetUserTeamsQuery query)
        {
            this.repository = repository;
            this.query = query;
        }

        public UserTeamDetailModel CreateNew()
        {
            return new UserTeamDetailModel() { Id = Guid.Empty };
        }

        public UserTeamDetailModel GetUserTeam(Guid id)
        {
            return repository.GetById(id);
        }

        public UserTeamDetailModel Save(UserTeamDetailModel model)
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

        public ICollection<UserTeamDetailModel> GetAll()
        {
            return query.Execute();
        }

        public ICollection<UserTeamDetailModel> GetTeamsByUserId(Guid id)
        {
            return repository.GetTeamsByUserId(id);
        }

        public ICollection<UserTeamDetailModel> GetUsersByTeamId(Guid id)
        {
            return repository.GetUsersByTeamId(id);
        }
    }
}
