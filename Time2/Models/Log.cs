using Time2.Exceptions;
using Time2.Services;

namespace Time2.Models;

internal class Log(DateTime created, IEnumerable<LogEntry> entries)
{
    private readonly List<LogEntry> _entries = entries.ToList();


    public IEnumerable<LogEntry> Entries => _entries;
    public DateTime Created { get; } = created;
    public int CursorPosition { get; private set; } = 3;


    public void Append(LogEntry entry)
    {
        _entries.Add(entry);
    }

    public void MoveCursorUp()
    {
        if (CursorPosition > 0)
        {
            CursorPosition--;
        }
    }

    public void MoveCursorDown()
    {
        if (CursorPosition < _entries.Count - 1)
        {
            CursorPosition++;
        }
    }

    public LogEntry? GetSelectedEntry()
    {
        if (_entries.Count == 0 || CursorPosition == -1)
        {
            return null;
        }

        return _entries[CursorPosition];
    }

    public void ReplaceSelectedEntry(LogEntry entry)
    {
        if (_entries.Count == 0 || CursorPosition == -1)
        {
            throw new InvalidLogOperationException("No selected entry to replace");
        }

        _entries[CursorPosition] = entry;
    }

    public void RemoveSelectedEntry()
    {
        if (_entries.Count == 0 || CursorPosition == -1)
        {
            throw new InvalidLogOperationException("No selected entry to remove");
        }

        _entries.RemoveAt(CursorPosition);
        CursorPosition--;
        if (CursorPosition == -1)
        {
            CursorPosition = 0;
        }
    }

    public void MoveSelectedEntryUp()
    {
        if (_entries.Count >= 2 && CursorPosition > 0)
        {
            (_entries[CursorPosition - 1], _entries[CursorPosition]) = (_entries[CursorPosition], _entries[CursorPosition - 1]);
        }
    }

    public void MoveSelectedEntryDown()
    {
        if (_entries.Count >= 2 && CursorPosition < _entries.Count - 1)
        {
            (_entries[CursorPosition - 1], _entries[CursorPosition]) = (_entries[CursorPosition], _entries[CursorPosition - 1]);
        }
    }

    public IEnumerable<string> Display(bool showCursor) // todo maybe move this up a layer and print directly?
    {
        var lines = _entries
            .Select((x, i) => $"{(showCursor && CursorPosition == i ? "> " : "  ")}{x.Display()}")
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
