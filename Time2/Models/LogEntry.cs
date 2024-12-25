namespace Time2.Models;

internal abstract record LogEntry
{
    public abstract string Serialize();
    public abstract string Display();
}


internal record InvalidLogEntry(string Value, string ErrorMessage)
    : LogEntry
{
    public override string Display()
    {
        return Value;
    }

    public override string Serialize()
    {
        return Value;
    }
}

internal record TimeLogEntry(CustomTime Time)
    : LogEntry
{
    public override string Display()
    {
        return Serialize();
    }

    public override string Serialize()
    {
        return Time.ToString();
    }
}

internal record LabelLogEntry(string Text)
    : LogEntry
{
    public override string Display()
    {
        return $"\t{Serialize()}";
    }

    public override string Serialize()
    {
        return Text;
    }
}

internal record SubSegmentLogEntry(CustomTime TimeSpan, string Text)
    : LogEntry
{
    public override string Display()
    {
        return $"\t{Serialize()}";
    }

    public override string Serialize()
    {
        return $"-{TimeSpan}=>{Text}";
    }
}
