using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Interface of Database utility.
    /// </summary>
    public interface IDatabaseUtility
    {
        /// <summary>
        /// Opens the connection.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task OpenConnectionAsync(DbContext dbContext, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the given SQL against the database and returns the number of rows affected.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows affected.</returns>
        Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default);

        /// <summary>
        /// Closes the connection.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        Task CloseConnectionAsync(DbContext dbContext);
    }
}
