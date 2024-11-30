namespace Time2.Exceptions;
internal class InvalidLogOperationException(string message)
    : InvalidOperationException(message);
