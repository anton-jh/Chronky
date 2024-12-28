namespace Chronky.Exceptions;
internal class InvalidLogOperationException(string message)
    : InvalidOperationException(message);
