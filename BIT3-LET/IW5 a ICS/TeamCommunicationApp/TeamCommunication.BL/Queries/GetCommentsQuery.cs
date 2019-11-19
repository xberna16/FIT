using System;
using System.Collections.Generic;
using System.Linq;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL;

namespace TeamCommunication.BL.Queries
{
    public class GetCommentsQuery
    {
        private readonly Func<TeamCommunicationDbContext> dbContextFactory;

        public GetCommentsQuery(Func<TeamCommunicationDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        internal ICollection<CommentListModel> Execute()
        {
            using (var context = dbContextFactory())
            {
                return context.Set<Comment>().Select(CommentMapper.MapCommentEntityToListModel).ToList();
            }
        }
    }
}
