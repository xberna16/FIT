using System;
using System.Linq;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        private IDbContextFactory dbContextFactory;

        public TeamRepository(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public TeamDetailModel GetById(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var team = teamCommunicationDbContext.Teams.Find(id);

                return team != null ? TeamMapper.MapTeamEntityToDetailModel(team) : null;
            }
        }

        
        public void Update(TeamDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var team = teamCommunicationDbContext.Teams.First(t => t.Id == detail.Id);

                team.name = detail.name;
                team.description = detail.description;

                teamCommunicationDbContext.SaveChanges();
            }
        }

        public TeamDetailModel Add(TeamDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var team = TeamMapper.MapTeamDetailModelToEntity(detail);
                team.Id = Guid.NewGuid();

                teamCommunicationDbContext.Teams.Add(team);
                teamCommunicationDbContext.SaveChanges();

                return TeamMapper.MapTeamEntityToDetailModel(team);
            }
        }

        public void Remove(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var team = teamCommunicationDbContext.Teams.Find(id);
                teamCommunicationDbContext.Teams.Attach(team);
                teamCommunicationDbContext.Teams.Remove(team);
                teamCommunicationDbContext.SaveChanges();
            }
        }
    }
}
