using System;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(false)]
namespace GodelTech.Database.EntityFrameworkCore
{
    /// <summary>
    /// Interface of data service.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        /// Apply data.
        /// </summary>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        Task ApplyDataAsync(CancellationToken cancellationToken = default);
    }
}
