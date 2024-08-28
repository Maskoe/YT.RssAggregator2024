using Microsoft.EntityFrameworkCore;

namespace RssAggregator.Db;

public class Context : DbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Post> Posts { get; set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUser>()
            .HasMany(user => user.Feeds)
            .WithMany(feed => feed.Users)
            .UsingEntity<Subscription>();

        modelBuilder.Entity<Subscription>()
            .HasIndex(x => new { x.FeedId, x.UserId })
            .IsUnique();
        
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}

public class AppUser
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }

    public List<Subscription> Subscriptions { get; set; } = new();
    public List<Feed> Feeds { get; set; } = new();
}

public class Feed
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public DateTime? LastFetchedAt { get; set; }

    public List<Post> Posts { get; set; } = new();

    public List<Subscription> Subscriptions { get; set; } = new();
    public List<AppUser> Users { get; set; } = new();
}

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime PublishedAt { get; set; }
    public string Url { get; set; }

    public Guid FeedId { get; set; }
    public Feed Feed { get; set; }
}

public class Subscription
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public AppUser User { get; set; }

    public Guid FeedId { get; set; }
    public Feed Feed { get; set; }

    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}