namespace TekNokta.Application.Common.Database.Parameters;

public sealed class DbRoutineParameter
{
    private string name = string.Empty;

    public DbRoutineParameter()
    {
    }

    public DbRoutineParameter(string name, object? value)
    {
        Name = name;
        Value = value;
    }

    public string Name
    {
        get => name;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Parameter name cannot be null or empty.", nameof(value));
            }

            name = value;
        }
    }

    public object? Value { get; set; }
}
