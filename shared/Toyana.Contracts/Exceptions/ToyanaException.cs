using System;

namespace Toyana.Contracts.Exceptions;

public abstract class ToyanaException : Exception
{
    public string ErrorCode { get; }
    public object[] Args { get; }

    protected ToyanaException(string errorCode, string message, params object[] args) 
        : base(message)
    {
        ErrorCode = errorCode;
        Args = args;
    }

    protected ToyanaException(string errorCode, string message, Exception innerException, params object[] args) 
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Args = args;
    }
}

// Common Exceptions
public class DomainException : ToyanaException
{
    public DomainException(string errorCode, string message, params object[] args) 
        : base(errorCode, message, args) { }
}

public class ValidationException : ToyanaException
{
    public ValidationException(string errorCode, string message, params object[] args) 
        : base(errorCode, message, args) { }
}

public class NotFoundException : ToyanaException
{
    public NotFoundException(string resourceName, object key) 
        : base("RESOURCE_NOT_FOUND", $"{resourceName} with key '{key}' was not found.") { }
}
