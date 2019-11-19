using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.DAL;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Facades;
using TeamCommunication.BL.Repositories;
using TeamCommunication.BL.Models;

namespace TeamCommunication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly TeamCommunicationDbContext _context;
        DbContextFactory factory;
        private readonly TeamRepository repo;
        private readonly TopicRepository topicRepo;
        private readonly GetTeamsQuery query;
        private TeamFacade facade;

        public TeamsController(TeamCommunicationDbContext context)
        {
            _context = context;
            this.factory = new DbContextFactory();
            this.query = new GetTeamsQuery(factory.CreateDbContext);
            this.topicRepo = new TopicRepository(factory);
            this.repo = new TeamRepository(factory);
            this.facade = new TeamFacade(repo, query);
        }

        // GET: api/Teams
        [HttpGet]
        public ICollection<TeamListModel> GetTeams()
        {
            return facade.GetAll();
        }

        //GET: api/Teams/topics/5
        [HttpGet("topics/{id}")]
        public ICollection<TopicListModel> GetTopicsByTeamId(Guid id)
        {
            return topicRepo.GetTopicsByTeamId(id);
        }

        // GET: api/Teams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TeamDetailModel>> GetTeam(Guid id)
        {
            var team = facade.GetTeam(id);
            if (team == null)
            {
                return NotFound();
            }
            return team;
        }

        // PUT: api/Teams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTeam(Guid id, TeamDetailModel team)
        {
            if (id != team.Id)
            {
                return BadRequest();
            }

            try
            {
                facade.Save(team);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeamExists(id))
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

        // POST: api/Teams
        [HttpPost]
        public async Task<ActionResult<Team>> PostTeam(TeamDetailModel detail)
        {
            TeamDetailModel team = facade.CreateNew();
            team.description = detail.description;
            team.name = detail.name;
            facade.Save(team);
            
            return Ok();
        }

        // DELETE: api/Teams/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TeamDetailModel>> DeleteTeam(Guid id)
        {
            if (!TeamExists(id))
            {
                return NotFound();
            }

            facade.Remove(id);
            return Ok();
        }

        private bool TeamExists(Guid id)
        {
            TeamDetailModel team = facade.GetTeam(id);
            return team == null ? false : true;
        }
    }
}
