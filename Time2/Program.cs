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

bool editMode = false;

while (true)
{
    Console.Clear();
    foreach (var line in log.Display(showCursor: editMode))
    {
        Console.WriteLine(line);
    }

    if (!editMode)
    {
        Console.Write("> ");
        var input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            log.ResetCursor();
            editMode = true;
        }
        else
        {
            try
            {
                var entries = ParseLine(input);
                foreach (var entry in entries)
                {
                    log.Append(entry);
                }
                LogFileManager.Save(log);
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
                log.RemoveSelectedEntry();
                break;
            case ConsoleKey.Escape:
                editMode = false;
                break;
            case ConsoleKey.Enter:
                // todo accept input and insert above
                break;
            case ConsoleKey.Spacebar:
                // todo accept input and replace selected
                break;
        }
    }
}


IEnumerable<LogEntry> ParseLine(string line)
{
    var words = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var entries = words
        .Select(PreProcess)
        .Select(LogEntryParser.ParseEntry);

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
// TODO: report with rounding and option to carry rests to new file (as +segments)
