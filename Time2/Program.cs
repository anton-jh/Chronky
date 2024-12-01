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

while (true)
{
    Console.Clear();
    foreach (var line in log.Display())
    {
        Console.WriteLine(line);
    }
    Console.Write("\n> ");
    var input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input))
    {
        // TODO: edit mode
        log.RemoveLast();
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

// TODO: pre-processors like 0800, 8, . and so on
// TODO: +segments (extra-segments) keep these at the top of the log always, no matter where they are created
// TODO: report with rounding and option to carry rests to new file (as +segments)
