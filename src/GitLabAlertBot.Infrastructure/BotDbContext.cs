using GitLabAlertBot.Domain;
using GitLabAlertBot.Domain.Invites;
using Microsoft.EntityFrameworkCore;

namespace GitLabAlertBot.Infrastructure;

public class BotDbContext : DbContext
{
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Invite> Invites { get; set; }

    public BotDbContext(DbContextOptions options) : base(options)
    {
        Chats = Set<Chat>();
        Invites = Set<Invite>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>().HasKey(x => x.ChatId);

        modelBuilder.Entity<Invite>().HasKey(x => x.Id);
    }
}
