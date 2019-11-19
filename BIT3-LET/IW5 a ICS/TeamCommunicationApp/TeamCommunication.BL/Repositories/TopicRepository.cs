using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;

namespace TeamCommunication.BL.Repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly IDbContextFactory dbContextFactory;

        public TopicRepository(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public TopicDetailModel GetById(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var topic = teamCommunicationDbContext.Topics.Include(e => e.Team).Include(e => e.User).First(c => c.Id == id);

                return TopicMapper.MapTopicEntityToDetailModel(topic);
            }
        }
        
        public ICollection<TopicListModel> GetTopicsByTeamId(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var topics = teamCommunicationDbContext.Topics.Where(t => t.Team.Id == id).Select(TopicMapper.MapTopicEntityToListModel).ToList();
                return topics;
            }
        }

        public void Update(TopicDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var topic = teamCommunicationDbContext.Topics.First(t => t.Id == detail.Id);

                topic.heading = detail.heading;
                topic.text = detail.text;

                teamCommunicationDbContext.SaveChanges();
            }
        }

        public TopicDetailModel Add(TopicDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var topic = TopicMapper.MapTopicDetailModelToEntity(detail);
                topic.Id = Guid.NewGuid();
                var idUser = detail.UserId;
                var idTeam = detail.TeamId;
                if(idUser != Guid.Empty)
                    topic.User = teamCommunicationDbContext.Users.First(c => c.Id == idUser);
                if (idTeam != Guid.Empty)
                    topic.Team = teamCommunicationDbContext.Teams.First(c => c.Id == idTeam);

                teamCommunicationDbContext.Topics.Add(topic);
                teamCommunicationDbContext.SaveChanges();

                return TopicMapper.MapTopicEntityToDetailModel(topic);
            }
        }

        public void Remove(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var topic = teamCommunicationDbContext.Topics.Find(id);
                teamCommunicationDbContext.Topics.Attach(topic);
                teamCommunicationDbContext.Topics.Remove(topic);
                teamCommunicationDbContext.SaveChanges();
            }
        }
    }
}
