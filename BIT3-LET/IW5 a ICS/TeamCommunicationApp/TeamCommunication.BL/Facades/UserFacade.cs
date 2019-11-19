using System;
using System.Collections.Generic;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Facades
{
    public class UserFacade
    {
        private readonly UserRepository repository;
        private readonly GetUsersQuery query;

        public UserFacade(UserRepository repository, GetUsersQuery query)
        {
            this.repository = repository;
            this.query = query;
        }

        public UserDetailModel CreateNew()
        {
            return new UserDetailModel() { Id = Guid.Empty };
        }

        public UserDetailModel GetUser(Guid id)
        {
            return repository.GetById(id);
        }

        public UserDetailModel GetUserByEmail(string email)
        {
            return repository.GetByEmail(email);
        }

        public UserDetailModel Save(UserDetailModel model)
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

        public ICollection<UserListModel> GetAll()
        {
            return query.Execute();
        }
    }
}
