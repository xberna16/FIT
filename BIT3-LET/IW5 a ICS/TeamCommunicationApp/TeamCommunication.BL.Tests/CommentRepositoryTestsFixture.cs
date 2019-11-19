using TeamCommunication.BL.Repositories;

namespace TeamCommunication.BL.Tests
{
    public class CommentRepositoryTestsFixture
    {
        private readonly ICommentRepository repository;

        public CommentRepositoryTestsFixture()
        {
            repository = new CommentRepository(new InMemoryDbContextFactory());
        }

        public ICommentRepository Repository => repository;

    }
}