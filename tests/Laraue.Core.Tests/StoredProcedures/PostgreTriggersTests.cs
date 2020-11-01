using Xunit;

namespace Laraue.Core.Tests.StoredProcedures
{
    public class PostgreTriggersTests
    {
        private readonly TestDbContext _dbContext;

        public PostgreTriggersTests()
        {
            _dbContext = new ContextFactory().CreatePgDbContext();
        }

        /// <summary>
        /// Draft variant of translating trigger expressions to query.
        /// </summary>
        [Fact]
        public void GenerateTriggerQuery()
        {

        }
    }
}
