

using Infrastructure.Entitites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityTypeConfiguration
{
    public class RoleEntityConfigurations : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            // Primary key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Name).HasColumnName("Name");
            builder.Property(p => p.Description).HasColumnName("Description");
            builder.Property(p => p.EnumActionList).HasColumnName("EnumActionList");
            builder.Property(p => p.Level).HasColumnName("Level");


            builder.Property(p => p.State).HasColumnName("State");
            builder.Property(p => p.CreatedDate).HasColumnName("CreatedDate");
            builder.Property(p => p.CreatedBy).HasColumnName("CreatedBy");
            builder.Property(p => p.UpdatedDate).HasColumnName("UpdatedDate");
            builder.Property(p => p.UpdatedBy).HasColumnName("UpdatedBy");

            // Table
            builder.ToTable("Role");
        }
    }
}
