using System;
using System.Linq;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory dbContextFactory;

        public UserRepository(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public UserDetailModel GetById(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var user = teamCommunicationDbContext.Find<User>(id);

                return user != null ? UserMapper.MapUserEntityToDetailModel(user) : null;
            }
        }


        public UserDetailModel GetByEmail(string email)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                User user = null;
                try
                {
                    user = teamCommunicationDbContext.Users.First(usr => usr.email == email);
                }
                catch
                {
                }

                return user != null ? UserMapper.MapUserEntityToDetailModel(user) : null;
            }
        }

        public void Update(UserDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var user = teamCommunicationDbContext.Users.First(u => u.Id == detail.Id);

                user.firstName = detail.firstName;
                user.email = detail.email;
                user.lastName = detail.lastName;
                user.nickname = detail.nickname;
                user.password = detail.password;

                teamCommunicationDbContext.SaveChanges();
            }
        }

        public UserDetailModel Add(UserDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var user = UserMapper.MapUserDetailModelToEntity(detail);
                user.Id = Guid.NewGuid();

                teamCommunicationDbContext.Users.Add(user);
                teamCommunicationDbContext.SaveChanges();

                return UserMapper.MapUserEntityToDetailModel(user);
            }
        }

        public void Remove(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var user = teamCommunicationDbContext.Users.Find(id);
                teamCommunicationDbContext.Users.Attach(user);

                teamCommunicationDbContext.Users.Remove(user);
                teamCommunicationDbContext.SaveChanges();
            }
        }
    }
}
