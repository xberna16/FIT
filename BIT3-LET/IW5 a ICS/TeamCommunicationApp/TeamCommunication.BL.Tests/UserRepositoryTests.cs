using TeamCommunication.BL.Models;
using Xunit;

namespace TeamCommunication.BL.Tests
{
    public class UserRepositoryTests : IClassFixture<UserRepositoryTestsFixture>
    {
        private readonly UserRepositoryTestsFixture fixture;

        public UserRepositoryTests(UserRepositoryTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Create_WithNonExistingItem_DoesNotThrow()
        {
            var model = new UserDetailModel
            {
                firstName = "František",
                lastName = "Test",
                nickname = "Tester007",
                password = "QasdjaoqwAads65d6asd"
            };

            var returnedModel = fixture.Repository.Add(model);
            Assert.NotNull(returnedModel);

            var model1 = new UserDetailModel
            {
                firstName = "Karel",
                lastName = "Test1",
                nickname = "Tester009",
                password = "QasdjaoqwAads65d6asd"
            };

            var returnedModel1 = fixture.Repository.Add(model1);

            fixture.Repository.Update(returnedModel);
            Assert.NotNull(returnedModel);

            var returnedModel1After = fixture.Repository.GetById(returnedModel1.Id);
            Assert.Equal(returnedModel1.nickname, returnedModel1After.nickname);

            fixture.Repository.Remove(returnedModel.Id);
            fixture.Repository.Remove(returnedModel1.Id);
        }
    }
}
