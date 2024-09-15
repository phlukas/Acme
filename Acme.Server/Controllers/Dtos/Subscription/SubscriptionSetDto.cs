namespace Acme.Server.Controllers.Dtos.Subscription;

public class SubscriptionSetDto
{
    public required string Email { get; set; }
    public DateTime ExpirationDate { get; set; }
}
