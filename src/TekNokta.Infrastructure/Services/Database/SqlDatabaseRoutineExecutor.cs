using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using TekNokta.Application.Common.Database.Parameters;
using TekNokta.Application.Common.Database.Results;
using TekNokta.Application.Services.Database;
using TekNokta.Infrastructure.Persistence;
using TekNokta.Infrastructure.Persistence.Routines;

namespace TekNokta.Infrastructure.Services.Database;

public sealed class SqlDatabaseRoutineExecutor(TekNoktaDbContext dbContext) : IDatabaseRoutineExecutor
{
    public async Task<DatabaseRoutineResult> ExecuteStoredProcedureAsync(
        string routineName,
        IEnumerable<DatabaseRoutineParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var routineParameters = parameters?.ToArray() ?? Array.Empty<DatabaseRoutineParameter>();
        var connection = dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        await using var command = connection.CreateCommand();
        command.CommandText = SqlRoutineNameFormatter.FormatRoutineName(routineName);
        command.CommandType = CommandType.StoredProcedure;
        AddParameters(command, routineParameters);

        try
        {
            if (shouldCloseConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }

            var rows = new List<IReadOnlyDictionary<string, object?>>();

            var recordsAffected = 0;

            await using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                do
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        rows.Add(ReadCurrentRow(reader));
                    }
                }
                while (await reader.NextResultAsync(cancellationToken));

                recordsAffected = reader.RecordsAffected;
            }

            return new DatabaseRoutineResult
            {
                RecordsAffected = recordsAffected,
                Rows = rows,
                OutputParameters = ReadOutputParameters(command)
            };
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    public async Task<DatabaseScalarResult<T>> ExecuteScalarFunctionAsync<T>(
        string functionName,
        IEnumerable<DatabaseRoutineParameter>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        var routineParameters = parameters?.ToArray() ?? Array.Empty<DatabaseRoutineParameter>();
        var connection = dbContext.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        await using var command = connection.CreateCommand();
        command.CommandText = CreateFunctionSql(functionName, routineParameters);
        command.CommandType = CommandType.Text;
        AddParameters(command, routineParameters);

        try
        {
            if (shouldCloseConnection)
            {
                await connection.OpenAsync(cancellationToken);
            }

            var value = await command.ExecuteScalarAsync(cancellationToken);

            return new DatabaseScalarResult<T>
            {
                Value = ConvertScalarValue<T>(value)
            };
        }
        finally
        {
            if (shouldCloseConnection)
            {
                await connection.CloseAsync();
            }
        }
    }

    private static void AddParameters(DbCommand command, IReadOnlyCollection<DatabaseRoutineParameter> parameters)
    {
        foreach (var parameter in parameters)
        {
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = SqlRoutineNameFormatter.FormatParameterName(parameter.Name);
            dbParameter.Value = parameter.Value ?? DBNull.Value;
            dbParameter.Direction = MapParameterDirection(parameter.Direction);

            if (parameter.Size.HasValue)
            {
                dbParameter.Size = parameter.Size.Value;
            }

            command.Parameters.Add(dbParameter);
        }
    }

    private static string CreateFunctionSql(
        string functionName,
        IReadOnlyCollection<DatabaseRoutineParameter> parameters)
    {
        var formattedFunctionName = SqlRoutineNameFormatter.FormatRoutineName(functionName);
        var parameterList = string.Join(
            ", ",
            parameters.Select(parameter => SqlRoutineNameFormatter.FormatParameterName(parameter.Name)));

        return $"SELECT {formattedFunctionName}({parameterList})";
    }

    private static Dictionary<string, object?> ReadCurrentRow(DbDataReader reader)
    {
        var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < reader.FieldCount; i++)
        {
            var value = reader.GetValue(i);
            row[reader.GetName(i)] = value == DBNull.Value ? null : value;
        }

        return row;
    }

    private static Dictionary<string, object?> ReadOutputParameters(DbCommand command)
    {
        var outputParameters = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (DbParameter parameter in command.Parameters)
        {
            if (parameter.Direction is ParameterDirection.Output
                or ParameterDirection.InputOutput
                or ParameterDirection.ReturnValue)
            {
                outputParameters[parameter.ParameterName.TrimStart('@')] =
                    parameter.Value == DBNull.Value ? null : parameter.Value;
            }
        }

        return outputParameters;
    }

    private static ParameterDirection MapParameterDirection(DatabaseRoutineParameterDirection direction)
    {
        return direction switch
        {
            DatabaseRoutineParameterDirection.Input => ParameterDirection.Input,
            DatabaseRoutineParameterDirection.Output => ParameterDirection.Output,
            DatabaseRoutineParameterDirection.InputOutput => ParameterDirection.InputOutput,
            DatabaseRoutineParameterDirection.ReturnValue => ParameterDirection.ReturnValue,
            _ => ParameterDirection.Input
        };
    }

    private static T? ConvertScalarValue<T>(object? value)
    {
        if (value is null or DBNull)
        {
            return default;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

        if (targetType.IsEnum)
        {
            return (T)Enum.ToObject(targetType, value);
        }

        return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}
