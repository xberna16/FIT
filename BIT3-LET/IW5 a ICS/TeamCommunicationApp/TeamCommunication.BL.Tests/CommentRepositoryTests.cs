using System;
using TeamCommunication.BL.Models;
using Xunit;

namespace TeamCommunication.BL.Tests
{
    public class CommentRepositoryTests : IClassFixture<CommentRepositoryTestsFixture>
    {
        private readonly CommentRepositoryTestsFixture fixture;

        public CommentRepositoryTests(CommentRepositoryTestsFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Create_WithNonExistingItem_DoesNotThrow()
        {
            var model = new CommentDetailModel
            {
                text = "Testovací komentář",
                date = new DateTime(2019, 4, 6, 20, 02, 37)
            };

            var returnedModel = fixture.Repository.Add(model);
            Assert.NotNull(returnedModel);

            var model1 = new CommentDetailModel
            {
                text = "Testovací komentář1",
                date = new DateTime(2019, 4, 6, 20, 02, 37)
            };

            var returnedModel1 = fixture.Repository.Add(model1);

            returnedModel1 = fixture.Repository.Add(model1);

            fixture.Repository.Update(returnedModel);
            Assert.NotNull(returnedModel);

            var returnedModel1After = fixture.Repository.GetById(returnedModel1.Id);
            Assert.Equal(returnedModel1.text, returnedModel1After.text);

            fixture.Repository.Remove(returnedModel.Id);
            fixture.Repository.Remove(returnedModel1.Id);
        }
    }
}
