using System;
using System.Threading.Tasks;

[assembly: CLSCompliant(true)]
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
        Task ApplyDataAsync();
    }
}
