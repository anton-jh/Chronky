﻿using Time2.Models;

namespace Time2.Services;
internal static class LogRenderer
{
    public static void Render(Log log, bool insertMode, LogValidator.LogValidatorResult validationResult, int bufferWidth)
    {
        var entries = log.Entries.ToList();

        var lines = new List<string>();

        for (int i = -1; i <= entries.Count - 1; i++)
        {
            var line = "";
            
            line += !insertMode && log.CursorPosition == i
                ? "> "
                : "  ";

            if (i >= 0 && i < entries.Count)
            {
                line += entries[i].Display();

                if (validationResult.Error is LogValidator.LogValidatorError error && error.LineNumber == i)
                {
                    line += $"    !!{error.Message}!!";
                }
            }

            lines.Add(line);

            if (insertMode && log.CursorPosition == i)
            {
                lines.Add("> ");
            }
        }

        var now = DateTime.Now;
        var logResult = LogCalculator.Sum(log, now);
        var longestAccountLabel = logResult.Accounts.Keys.MaxBy(x => x.Length)?.Length ?? 0;
        var accountLines = logResult.Accounts
            .Select(kv => $"{new string(' ', longestAccountLabel - kv.Key.Length)}{kv.Key}: {kv.Value}")
            .ToList();

        lines.Add("");
        if (logResult.IsOpen)
        {
            lines.Add($"Assuming last period closed now ({LogCalculator.GetNow(now)}):");
        }
        else
        {
            lines.Add($"");
        }

        //lines.Add("");
        lines.Add($"End of day: {logResult.ProjectedEndOfDay?.ToString() ?? "N/A"}");

        //lines.Add("");
        lines.Add($"Total: {logResult.Total}");

        lines.Add("");
        lines.AddRange(accountLines.Where(x => !x.TrimStart().StartsWith('_')));

        lines.Add("");
        lines.AddRange(accountLines.Where(x => x.TrimStart().StartsWith('_')));

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
//> line2
//  line3
//  line4
