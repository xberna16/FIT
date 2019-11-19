using System;
using System.Collections.Generic;
using System.Linq;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL;

namespace TeamCommunication.BL.Queries
{
    public class GetTopicsQuery
    {
        private readonly Func<TeamCommunicationDbContext> dbContextFactory;

        public GetTopicsQuery(Func<TeamCommunicationDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        internal ICollection<TopicListModel> Execute()
        {
            using (var context = dbContextFactory())
            {
                return context.Set<Topic>().Select(TopicMapper.MapTopicEntityToListModel).ToList();
            }
        }
    }
}
