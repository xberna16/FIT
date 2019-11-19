using TeamCommunication.BL.Models;
using System;
using Xunit;

namespace TeamCommunication.BL.Tests
{
    public class TeamRepositoryTests : IClassFixture<TeamRepositoryTestsFixture>
    {
        private readonly TeamRepositoryTestsFixture fixture;

        public TeamRepositoryTests(TeamRepositoryTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Create_WithNonExistingItem_DoesNotThrow_Team()
        {
            var model = new TeamDetailModel
            {
                Id = Guid.NewGuid(),
                name = "Testovací team",
                description = "Team sloužící k testu"
            };

            var returnedModel = fixture.Repository.Add(model);

            fixture.Repository.Update(returnedModel);
            Assert.NotNull(returnedModel);

            var returnedModel_after = fixture.Repository.GetById(returnedModel.Id);
            Assert.Equal(returnedModel.name, returnedModel_after.name);

            fixture.Repository.Remove(returnedModel.Id);
        }
    }
}