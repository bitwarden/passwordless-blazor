namespace Passwordless.Blazor;

public class PasswordlessClientException : Exception
{
    public PasswordlessProblemDetails Details { get; }

    public PasswordlessClientException(PasswordlessProblemDetails details)
        : base(details.Title)
    {
        Details = details;
    }
}

public class PasswordlessProblemDetails
{
    public string From { get; set; }
    public string ErrorCode { get; set; }
    public string Title { get; set; }
    public int? Status { get; set; }
    public string? Detail { get; set; }
}
