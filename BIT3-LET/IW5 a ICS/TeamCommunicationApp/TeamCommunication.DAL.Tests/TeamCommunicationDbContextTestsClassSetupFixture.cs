using Microsoft.EntityFrameworkCore;
using System;

namespace TeamCommunication.DAL.Tests
{
    public class TeamCommunicationDbContextTestsClassSetupFixture : IDisposable
    {
        public TeamCommunicationDbContext TeamCommunicationDbContextSUT { get; set; }

        public TeamCommunicationDbContextTestsClassSetupFixture()
        {
            this.TeamCommunicationDbContextSUT = CreateTeamCommunicationDbContext();
        }

        public TeamCommunicationDbContext CreateTeamCommunicationDbContext()
        {
            return new TeamCommunicationDbContext(CreateDbContextOptions());
        }

        private DbContextOptions<TeamCommunicationDbContext> CreateDbContextOptions()
        {
            var contextOptionsBuilder = new DbContextOptionsBuilder<TeamCommunicationDbContext>();
            contextOptionsBuilder.UseInMemoryDatabase("TeamCommunication");
            return contextOptionsBuilder.Options;
        }

        public void Dispose()
        {
            TeamCommunicationDbContextSUT?.Dispose();
        }
    }
}