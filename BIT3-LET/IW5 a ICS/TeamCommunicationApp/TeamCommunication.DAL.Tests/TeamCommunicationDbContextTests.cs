using System;
using TeamCommunication.DAL.Entities;
using System.Linq;
using Xunit;

namespace TeamCommunication.DAL.Tests
{
    public class TeamCommunicationDbContextTests : IClassFixture<TeamCommunicationDbContextTestsClassSetupFixture>
    {
        public TeamCommunicationDbContextTests(TeamCommunicationDbContextTestsClassSetupFixture testContext)
        {
            _testContext = testContext;
        }

        private readonly TeamCommunicationDbContextTestsClassSetupFixture _testContext;

        [Fact]
        public void AddUserTest()
        {
            //Arrange
            var user = new User
            {
                firstName = "Jan",
                lastName = "Novak",
                nickname = "novic12345",
                email = "xnovak99@fit.vutbr.cz",
                password = "Qjadhqw54SDSdjsas"
            };

            //Act
            _testContext.TeamCommunicationDbContextSUT.Users.Add(user);
            _testContext.TeamCommunicationDbContextSUT.SaveChanges();


            //Assert
            using (var dbx = _testContext.CreateTeamCommunicationDbContext())
            {
                var retrievedUser = dbx.Users.First(entity => entity.Id == user.Id);
                Assert.Equal(user, retrievedUser, User.UserComparer);
            }
        }

        [Fact]
        public void AddTeamTest()
        {
            //Arrange
            var team = new Team
            {
                name = "Frajeri",
                description = "FIT studenti"
            };

            //Act
            _testContext.TeamCommunicationDbContextSUT.Teams.Add(team);
            _testContext.TeamCommunicationDbContextSUT.SaveChanges();


            //Assert
            using (var dbx = _testContext.CreateTeamCommunicationDbContext())
            {
                var retrievedTeam = dbx.Teams.First(entity => entity.Id == team.Id);
                Assert.Equal(team, retrievedTeam, Team.WithoutUserComparer);
            }
        }

        [Fact]
        public void AddTopicTest()
        {
            //Arrange
            var topic = new Topic
            {
                heading = "Dotaz",
                text = "Jak postavit letadlo?",
                date = new DateTime(2019, 4, 6, 20, 02, 37)
            };

            //Act
            _testContext.TeamCommunicationDbContextSUT.Topics.Add(topic);
            _testContext.TeamCommunicationDbContextSUT.SaveChanges();


            //Assert
            using (var dbx = _testContext.CreateTeamCommunicationDbContext())
            {
                var retrievedTopic = dbx.Topics.First(entity => entity.Id == topic.Id);
                Assert.Equal(topic, retrievedTopic, Topic.WithoutCommentComparer);
            }
        }

        [Fact]
        public void AddCommentTest()
        {
            //Arrange
            var comment = new Comment
            {
                text = "Musíš si ho postavit.",
                date = new DateTime(2019, 4, 6, 20, 10, 37)
            };

            //Act
            _testContext.TeamCommunicationDbContextSUT.Comments.Add(comment);
            _testContext.TeamCommunicationDbContextSUT.SaveChanges();


            //Assert
            using (var dbx = _testContext.CreateTeamCommunicationDbContext())
            {
                var retrievedTopic = dbx.Comments.First(entity => entity.Id == comment.Id);
                Assert.Equal(comment, retrievedTopic, Comment.CommentComparer);
            }
        }

        [Fact]
        public void AddUserTeamTest()
        {
            //Arrange
            var userTeam = new UserTeam
            {
                idUser = new Guid("ab8db9b3-799c-4ef2-9d85-ce32a9ffa843"),
                idTeam = new Guid("cb8db9b3-799c-4ef2-9d85-ce32a9ffa843")
            };

            //Act
            _testContext.TeamCommunicationDbContextSUT.UserTeams.Add(userTeam);
            _testContext.TeamCommunicationDbContextSUT.SaveChanges();


            //Assert
            using (var dbx = _testContext.CreateTeamCommunicationDbContext())
            {
                var retrievedUserTeam = dbx.UserTeams.First(entity => entity.Id == userTeam.Id);
                Assert.Equal(userTeam, retrievedUserTeam, UserTeam.UserTeamComparer);
            }
        }
    }
}
