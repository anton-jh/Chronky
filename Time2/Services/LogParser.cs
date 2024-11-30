using System.Text.RegularExpressions;
using Time2.Exceptions;
using Time2.Models;

namespace Time2.Services;
internal partial class LogEntryParser
{
    public static IEnumerable<LogEntry> ParseLine(string line)
    {
        return line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ParseEntry);
    }


    public static LogEntry ParseEntry(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry) == false)
        {
            var parsers = new List<Func<string, LogEntry?>>()
            {
                TryParseTime,
                TryParseLabel,
                TryParseSubSegment
            };

            foreach (var parser in parsers)
            {
                if (parser.Invoke(entry) is LogEntry parsed)
                {
                    return parsed;
                }
            }
        }

        throw new LogParsingException($"Invalid entry '{entry}'");
    }


    public static TimeLogEntry? TryParseTime(string line)
    {
        return (CustomTime.TryParse(line) is CustomTime time
            && time.TotalMinutes > 0)
            ? new(time)
            : null;
    }

    public static LabelLogEntry? TryParseLabel(string line)
    {
        return LabelRegex().IsMatch(line)
            ? new(line)
            : null;
    }

    public static SubSegmentLogEntry? TryParseSubSegment(string line)
    {
        var match = SubSegmentRegex().Match(line);
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


    [GeneratedRegex(@"^[_a-zA-ZåäöÅÄÖ]")]
    private static partial Regex LabelRegex();

    [GeneratedRegex(@"^-(?<timespan>\d\d:\d\d)=>(?<label>[_a-zA-ZåäöÅÄÖ]+)$")]
    private static partial Regex SubSegmentRegex();
}
