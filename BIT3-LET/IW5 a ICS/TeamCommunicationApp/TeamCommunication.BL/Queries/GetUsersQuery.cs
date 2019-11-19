using System;
using System.Collections.Generic;
using System.Linq;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL;

namespace TeamCommunication.BL.Queries
{
    public class GetUsersQuery
    {
        private readonly Func<TeamCommunicationDbContext> dbContextFactory;

        public GetUsersQuery(Func<TeamCommunicationDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        internal ICollection<UserListModel> Execute()
        {
            using (var context = dbContextFactory())
            {
                return context.Set<User>().Select(UserMapper.MapUserEntityToListModel).ToList();
            }
        }
    }
}
