using Time2.Exceptions;
using Time2.Models;
using Time2.Services;

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
            var entries = LogEntryParser.ParseLine(input);
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

// TODO: pre-processors like 0800, 8, . and so on
// TODO: +segments (extra-segments) keep these at the top of the log always, no matter where they are created
// TODO: report with rounding and option to carry rests to new file (as +segments)
