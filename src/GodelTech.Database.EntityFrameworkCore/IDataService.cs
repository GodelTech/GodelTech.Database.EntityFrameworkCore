using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: CLSCompliant(false)]
[assembly: InternalsVisibleTo("GodelTech.Database.EntityFrameworkCore.IntegrationTests")]
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
