using Infrastructure.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfiguration
{
    public class TelegramChatEntityConfigurations : IEntityTypeConfiguration<TelegramChat>
    {
        public void Configure(EntityTypeBuilder<TelegramChat> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.UserId).HasColumnName("UserId");
            builder.Property(p => p.TelegramChatId).HasColumnName("TelegramChatId");
            builder.Property(p => p.PhoneNumber).HasColumnName("PhoneNumber");
            builder.Property(p => p.LastSendAnniversaryNotiDateTime).HasColumnName("LastSendAnniversaryNotiDateTime");
            builder.Property(p => p.LastSendExpiredNotiDateTime).HasColumnName("LastSendExpiredNotiDateTime");

            builder.Property(p => p.State).HasColumnName("State");
            builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate");
            builder.Property(p => p.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
            builder.Property(p => p.UpdatedBy).HasColumnName("UpdatedBy");

            // Table
            builder.ToTable("TelegramChat");
        }
    }
}
