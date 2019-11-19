using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamCommunication.DAL;
using TeamCommunication.DAL.Entities;
using TeamCommunication.BL;
using TeamCommunication.BL.Facades;
using TeamCommunication.BL.Queries;
using TeamCommunication.BL.Repositories;
using TeamCommunication.BL.Models;

namespace TeamCommunication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TeamCommunicationDbContext _context;
        private DbContextFactory factory;
        private readonly UserRepository repo;
        private readonly GetUsersQuery query;
        private UserFacade facade;

        public UsersController(TeamCommunicationDbContext context)
        {
            _context = context;
            this.factory = new DbContextFactory();
            this.repo = new UserRepository(factory);
            this.query = new GetUsersQuery(factory.CreateDbContext);
            facade = new UserFacade(repo, query);
        }

        // GET: api/Users
        [HttpGet]
        public ICollection<UserListModel> GetUsers()
        {
            return facade.GetAll();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailModel>> GetUser(Guid id)
        {
            var user = facade.GetUser(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(Guid id, UserDetailModel user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            try
            {
                facade.Save(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
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

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDetailModel detail)
        {
            UserDetailModel user = facade.CreateNew();
            user.email = detail.email;
            user.firstName = detail.firstName;
            user.lastName = detail.lastName;
            user.nickname = detail.nickname;
            user.password = detail.password;
            user.salt = detail.salt;
            if(facade.Save(user) == null)
            {
                return NotFound();
            }
            
            return Ok();
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserDetailModel>> DeleteUser(Guid id)
        {
            if (!UserExists(id))
            {
                return NotFound();
            }

            facade.Remove(id);
            return Ok();
        }

        private bool UserExists(Guid id)
        {
            UserDetailModel user = facade.GetUser(id);
            return user == null ? false : true;
        }
    }
}
