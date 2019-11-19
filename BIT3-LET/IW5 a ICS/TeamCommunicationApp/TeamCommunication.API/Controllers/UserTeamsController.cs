using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.BL;
using TeamCommunication.BL.Models;
using TeamCommunication.BL.Repositories;
using TeamCommunication.BL.Facades;
using TeamCommunication.BL.Queries;
using TeamCommunication.DAL;
using TeamCommunication.DAL.Entities;

namespace TeamCommunication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTeamsController : ControllerBase
    {
        private readonly TeamCommunicationDbContext _context;
        private readonly DbContextFactory factory;
        private readonly UserTeamRepository repo;
        private readonly GetUserTeamsQuery query;
        private readonly UserTeamFacade facade;
        private readonly TeamFacade teamFacade;
        private readonly TeamRepository teamRepo;
        private readonly GetTeamsQuery teamQuery;
        private readonly UserFacade userFacade;
        private readonly UserRepository userRepo;
        private readonly GetUsersQuery usersQuery;

        public UserTeamsController(TeamCommunicationDbContext context)
        {
            _context = context;
            this.factory = new DbContextFactory();
            this.repo = new UserTeamRepository(factory);
            this.query = new GetUserTeamsQuery(factory.CreateDbContext);
            this.facade = new UserTeamFacade(repo, query);
            this.teamRepo = new TeamRepository(factory);
            this.teamQuery = new GetTeamsQuery(factory.CreateDbContext);
            this.teamFacade = new TeamFacade(teamRepo, teamQuery);
            this.userRepo = new UserRepository(factory);
            this.usersQuery = new GetUsersQuery(factory.CreateDbContext);
            this.userFacade = new UserFacade(userRepo, usersQuery);
        }

        // GET: api/UserTeams
        [HttpGet]
        public ICollection<UserTeamDetailModel> GetUserTeams()
        {
            return facade.GetAll();
        }

        // GET: api/UserTeams/user/5
        [HttpGet("user/{id}")]
        public ICollection<TeamDetailModel> GetTeamsByUserId(Guid id)
        {
            var userTeam = facade.GetTeamsByUserId(id);
            ICollection<TeamDetailModel> teams = new List<TeamDetailModel>();

            foreach (var item in userTeam)
            {
                teams.Add(teamFacade.GetTeam(item.idTeam));
            }

            return teams;
        }

        // GET: api/UserTeams/team/5
        [HttpGet("team/{id}")]
        public ICollection<UserDetailModel> GetUsersByTeamId(Guid id)
        {
            var userTeam = facade.GetUsersByTeamId(id);
            ICollection<UserDetailModel> users = new List<UserDetailModel>();

            foreach (var item in userTeam)
            {
                users.Add(userFacade.GetUser(item.idUser));
            }

            return users;
        }

        // GET: api/UserTeams/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserTeamDetailModel>> GetUserTeam(Guid id)
        {
            var userTeam = facade.GetUserTeam(id);


            if (userTeam == null)
            {
                return NotFound();
            }

            return userTeam;
        }

        // PUT: api/UserTeams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserTeam(Guid id, UserTeamDetailModel userTeam)
        {
            if (id != userTeam.idTeam)
            {
                return BadRequest();
            }

            try
            {
                facade.Save(userTeam);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserTeamExists(id))
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

        // POST: api/UserTeams
        [HttpPost]
        public async Task<ActionResult<UserTeam>> PostUserTeam(UserTeamDetailModel userTeam)
        {
            UserTeamDetailModel model = facade.CreateNew();
            model.idTeam = userTeam.idTeam;
            model.idUser = userTeam.idUser;
            facade.Save(model);
            
            return Ok();
        }

        // DELETE: api/UserTeams/5
        [HttpDelete]
        public async Task<ActionResult<UserTeam>> DeleteUserTeam(UserTeamDetailModel userTeam)
        {

            var allUserTeams = facade.GetAll();
            Guid id = Guid.Empty;

            foreach (var item in allUserTeams)
            {
                if (item.idTeam == userTeam.idTeam && item.idUser == userTeam.idUser)
                {
                     id = item.Id;
                }
                
            }

            if (!UserTeamExists(id))
            {
                return NotFound();
            }

            facade.Remove(id);
            return Ok();
        }

        // GET: api/UserTeams/not/5
        [HttpGet("team/not/{id}")]
        public ICollection<UserDetailModel> GetUsersNotInTeam(Guid id)
        {

            var usersInTeam = facade.GetUsersByTeamId(id);
            var allUsers = userFacade.GetAll();

            return (from item in allUsers let isIn = usersInTeam.Any(usr => usr.idUser == item.Id) where !isIn select userFacade.GetUser(item.Id)).ToList();
        }

        private bool UserTeamExists(Guid id)
        {
            UserTeamDetailModel model = facade.GetUserTeam(id);
            return model == null ? false : true;
        }
    }
}
