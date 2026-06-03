namespace TekNokta.Application.Common.Database.Results;

public sealed class DatabaseRoutineResult
{
    public int RecordsAffected { get; init; }

    public IReadOnlyList<IReadOnlyDictionary<string, object?>> Rows { get; init; } =
        Array.Empty<IReadOnlyDictionary<string, object?>>();

    public IReadOnlyDictionary<string, object?> OutputParameters { get; init; } =
        new Dictionary<string, object?>();
}
