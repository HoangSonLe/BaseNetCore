using Core.Enums;
using Infrastructure.Entitites;
using Infrastructure.EntityTypeConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DBContexts
{
    public class SampleReadOnlyDBContext : SampleDBContext
    {
        public SampleReadOnlyDBContext() : base()
        {
        }
        public SampleReadOnlyDBContext(DbContextOptions<SampleDBContext> options)
           : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public override int SaveChanges()
        {
            throw new NotSupportedException();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
    public class SampleDBContext : DbContext
    {
        public SampleDBContext(DbContextOptions<SampleDBContext> options)
           : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public SampleDBContext() : base()
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<TelegramChat> TelegramChats { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.EnableSensitiveDataLogging();
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new UserEntityConfigurations());
            modelBuilder.ApplyConfiguration(new RoleEntityConfigurations());
            modelBuilder.ApplyConfiguration(new TelegramChatEntityConfigurations());

            //Default value for column entity models

            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = 1,
                    Name = "Admin",
                    Description = "Admin",
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    State = (int)EState.Active,
                    EnumActionList = new List<int> { 1, 2 },
                    NameNonUnicode = "Admin",
                },
                new Role()
                {
                    Id = 2,
                    Name = "Dev",
                    Description = "Dev",
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    State = (int)EState.Active,
                    EnumActionList = new List<int> { 1, 2 },
                    NameNonUnicode = "Dev"
                }
            );
            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    UserName = "Admin",
                    NameNonUnicode = "Admin",
                    Password = "/cA7ZZQqtyOGVwe1kEbPSg==", //123456
                    Name = "Admin",
                    Phone = "",
                    TypeAccount = 1,
                    RoleIdList = new List<int>() { 1, 2, 3, 4 },
                    CreatedBy = 1,
                    CreatedDate = DateTime.Now,
                    State = (int)EState.Active,
                    Email = ""
                }
            );



            base.OnModelCreating(modelBuilder);
        }

    }
}
