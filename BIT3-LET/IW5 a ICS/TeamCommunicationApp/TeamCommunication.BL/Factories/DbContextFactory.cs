using TeamCommunication.DAL;
using Microsoft.EntityFrameworkCore;

namespace TeamCommunication.BL
{
    public class DbContextFactory: IDbContextFactory
    {
        public TeamCommunicationDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TeamCommunicationDbContext>();
            optionsBuilder.UseSqlServer(
                    @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog = TCAdb;MultipleActiveResultSets = True;Integrated Security = True");

            return new TeamCommunicationDbContext(optionsBuilder.Options);
        }
    }
}
