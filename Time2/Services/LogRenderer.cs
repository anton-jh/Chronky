using Time2.Models;

namespace Time2.Services;
internal static class LogRenderer
{
    public static void Render(Log log)
    {
        var lines = log.Entries
            .Select((x, i) => $"{(log.CursorPosition == i ? "> " : "  ")}{x}")
            .ToList();

        lines.Add("");

        var longestLine = lines
            .Select(x => x.Length)
            .Max();

        var validationResult = new LogValidator().Validate(log.Entries);

        if (validationResult.Error is LogValidator.LogValidatorError error)
        {
            var line = lines[error.LineNumber];
            lines[error.LineNumber] = $"{line}{new string(' ', longestLine - line.Length)} !!{error.Message}!!";
        }

        var rendered = string.Join('\n', lines);
        Console.Clear();
        Console.Write(rendered);
    }
}
