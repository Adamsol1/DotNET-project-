using Microsoft.EntityFrameworkCore;
using DOTNET_PROJECT.Domain.Models;

namespace DOTNET_PROJECT.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Character> Characters { get; set; } = null!;
    public DbSet<StoryNode> StoryNodes { get; set; } = null!;
    public DbSet<Dialogue> Dialogues { get; set; } = null!;
    public DbSet<Choice> Choices { get; set; } = null!;
    public DbSet<PlayerCharacter> PlayerCharacters { get; set; } = null!;
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure relationships and constraints if needed
        modelBuilder.Entity<Choice>()
            .HasOne(c => c.StoryNode)
            .WithMany(sn => sn.Choices)
            .HasForeignKey(c => c.StoryNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Choice>()
            .HasOne(c => c.NextStoryNode)
            .WithMany()
            .HasForeignKey(c => c.NextStoryNodeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dialogue>()
            .HasOne(d => d.Character)
            .WithMany(c => c.Dialogues)
            .HasForeignKey(d => d.CharacterId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Dialogue>()
            .HasOne(d => d.StoryNode)
            .WithMany(sn => sn.Dialogues)
            .HasForeignKey(d => d.StoryNodeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PlayerCharacter>()
            .HasBaseType<Character>();
    }
}