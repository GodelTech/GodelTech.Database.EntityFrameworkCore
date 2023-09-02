﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Interface of SQL executor.
    /// </summary>
    public interface ISqlExecutor
    {
        /// <summary>
        /// Executes the given SQL against the database and returns the number of rows affected.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <param name="sql">SQL to execute.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is the number of rows affected.</returns>
        Task<int> ExecuteSqlRawAsync(DbContext dbContext, string sql, CancellationToken cancellationToken = default);
    }
}
