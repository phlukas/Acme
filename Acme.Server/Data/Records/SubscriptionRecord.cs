using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Server.Data.Records;

public class SubscriptionRecord
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public DateTime ExpirationDate { get; set; }

    internal static void OnEntityCreating(EntityTypeBuilder<SubscriptionRecord> builder)
    {
        builder.Property(x => x.Email).HasMaxLength(320);
    }
}
