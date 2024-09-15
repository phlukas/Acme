namespace Acme.Server.Controllers.Dtos.Subscription;

public class SubscriptionDto
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public DateTime ExpirationDate { get; set; }
}
