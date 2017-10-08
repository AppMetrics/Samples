using System;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics.Health;

namespace HealthCheck.Samples
{
    public interface IDatabase
    {
        void Ping();
    }

    public class Database : IDatabase
    {
        public void Ping()
        {
            throw new NotImplementedException();
        }
    }

    public class DatabaseHealthCheck : App.Metrics.Health.HealthCheck
    {
        private readonly IDatabase _database;

        public DatabaseHealthCheck(IDatabase database)
            : base("Referencing Assembly - DatabaseCheck")
        {
            _database = database;
        }

        protected override ValueTask<HealthCheckResult> CheckAsync(CancellationToken token = default)
        {
            // exceptions will be caught and the result will be unhealthy
            _database.Ping();

            return new ValueTask<HealthCheckResult>(HealthCheckResult.Healthy());
        }
    }
}