using System.Threading;
using System.Threading.Tasks;

namespace ITRepairService.Services;

public interface IDepartmentInitializerService
{
    /// <summary>
    /// Ensures that a department exists in the master data.
    /// If the department does not exist, it will be added automatically.
    /// </summary>
    /// <param name="departmentName">The department name from AD</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the department was newly added, false if it already existed</returns>
    Task<bool> EnsureDepartmentExistsAsync(string departmentName, CancellationToken cancellationToken = default);
}