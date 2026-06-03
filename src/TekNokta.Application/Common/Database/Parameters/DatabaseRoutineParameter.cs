namespace TekNokta.Application.Common.Database.Parameters;

public sealed record DatabaseRoutineParameter(
    string Name,
    object? Value = null,
    DatabaseRoutineParameterDirection Direction = DatabaseRoutineParameterDirection.Input,
    int? Size = null);
