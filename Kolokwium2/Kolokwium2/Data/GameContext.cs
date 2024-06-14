using KolosGrupa1.Models;
using Microsoft.EntityFrameworkCore;

namespace Kolokwium2.Context;

public class GameContext : DbContext
{
    protected GameContext()
    {
    }

    public GameContext(DbContextOptions options) : base(options)
    {
    }
    
    public virtual DbSet<Backpack> Backpacks { get; set; }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<CharacterTitle> CharacterTitles { get; set; }

    public virtual DbSet<Item> Items { get; set; }
    
    public virtual DbSet<Title> Titles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Title>().HasData(
            new Title(){Id = 1, Name = "Władca podziemi"},
            new Title(){Id = 2, Name = "Miszczu toplejna"}
        );
        
        modelBuilder.Entity<Character>().HasData(
            new Character(){Id = 1, FirstName = "Palmix", LastName = "Palmowski", CurrentWeight = 160, MaxWeight = 200},
            new Character(){Id = 2, FirstName = "Natalia", LastName = "Kocur", CurrentWeight = 800, MaxWeight = 1000},
            new Character(){Id = 3, FirstName = "Ola", LastName = "Nowak", CurrentWeight = 40, MaxWeight = 10000}
        );
        
        modelBuilder.Entity<Item>().HasData(
            new Item(){Id = 1, Name = "No noz a co innego", Weight = 40},
            new Item(){Id = 2, Name = "Zbroja", Weight = 80},
            new Item(){Id = 3, Name = "Naszyjnik", Weight = 20}
        );

        modelBuilder.Entity<Backpack>().HasData(
            new Backpack(){CharacterId = 1, ItemId = 1, Amount = 3},
            new Backpack(){CharacterId = 1, ItemId = 3, Amount = 2},
            new Backpack(){CharacterId = 2, ItemId = 2, Amount = 10},
            new Backpack(){CharacterId = 3, ItemId = 1, Amount = 1}
        );
        
        modelBuilder.Entity<CharacterTitle>().HasData(
            new CharacterTitle(){CharacterId = 1, TitleId = 1, AcquiredAt = new DateTime(2018, 9, 12)},
            new CharacterTitle(){CharacterId = 3, TitleId = 2, AcquiredAt = new DateTime(2019, 7, 13)}
        );
        
        base.OnModelCreating(modelBuilder);
    }
    
}