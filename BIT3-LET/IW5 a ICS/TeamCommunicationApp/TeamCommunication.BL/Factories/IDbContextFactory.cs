using TeamCommunication.DAL;

namespace TeamCommunication.BL
{
    public interface IDbContextFactory
    {
        TeamCommunicationDbContext CreateDbContext();
    }
}
