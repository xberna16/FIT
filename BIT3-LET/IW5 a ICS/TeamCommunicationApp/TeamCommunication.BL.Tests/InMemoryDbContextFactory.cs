using TeamCommunication.DAL;
using Microsoft.EntityFrameworkCore;

namespace TeamCommunication.BL.Tests
{
    public class InMemoryDbContextFactory : IDbContextFactory
    {
        public TeamCommunicationDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TeamCommunicationDbContext>();
            optionsBuilder.UseInMemoryDatabase("TeamcommunicationDB");
            return new TeamCommunicationDbContext(optionsBuilder.Options);
        }
    }
}
