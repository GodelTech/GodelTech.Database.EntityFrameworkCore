using System.Threading.Tasks;

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
