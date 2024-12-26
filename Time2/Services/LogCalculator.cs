using Time2.Models;

namespace Time2.Services;

internal static class LogCalculator
{
    public static LogCalculatorResult Sum(Log log, DateTime now)
    {
        var accounts = new Dictionary<string, CustomTime>();

        CustomTime? lastTime = null;
        string? lastLabel = null;

        foreach (var entry in log.Entries)
        {
            if (entry is TimeLogEntry timeEntry)
            {
                if (lastTime is not null && lastLabel is not null && lastTime < timeEntry.Time)
                {
                    accounts[lastLabel] =
                        accounts.GetValueOrDefault(lastLabel, new CustomTime(0, 0))
                        + timeEntry.Time - lastTime;
                }

                lastTime = timeEntry.Time;
                lastLabel = null;
            }
            else if (entry is LabelLogEntry labelEntry)
            {
                if (lastLabel is null)
                {
                    lastLabel = labelEntry.Text;
                }
                else
                {
                    lastLabel = null;
                }
            }
            else if (entry is SubSegmentLogEntry subSegmentEntry)
            {
                if (lastLabel is not null)
                {
                    accounts[subSegmentEntry.Text] =
                        accounts.GetValueOrDefault(subSegmentEntry.Text, new CustomTime(0, 0))
                        + subSegmentEntry.TimeSpan;

                    accounts[lastLabel] =
                        accounts.GetValueOrDefault(lastLabel, new CustomTime(0, 0))
                        - subSegmentEntry.TimeSpan;
                }
            }
        }

        var isOpen = false;
        CustomTime? endTime = null;

        if (lastLabel is not null && lastTime is not null)
        {
            isOpen = true;

            accounts[lastLabel] =
                accounts.GetValueOrDefault(lastLabel, new CustomTime(0, 0))
                + GetNow(now) - lastTime;
        }

        var total = accounts
            .Where(kv => !kv.Key.StartsWith('_'))
            .Select(kv => kv.Value)
            .Aggregate(new CustomTime(0, 0), (acc, time) => acc + time);

        if (lastTime is not null && total <= new CustomTime(8, 0))
        {
            endTime = lastTime + new CustomTime(8, 0) - total;
        }


        return new LogCalculatorResult(
            accounts,
            isOpen,
            endTime,
            total);
    }

    public static CustomTime GetNow(DateTime now)
    {
        return new CustomTime(
            now.Hour,
            now.Minute);
    }


    public record LogCalculatorResult(
        Dictionary<string, CustomTime> Accounts,
        bool IsOpen,
        CustomTime? ProjectedEndOfDay,
        CustomTime Total);
}
