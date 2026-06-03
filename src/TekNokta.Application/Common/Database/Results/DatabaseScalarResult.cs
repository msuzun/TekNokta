namespace TekNokta.Application.Common.Database.Results;

public sealed class DatabaseScalarResult<T>
{
    public T? Value { get; init; }
}
