using Microsoft.EntityFrameworkCore;

namespace NixBot.Entities;

public class NixbotContext : DbContext
{
    public DbSet<DbMessage> Messages { get; set; } = null!;
    public DbSet<DbBlacklistEntry> Blacklist { get; set; } = null!;

    public string DbPath { get; }

    public NixbotContext()
    {
        DbPath = Environment.GetEnvironmentVariable("NIXBOT_DB_PATH")!;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.UseSqlite($"Data Source={DbPath}");
    }
}