using Time2.Services;

namespace Time2.Models;

internal class Log(DateTime created, IEnumerable<LogEntry> entries)
{
    private readonly List<LogEntry> _entries = entries.ToList();


    public IEnumerable<LogEntry> Entries => _entries;
    public DateTime Created { get; } = created;


    public void Append(LogEntry entry)
    {
        _entries.Add(entry);
    }

    /// <summary>
    /// Probably temporary
    /// </summary>
    public void RemoveLast()
    {
        _entries.Remove(_entries.Last());
    }

    public IEnumerable<string> Display()
    {
        var lines = _entries
            .Select(x => x.Display())
            .ToList();

        if (lines.Count == 0)
        {
            return [];
        }

        var longestLine = lines
            .Select(x => x.Length)
            .Max();

        var validationResult = new LogValidator().Validate(_entries);

        if (validationResult.Error is LogValidator.LogValidatorError error)
        {
            var line = lines[error.LineNumber];
            lines[error.LineNumber] = $"{line}{new string(' ', longestLine - line.Length)} !!{error.Message}!!";
        }

        return lines;
    }
}
