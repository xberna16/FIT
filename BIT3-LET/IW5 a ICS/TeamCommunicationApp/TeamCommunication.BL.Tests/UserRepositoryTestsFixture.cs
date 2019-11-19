using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Tests
{
    public class UserRepositoryTestsFixture
    {
        private readonly IUserRepository repository;

        public UserRepositoryTestsFixture()
        {
            repository = new UserRepository(new InMemoryDbContextFactory());
        }

        public IUserRepository Repository => repository;

    }
}