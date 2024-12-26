using System.Text.RegularExpressions;
using Time2.Exceptions;

namespace Time2.Services;

internal abstract class PreProcessor
{
    public abstract string Process(string input);
}

internal class ShortFormTimePreProcessor
    : PreProcessor
{
    public override string Process(string input)
    {
        if (!input.All(char.IsDigit))
        {
            return input;
        }

        return input switch
        {
            [var a] => $"0{a}:00",
            [var a, var b] => $"{a}{b}:00",
            [var a, var b, var c] => $"0{a}:{b}{c}",
            [var a, var b, var c, var d] => $"{a}{b}:{c}{d}",
            _ => input
        };
    }
}

internal class NowPreProcessor
    : PreProcessor
{
    public override string Process(string input)
    {
        if (input == ".")
        {
            return DateTime.Now.ToString("HH:mm");
        }
        if (input == "..")
        {
            var now = DateTime.Now;
            var minutes = (int)Math.Round(now.Minute / 15.0) * 15;
            if (minutes == 60)
            {
                now = now.AddHours(1);
                minutes = 0;
            }
            return $"{now.Hour:D2}:{minutes:D2}";
        }
        return input;
    }
}

internal partial class SegmentPreProcessor
    : PreProcessor
{
    public override string Process(string input)
    {
        var match = ShortFormSegmentRegex().Match(input);
        if (match.Success == false)
        {
            return input;
        }

        var hours = 0;
        var minutes = 0;

        if (string.IsNullOrWhiteSpace(match.Groups["h"].Value) == false)
        {
            if (int.TryParse(match.Groups["h"].Value, out hours) == false)
            {
                throw new LogParsingException($"Invalid hour value '{match.Groups["h"].Value}' in entry '{input}'");
            }
        }
        if (string.IsNullOrWhiteSpace(match.Groups["m"].Value) == false)
        {
            if (int.TryParse(match.Groups["m"].Value, out minutes) == false)
            {
                throw new LogParsingException($"Invalid minute value '{match.Groups["m"].Value}' in entry '{input}'");
            }
        }

        return $"{match.Groups["prefix"].Value}{hours:D2}:{minutes:D2}=>{match.Groups["label"].Value}";
    }

    [GeneratedRegex(@"^(?<prefix>[-+])((?<h>\d+)h)?((?<m>\d+)m)?=>(?<label>.+)$")]
    private static partial Regex ShortFormSegmentRegex();
}
