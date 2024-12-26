using System.Globalization;
using Time2.Models;

namespace Time2.Services;

internal class LogFileManager
{
    private static readonly string _logFolderPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "anton-jh", "timey", "logs");

    private static readonly string _tempFilePath = Path.Combine(_logFolderPath, "temp.txt");

    private static List<LogEntry>? _previousLog;


    public static Log? Load()
    {
        var log = LoadTemp()
            ?? LoadLatest();

        _previousLog = log?.Entries.ToList();

        return log;
    }

    public static void Save(Log log)
    {
        if (_previousLog is not null && log.Entries.SequenceEqual(_previousLog))
        {
            return;
        }

        if (!Directory.Exists(_logFolderPath))
        {
            Directory.CreateDirectory(_logFolderPath);
        }

        var filename = CreateFilename(log.Created);
        var path = Path.Combine(_logFolderPath, filename);

        var lines = log.Entries
            .Select(x => x.Serialize());

        File.WriteAllLines(path, lines);

        if (File.Exists(_tempFilePath))
        {
            File.Delete(_tempFilePath);
        }
    }


    private static Log? LoadLatest()
    {
        if (!Directory.Exists(_logFolderPath))
        {
            return null;
        }

        var logInfo = Directory.EnumerateFiles(_logFolderPath)
            .Select(Path.GetFileName)
            .OfType<string>()
            .Select(ParseLogFilename)
            .OfType<LogInfo>()
            .MaxBy(x => x.Created);

        if (logInfo is null)
        {
            return null;
        }

        var lines = File.ReadAllLines(Path.Combine(_logFolderPath, logInfo.Filename));

        return new Log(logInfo.Created, lines.Select(LogEntryParser.ParseEntry));
    }

    private static Log? LoadTemp()
    {
        if (!File.Exists(_tempFilePath))
        {
            return null;
        }

        var lines = File.ReadAllLines(_tempFilePath);

        return new Log(DateTime.Now, lines.Select(LogEntryParser.ParseEntry));
    }

    private static LogInfo? ParseLogFilename(string filename)
    {
        return DateTime.TryParseExact(
            filename[..filename.IndexOf('.')],
            "yyyy-MM-dd_HH-mm-ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var date)
            ? new(filename, date)
            : null;
    }

    private static string CreateFilename(DateTime created)
    {
        return $"{created:yyyy-MM-dd_HH-mm-ss}.txt";
    }

    private record LogInfo(string Filename, DateTime Created);
}
