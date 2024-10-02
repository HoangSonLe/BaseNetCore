using Infrastructure.Entitites;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class KafkaMessageEntityConfigurations : IEntityTypeConfiguration<KafkaMessage>
{
    public void Configure(EntityTypeBuilder<KafkaMessage> builder)
    {
        // Primary key
        builder.HasKey(p => p.MessageId);

        // Properties
        builder.Property(p => p.Status).HasColumnName("Status");
        builder.Property(p => p.Offset).HasColumnName("Offset");
        builder.Property(p => p.Partition).HasColumnName("Partition");
        builder.Property(p => p.Timestamp).HasColumnName("Timestamp");
        builder.Property(p => p.MessageData).HasColumnName("MessageData");

        // Table
        builder.ToTable("KafkaMessage");
    }
}