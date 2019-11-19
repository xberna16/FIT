using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.DAL;
using TeamCommunication.BL;
using TeamCommunication.BL.Facades;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;
using TeamCommunication.BL.Queries;

namespace TeamCommunication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private readonly TeamCommunicationDbContext _context;
        private DbContextFactory factory;
        private readonly TopicRepository repo;
        private readonly CommentRepository commRepo;
        private readonly GetTopicsQuery query;
        private TopicFacade facade;

        public TopicsController(TeamCommunicationDbContext context)
        {
            _context = context;
            this.factory = new DbContextFactory();
            repo = new TopicRepository(factory);
            commRepo = new CommentRepository(factory);
            query = new GetTopicsQuery(factory.CreateDbContext);
            facade = new TopicFacade(repo, query);
        }

        // GET: api/Topics
        [HttpGet]
        public ICollection<TopicListModel> GetTopics()
        {
            return facade.GetAll();
        }

        //GET: api/Topics/comments/5
        [HttpGet("comments/{id}")]
        public ICollection<CommentListModel> GetCommentsByTopicId(Guid id)
        {
            return commRepo.GetCommentsByTopicId(id);
        }

        // GET: api/Topics/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TopicDetailModel>> GetTopic(Guid id)
        {
            var topic = facade.GetTopic(id);
            if (topic == null)
            {
                return NotFound();
            }
            return topic;
        }

        // PUT: api/Topics/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopic(Guid id, TopicDetailModel topic)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            _context.Entry(topic).State = EntityState.Modified;

            try
            {
                facade.Save(topic);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TopicExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Topics
        [HttpPost]
        public async Task<ActionResult<TopicDetailModel>> PostTopic(TopicDetailModel detail)
        {

            DateTime date = DateTime.Now;

            TopicDetailModel topic = facade.CreateNew();
            topic.TeamId = detail.TeamId;
            topic.UserId = detail.UserId;
            topic.date = date;
            topic.heading = detail.heading;
            topic.text = detail.text;
            if (facade.Save(topic) == null)
            {
                return NotFound();
            }
            
            return Ok();
        }

        // DELETE: api/Topics/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CommentDetailModel>> DeleteTopic(Guid id)
        {
            if (!TopicExists(id))
            {
                return NotFound();
            }

            facade.Remove(id);
            return Ok();
        }

        private bool TopicExists(Guid id)
        {
            TopicDetailModel topic = facade.GetTopic(id);
            return topic == null ? false : true;
        }
    }
}
