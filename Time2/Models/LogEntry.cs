using System.Text.RegularExpressions;

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


    public static LogEntry? TryParse(string input)
    {
        return new InvalidLogEntry(input, "Invalid entry");
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


    public static LogEntry? TryParse(string input)
    {
        return (CustomTime.TryParse(input) is CustomTime time
            && time.TotalMinutes > 0)
            ? new TimeLogEntry(time)
            : null;
    }
}

internal partial record LabelLogEntry(string Text)
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


    public static LogEntry? TryParse(string input)
    {
        return LabelRegex().IsMatch(input)
            ? new LabelLogEntry(input)
            : null;
    }


    [GeneratedRegex(@"^[_a-zA-ZåäöÅÄÖ]")]
    private static partial Regex LabelRegex();
}

internal partial record SubSegmentLogEntry(CustomTime TimeSpan, string Text)
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


    public static LogEntry? TryParse(string input)
    {
        var match = SubSegmentRegex().Match(input);
        if (match.Success == false)
        {
            return null;
        }

        var time = CustomTime.TryParse(match.Groups["timespan"].Value);
        if (time is null)
        {
            return null;
        }

        return new SubSegmentLogEntry(time, match.Groups["label"].Value);
    }


    [GeneratedRegex(@"^-(?<timespan>\d\d:\d\d)=>(?<label>[_a-zA-ZåäöÅÄÖ][_a-zA-ZåäöÅÄÖ0-9]+)$")]
    private static partial Regex SubSegmentRegex();
}

internal partial record ExtraSegmentLogEntry(CustomTime TimeSpan, string Text)
    : LogEntry
{
    public override string Display()
    {
        return Serialize();
    }

    public override string Serialize()
    {
        return $"+{TimeSpan}=>{Text}";
    }


    public static LogEntry? TryParse(string input)
    {
        var match = ExtraSegmentRegex().Match(input);
        if (match.Success == false)
        {
            return null;
        }

        var time = CustomTime.TryParse(match.Groups["timespan"].Value);
        if (time is null)
        {
            return null;
        }

        return new ExtraSegmentLogEntry(time, match.Groups["label"].Value);
    }


    [GeneratedRegex(@"^\+(?<timespan>\d\d:\d\d)=>(?<label>[_a-zA-ZåäöÅÄÖ][_a-zA-ZåäöÅÄÖ0-9]+)$")]
    private static partial Regex ExtraSegmentRegex();
}
