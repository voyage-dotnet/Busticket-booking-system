using System;

namespace BusTicketSystem.Web.Exceptions;

public class BaseException : Exception
{
    public int StatusCode { get; }
    public List<string> Errors { get; }
    public BaseException(string message, int StatusCodes = 500, List<string> errors = null) : base(message)
    {
        StatusCode = StatusCodes;
        Errors = errors ?? new List<string>();
    }
}

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message, 404)
    {

    }
}
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message) : base(message, 401)
    {

    }
}

public class BadRequestException : BaseException
{
    public BadRequestException(string message, List<string> errors = null) : base(message, 400, errors)
    {

    }
}
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message) : base(message, 403)
    {

    }
}
public class PaymentFailedException : Exception
{
    public PaymentFailedException(string message) : base(message) { }
}
