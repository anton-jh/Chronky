using Time2.Exceptions;

namespace Time2.Models;

internal class Log(DateTime created, IEnumerable<string> entries)
{
    private readonly List<string> _entries = entries.ToList();


    public IEnumerable<string> Entries => _entries;
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
        if (CursorPosition < _entries.Count - 1)
        {
            CursorPosition++;
        }
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

    public void Insert(string entry)
    {
        _entries.Insert(CursorPosition, entry);
        CursorPosition++;
    }
}
