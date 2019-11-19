using System;
using System.Collections.Generic;
using System.Linq;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL;

namespace TeamCommunication.BL.Queries
{
    public class GetUserTeamsQuery
    {
        private readonly Func<TeamCommunicationDbContext> dbContextFactory;

        public GetUserTeamsQuery(Func<TeamCommunicationDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        internal ICollection<UserTeamDetailModel> Execute()
        {
            using (var context = dbContextFactory())
            {
                return context.Set<UserTeam>().Select(UserTeamMapper.MapUserTeamEntityToDetailModel).ToList();
            }
        }
    }
}
