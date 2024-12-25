using Time2.Models;

namespace Time2.Services;
internal static class LogRenderer
{
    public static void Render(Log log, bool insertMode, LogValidator.LogValidatorResult validationResult)
    {
        var lines = log.Entries.Select((entry, i) =>
        {
            var line = "";

            line += !insertMode && log.CursorPosition == i
                ? "> "
                : "  ";

            line += entry.Display();

            if (validationResult.Error is LogValidator.LogValidatorError error && error.LineNumber == i)
            {
                line += $"    !!{error.Message}!!";
            }

            return line;
        }).ToList();

        if (insertMode)
        {
            lines.Insert(log.CursorPosition, "> ");
        }

        var rendered = string.Join('\n', lines);
        Console.Clear();
        Console.Write(rendered);
    }
}


// Insert mode:
//
//  line1
//  line2
//> [Console.ReadLine()]
//  line3
//
//
// Cursor mode:
//
//  line1
//  line2
//> line3
//  line4
