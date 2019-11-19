using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Tests
{
    public class TopicRepositoryTestsFixture
    {
        public TopicRepositoryTestsFixture()
        {
            Repository = new TopicRepository(new InMemoryDbContextFactory());
        }

        public ITopicRepository Repository { get; }

    }
}