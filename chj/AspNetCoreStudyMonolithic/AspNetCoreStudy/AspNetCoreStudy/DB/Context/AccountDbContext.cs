namespace AspNetCoreStudy.DB.Context;

using AspNetCoreStudy.DB.Entity;
using Microsoft.EntityFrameworkCore;

public class AccountDbContext : DbContext
{
    public DbSet<AccountEntity> Account { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }
}
