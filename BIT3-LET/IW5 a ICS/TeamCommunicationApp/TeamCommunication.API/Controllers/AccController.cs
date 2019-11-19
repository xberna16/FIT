using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TeamCommunication.DAL;
using TeamCommunication.BL.Models;
using TeamCommunication.BL;

namespace TeamCommunication.API.Controllers
{
    [ApiController]
    public class AccController : ControllerBase
    {
        private readonly TeamCommunicationDbContext _context;

        DbContextFactory cont;

        public AccController(TeamCommunicationDbContext context)
        {
            _context = context;
            cont = new DbContextFactory();
        }
        [Route("api/[controller]/log")]
        [HttpPost]
        public async Task<ActionResult<Guid>> ULogin(UserDetailModel credentials)
        {
            PasswordControl pwctl = new PasswordControl(cont);
            UserDetailModel result = pwctl.Login(credentials);
            if (result != null)
                return Ok(result.Id);
            else
                return Unauthorized();
        }
        [Route("api/[controller]/reg")]
        [HttpPost]
        public async Task<ActionResult<UserDetailModel>> URegister(UserDetailModel credentials)
        {
            PasswordControl pwctl = new PasswordControl(cont);
            pwctl.Registration(credentials);
            return Ok();
        }
    }
}
