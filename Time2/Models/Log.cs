using Time2.Exceptions;

namespace Time2.Models;

internal class Log(DateTime created, IEnumerable<LogEntry> entries)
{
    private readonly List<LogEntry> _entries = entries.ToList();


    public IEnumerable<LogEntry> Entries => _entries;
    public DateTime Created { get; } = created;
    public int CursorPosition { get; private set; }


    public void MoveCursorUp()
    {
        if (CursorPosition > 0)
        {
            CursorPosition--;
        }
    }

    public void MoveCursorDown()
    {
        if (CursorPosition < _entries.Count) // no (- 1), allow passing the end
        {
            CursorPosition++;
        }
    }

    public void RemoveSelectedEntry()
    {
        if (_entries.Count == 0 || CursorPosition == -1 || CursorPosition > _entries.Count - 1)
        {
            return;
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
            (_entries[CursorPosition + 1], _entries[CursorPosition]) = (_entries[CursorPosition], _entries[CursorPosition + 1]);
        }
    }

    public void Insert(LogEntry entry)
    {
        _entries.Insert(CursorPosition, entry);
        CursorPosition++;
    }
}


// TODO:
// allow cursorPos == -1 to insert before and == length to insert after.
// render with for loop to allow rendering cursor outside of log?
