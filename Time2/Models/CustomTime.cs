namespace Time2.Models;
internal partial record CustomTime(int Hours, int Minutes)
{
    public int TotalMinutes => Hours * 60 + Minutes;

    public override string ToString()
    {
        int absHours = Math.Abs(Hours);
        int absMinutes = Math.Abs(Minutes);
        string sign = TotalMinutes < 0 ? "-" : "";
        return $"{sign}{absHours:D2}:{absMinutes:D2}";
    }

    public static CustomTime? TryParse(string input)
    {
        var parts = input.Split(':');
        if (parts.Length != 2)
        {
            return null;
        }
        if (!int.TryParse(parts[0], out var hours))
        {
            return null;
        }
        if (!int.TryParse(parts[1], out var minutes) || minutes < 0 || minutes >= 60)
        {
            return null;
        }
        var sign = Math.Sign(hours) == -1 ? -1 : 1;
        return new CustomTime(hours, minutes * sign);
    }

    public static bool operator <(CustomTime left, CustomTime right) =>
        left.TotalMinutes < right.TotalMinutes;

    public static bool operator >(CustomTime left, CustomTime right) =>
        left.TotalMinutes > right.TotalMinutes;

    public static bool operator <=(CustomTime left, CustomTime right) =>
        left.TotalMinutes <= right.TotalMinutes;

    public static bool operator >=(CustomTime left, CustomTime right) =>
        left.TotalMinutes >= right.TotalMinutes;

    public static CustomTime operator +(CustomTime left, CustomTime right)
    {
        int totalMinutes = left.TotalMinutes + right.TotalMinutes;
        return FromTotalMinutes(totalMinutes);
    }

    public static CustomTime operator -(CustomTime left, CustomTime right)
    {
        int totalMinutes = left.TotalMinutes - right.TotalMinutes;
        return FromTotalMinutes(totalMinutes);
    }

    private static CustomTime FromTotalMinutes(int totalMinutes)
    {
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        // Adjust for negative minutes rolling into hours
        if (minutes < 0)
        {
            minutes += 60;
            hours -= 1;
        }

        return new CustomTime(hours, minutes);
    }
}
