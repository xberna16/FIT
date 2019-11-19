using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.BL.Mappers;
using TeamCommunication.BL.Models;
using TeamCommunication.DAL;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.BL.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly IDbContextFactory dbContextFactory;
       
        public CommentRepository(IDbContextFactory dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public CommentDetailModel GetById(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var comment = teamCommunicationDbContext.Comments.Include(e => e.User).Include(e => e.Topic).First(c => c.Id == id); ;
                Console.WriteLine("COMM HERE:");
                if (comment.User != null)
                    Console.WriteLine(comment.User.Id);
                Console.WriteLine("TOPIC HERE:");
                if (comment.Topic != null)
                    Console.WriteLine(comment.Topic.Id);

                return CommentMapper.MapCommentEntityToDetailModel(comment);
            }
        }

        public ICollection<CommentListModel> GetCommentsByTopicId(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var comms = teamCommunicationDbContext.Comments.Where(c => c.Topic.Id == id).Select(CommentMapper.MapCommentEntityToListModel).ToList();
                return comms;
            }
        }

        public void Update(CommentDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var comment = teamCommunicationDbContext.Comments.First(c => c.Id == detail.Id);

                comment.text = detail.text;
                comment.date = detail.date;

                teamCommunicationDbContext.SaveChanges();
            }
        }

        public CommentDetailModel Add(CommentDetailModel detail)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {

                var comment = CommentMapper.MapCommentDetailModelToEntity(detail);
                comment.Id = Guid.NewGuid();
               
                
                Guid idUser = detail.UserId;
                Console.WriteLine("NEW ID:");
                Console.WriteLine(idUser);
                Guid idTopic = detail.TopicId;
                Console.WriteLine("NEW TOPIC ID:");
                Console.WriteLine(idTopic);
                if (idUser != Guid.Empty)
                    comment.User = teamCommunicationDbContext.Users.First(c => c.Id == idUser);
                if (idTopic != Guid.Empty)
                    comment.Topic = teamCommunicationDbContext.Topics.First(c => c.Id == idTopic);
                teamCommunicationDbContext.Comments.Add(comment);
                teamCommunicationDbContext.SaveChanges();

                return CommentMapper.MapCommentEntityToDetailModel(comment);
            }
        }

        public void Remove(Guid id)
        {
            using (var teamCommunicationDbContext = dbContextFactory.CreateDbContext())
            {
                var comment = teamCommunicationDbContext.Comments.Find(id);
                teamCommunicationDbContext.Comments.Attach(comment);
                teamCommunicationDbContext.Comments.Remove(comment);
                teamCommunicationDbContext.SaveChanges();
            }
        }
    }
}
