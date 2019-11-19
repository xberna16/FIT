using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TeamCommunication.DAL
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TeamCommunicationDbContext>
    {
        public TeamCommunicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<TeamCommunicationDbContext>();
            optionsBuilder.UseSqlServer(
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog = TCAdb;MultipleActiveResultSets = True;Integrated Security = True");
            return new TeamCommunicationDbContext(optionsBuilder.Options);
        }
    }
}
