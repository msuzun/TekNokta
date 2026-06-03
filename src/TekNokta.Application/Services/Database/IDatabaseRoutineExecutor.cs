using TekNokta.Application.Common.Database.Parameters;
using TekNokta.Application.Common.Database.Results;

namespace TekNokta.Application.Services.Database;

public interface IDatabaseRoutineExecutor
{
    Task<DatabaseRoutineResult> ExecuteStoredProcedureAsync(
        string routineName,
        IEnumerable<DatabaseRoutineParameter>? parameters = null,
        CancellationToken cancellationToken = default);

    Task<DatabaseScalarResult<T>> ExecuteScalarFunctionAsync<T>(
        string functionName,
        IEnumerable<DatabaseRoutineParameter>? parameters = null,
        CancellationToken cancellationToken = default);
}
