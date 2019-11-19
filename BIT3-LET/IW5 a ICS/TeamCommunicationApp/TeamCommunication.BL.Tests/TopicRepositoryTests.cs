using TeamCommunication.BL.Models;
using Xunit;

namespace TeamCommunication.BL.Tests
{
    public class TopicRepositoryTests : IClassFixture<TopicRepositoryTestsFixture>
    {
        private readonly TopicRepositoryTestsFixture _fixture;

        public TopicRepositoryTests(TopicRepositoryTestsFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Create_WithNonExistingItem_DoesNotThrow()
        {
            var model = new TopicDetailModel
            {
                heading = "Testovací téma",
                text = "Téma k InMemory testu"
            };

            var returnedModel = _fixture.Repository.Add(model);

            var model1 = new TopicDetailModel
            {
                heading = "Testovací téma1",
                text = "Téma1 k InMemory testu"
            };

            var returnedModel1 = _fixture.Repository.Add(model1);

            Assert.Equal(returnedModel1, returnedModel1);
            Assert.NotNull(returnedModel);

            _fixture.Repository.Update(returnedModel);
            Assert.NotNull(returnedModel);

            var returnedModel1After = _fixture.Repository.GetById(returnedModel1.Id);
            Assert.Equal(returnedModel1.heading, returnedModel1After.heading);

            _fixture.Repository.Remove(returnedModel.Id);
            _fixture.Repository.Remove(returnedModel1.Id);
        }
    }
}
