using Chronky.Models;

namespace Chronky.Services;
internal partial class LogEntryParser
{
    public static LogEntry ParseEntry(string entry)
    {
        if (string.IsNullOrWhiteSpace(entry) == false)
        {
            var parsers = new List<Func<string, LogEntry?>>()
            {
                TimeLogEntry.TryParse,
                LabelLogEntry.TryParse,
                SubSegmentLogEntry.TryParse,
                ExtraSegmentLogEntry.TryParse,
            };

            foreach (var parser in parsers)
            {
                if (parser.Invoke(entry) is LogEntry parsed)
                {
                    return parsed;
                }
            }
        }

        return new InvalidLogEntry(entry, "Invalid entry");
    }
}
