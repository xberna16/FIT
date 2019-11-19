using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Tests
{
    public class TeamRepositoryTestsFixture
    {
        private readonly ITeamRepository repository;

        public TeamRepositoryTestsFixture()
        {
            repository = new TeamRepository(new InMemoryDbContextFactory());
        }

        public ITeamRepository Repository => repository;

    }
}