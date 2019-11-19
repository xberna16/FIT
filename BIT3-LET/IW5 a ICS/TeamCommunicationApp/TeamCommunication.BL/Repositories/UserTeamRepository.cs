using System;
using System.Collections.Generic;
using System.Linq;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Mappers;

namespace TeamCommunication.BL.Repositories
{
    public class UserTeamRepository : IUserTeamRepository
    {
        private readonly IDbContextFactory dbContextFactory;

        public UserTeamRepository(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public UserTeamDetailModel GetById(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var userTeam = teamCommunicationDbContext.UserTeams.First(ut => ut.Id == id);

                return userTeam != null ? UserTeamMapper.MapUserTeamEntityToDetailModel(userTeam) : null;
            }
        }
        

        public ICollection<UserTeamDetailModel> GetTeamsByUserId(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var teams = teamCommunicationDbContext.UserTeams.Where(ut => ut.idUser == id).Select(UserTeamMapper.MapUserTeamEntityToDetailModel).ToList();
                return teams;
            }
        }

        public ICollection<UserTeamDetailModel> GetUsersByTeamId(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var users = teamCommunicationDbContext.UserTeams.Where(ut => ut.idTeam == id).Select(UserTeamMapper.MapUserTeamEntityToDetailModel).ToList();
                return users;
            }
        }

        public void Update(UserTeamDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var userTeam = teamCommunicationDbContext.UserTeams.First(ut => ut.Id == detail.Id);

                userTeam.idUser = detail.idUser;
                userTeam.idTeam = detail.idTeam;

                teamCommunicationDbContext.SaveChanges();
            }
        }
        
        public UserTeamDetailModel Add(UserTeamDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var userTeam = UserTeamMapper.MapUserTeamDetailModelToEntity(detail);
                userTeam.Id = Guid.NewGuid();

                teamCommunicationDbContext.UserTeams.Add(userTeam);
                teamCommunicationDbContext.SaveChanges();

                return UserTeamMapper.MapUserTeamEntityToDetailModel(userTeam);
            }
        }

        public void Remove(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var userTeam = teamCommunicationDbContext.UserTeams.First(ut => ut.Id == id);
                teamCommunicationDbContext.UserTeams.Attach(userTeam);

                teamCommunicationDbContext.Remove(userTeam);
                teamCommunicationDbContext.SaveChanges();
            }
        }
    }
}
