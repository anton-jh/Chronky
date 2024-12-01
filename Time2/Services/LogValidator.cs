using Time2.Models;

namespace Time2.Services;

internal class LogValidator
{
    private int _lineNumber = 0;
    private TimeLogEntry? _lastTime = null;
    private LabelLogEntry? _currentLabel = null;
    private CustomTime _sumOfContainedSubSegments = new(0, 0);


    public LogValidatorResult Validate(IEnumerable<LogEntry> entries)
    {
        _lineNumber = 0;
        _lastTime = null;
        _currentLabel = null;
        _sumOfContainedSubSegments = new(0, 0);

        foreach (var entry in entries)
        {
            var error = entry switch
            {
                TimeLogEntry timeEntry => ValidateTime(timeEntry),
                LabelLogEntry labelEntry => ValidateLabel(labelEntry),
                SubSegmentLogEntry subSegmentLogEntry => ValidateSubSegment(subSegmentLogEntry),
                _ => throw new ArgumentException($"Invalid subclass '{entry.GetType().Name}'")
            };

            if (error is not null)
            {
                return new LogValidatorResult(Error: error);
            }

            _lineNumber++;
        }

        return new LogValidatorResult(Error: null);
    }


    private LogValidatorError? ValidateTime(TimeLogEntry entry)
    {
        if (_lastTime is not null && _lastTime.Time >= entry.Time)
        {
            return Error($"New time ({entry.Time:HH:mm}) must be after previous time ({_lastTime.Time:HH:mm})");
        }

        if (_lastTime is not null)
        {
            var totalSegmentLength = entry.Time - _lastTime.Time;
            if (_sumOfContainedSubSegments >= totalSegmentLength)
            {
                return Error($"Resulting segment ({_lastTime.Time} to {entry.Time} = {entry.Time - _lastTime.Time}) must be > sum of sub-segments ({_sumOfContainedSubSegments:HH:mm})");
            }
        }

        _lastTime = entry;
        _currentLabel = null;
        _sumOfContainedSubSegments = new(0, 0);

        return null;
    }

    private LogValidatorError? ValidateLabel(LabelLogEntry entry)
    {
        if (_currentLabel is not null)
        {
            return Error($"Segment already has a label: '{_currentLabel.Text}'");
        }

        if (_lastTime is null)
        {
            return Error($"Cannot add label '{entry.Text}' to log without start time");
        }

        _currentLabel = entry;

        return null;
    }

    private LogValidatorError? ValidateSubSegment(SubSegmentLogEntry entry)
    {
        if (_currentLabel is null)
        {
            return Error($"Cannot add sub segment '{entry.Serialize()}' to unlabeled segment");
        }

        _sumOfContainedSubSegments += entry.TimeSpan;

        return null;
    }


    private LogValidatorError Error(string message)
    {
        return new LogValidatorError(_lineNumber, message);
    }


    public record LogValidatorResult(LogValidatorError? Error);
    public record LogValidatorError(int LineNumber, string Message);
}
