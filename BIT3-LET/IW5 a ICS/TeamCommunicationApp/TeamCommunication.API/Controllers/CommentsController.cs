using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.DAL;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL;
using TeamCommunication.BL.Facades;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;
using TeamCommunication.BL.Queries;

namespace TeamCommunication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly TeamCommunicationDbContext _context;
        private DbContextFactory factory;
        private readonly CommentRepository repo;
        private readonly GetCommentsQuery query;
        private CommentFacade facade;

        public CommentsController(TeamCommunicationDbContext context)
        {
            _context = context;
            this.factory = new DbContextFactory();
            repo = new CommentRepository(factory);
            query = new GetCommentsQuery(factory.CreateDbContext);
            facade = new CommentFacade(repo, query);
        }

        // GET: api/Comments
        [HttpGet]
        public ICollection<CommentListModel> GetComments()
        {
            return facade.GetAll();
        }

        // GET: api/Comments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CommentDetailModel>> GetComment(Guid id)
        {
            var comment = facade.GetComment(id);
            if (comment == null)
            {
                return NotFound();
            }
            return comment;
        }

        // PUT: api/Comments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComment(Guid id, CommentDetailModel comment)
        {
            if (id != comment.Id)
            {
                return BadRequest();
            }

            try
            {
                facade.Save(comment);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentExists(id))
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

        // POST: api/Comments
        [HttpPost]
        public async Task<ActionResult<Comment>> PostComment(CommentDetailModel detail)
        {
            DateTime date = DateTime.Now;

            CommentDetailModel comment = facade.CreateNew();
            comment.text = detail.text;
            comment.TopicId = detail.TopicId;
            comment.UserId = detail.UserId;
            comment.date = date;
            if(facade.Save(comment) == null)
            {
                return NotFound();
            }
            
            return Ok();
        }

        // DELETE: api/Comments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<CommentDetailModel>> DeleteComment(Guid id)
        {
            if(!CommentExists(id))
            {
                return NotFound();
            }

            facade.Remove(id);
            return Ok();
        }

        private bool CommentExists(Guid id)
        {
            CommentDetailModel comment = facade.GetComment(id);
            return comment == null ? false : true;
        }
    }
}
