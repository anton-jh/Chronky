using Chronky.Models;

namespace Chronky.Services;

internal static class LogCalculator
{
    private static readonly CustomTime _fullDay = new CustomTime(8, 0);


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
                        accounts.GetValueOrDefault(lastLabel, CustomTime.Zero)
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
                        accounts.GetValueOrDefault(subSegmentEntry.Text, CustomTime.Zero)
                        + subSegmentEntry.TimeSpan;

                    accounts[lastLabel] =
                        accounts.GetValueOrDefault(lastLabel, CustomTime.Zero)
                        - subSegmentEntry.TimeSpan;
                }
            }
            else if (entry is ExtraSegmentLogEntry extraSegmentEntry)
            {
                accounts[extraSegmentEntry.Text] =
                        accounts.GetValueOrDefault(extraSegmentEntry.Text, CustomTime.Zero)
                        + extraSegmentEntry.TimeSpan;
            }
        }

        var isOpenAndInThePast = false;
        CustomTime? endTime = null;
        CustomTime nowCustomTime = CustomTime.FromDateTime(now);

        if (lastLabel is not null && lastTime is not null && nowCustomTime > lastTime)
        {
            isOpenAndInThePast = true;

            accounts[lastLabel] =
                accounts.GetValueOrDefault(lastLabel, CustomTime.Zero)
                + nowCustomTime - lastTime;

            lastTime = nowCustomTime;
        }

        var total = accounts
            .Where(kv => !kv.Key.StartsWith('_'))
            .Select(kv => kv.Value)
            .Aggregate(CustomTime.Zero, (acc, time) => acc + time);

        if (lastTime is not null && total <= _fullDay)
        {
            endTime = lastTime + _fullDay - total;
        }


        return new LogCalculatorResult(
            accounts,
            isOpenAndInThePast,
            endTime,
            total);
    }


    public record LogCalculatorResult(
        Dictionary<string, CustomTime> Accounts,
        bool IsOpenAndInThePast,
        CustomTime? ProjectedEndOfDay,
        CustomTime Total);
}
