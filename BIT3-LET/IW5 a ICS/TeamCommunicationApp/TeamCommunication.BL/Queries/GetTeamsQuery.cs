using System;
using System.Collections.Generic;
using System.Linq;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL;

namespace TeamCommunication.BL.Queries
{
    public class GetTeamsQuery
    {
        private readonly Func<TeamCommunicationDbContext> dbContextFactory;

        public GetTeamsQuery(Func<TeamCommunicationDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        internal ICollection<TeamListModel> Execute()
        {
            using (var context = dbContextFactory())
            {
                return context.Set<Team>().Select(TeamMapper.MapTeamEntityToListModel).ToList();
            }
        }
    }
}
