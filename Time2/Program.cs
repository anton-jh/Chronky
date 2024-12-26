using Time2.Exceptions;
using Time2.Models;
using Time2.Services;

var preProcessors = new List<PreProcessor>()
{
    new ShortFormTimePreProcessor(),
    new NowPreProcessor(),
    new SubSegmentPreProcessor(),
};

var log = LogFileManager.Load()
    ?? new Log(DateTime.Now, []);

bool insertMode = true;

while (true)
{
    var validationResult = new LogValidator().Validate(log.Entries);
    LogRenderer.Render(log, insertMode, validationResult, Console.BufferWidth);

    if (insertMode)
    {
        Console.CursorLeft = 2;
        Console.CursorTop = log.CursorPosition + 2;
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            insertMode = false;
        }
        else
        {
            try
            {
                var entries = ParseLine(input);
                foreach (var entry in entries)
                {
                    log.Insert(LogEntryParser.ParseEntry(entry));
                }
            }
            catch (LogParsingException ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadKey(true);
            }
        }
    }
    else
    {
        Console.CursorTop = log.CursorPosition + 1;
        Console.CursorLeft = 0;
        var keyInfo = Console.ReadKey(intercept: true);
        switch (keyInfo.Key)
        {
            case ConsoleKey.UpArrow when keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt):
                log.MoveSelectedEntryUp();
                break;
            case ConsoleKey.DownArrow when keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt):
                log.MoveSelectedEntryDown();
                break;
            case ConsoleKey.UpArrow:
                log.MoveCursorUp();
                break;
            case ConsoleKey.DownArrow:
                log.MoveCursorDown();
                break;
            case ConsoleKey.Backspace:
                var pos = log.CursorPosition;
                log.RemoveSelectedEntry();
                if (log.CursorPosition == pos)
                {
                    log.MoveCursorUp();
                }
                break;
            case ConsoleKey.Escape:
            case ConsoleKey.Enter:
                insertMode = true;
                break;
        }
    }

    LogFileManager.Save(log);
}


IEnumerable<string> ParseLine(string line)
{
    var words = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var entries = words
        .Select(PreProcess);

    return entries;
}

string PreProcess(string word)
{
    foreach (var preProcessor in preProcessors)
    {
        word = preProcessor.Process(word);
    }

    return word;
}

// TODO: +segments (extra-segments) keep these at the top of the log always, no matter where they are created
// TODO: report with rounding and option to carry rests to new file (as +segments) (maybe?)
