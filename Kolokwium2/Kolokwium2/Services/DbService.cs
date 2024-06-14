using System.Transactions;
using Kolokwium2.Context;
using KolosGrupa1.Models;
using Microsoft.EntityFrameworkCore;

namespace Kolokwium2.Services;

public class DbService : IDbService
{
    private GameContext Context;

    public DbService(GameContext context)
    {
        Context = context;
    }

    public async Task<bool> DoesCharacterExist(int characterId)
    {
        return await Context.Characters.FindAsync(characterId) != null;
    }

    public async Task<object?> GetCharacters(int characterId)
    {
        var characters = await Context.Characters
            .Include(e => e.Backpacks)
            .ThenInclude(e => e.ItemNavigation)
            .Include(e => e.CharacterTitles)
            .ThenInclude(e => e.TitleNavigation)
            .FirstOrDefaultAsync(e => e.Id == characterId);

        if (characters == null)
        {
            throw new Exception("Bad characterId");
        }

        var result = new
        {
            characters.FirstName,
            characters.LastName,
            characters.CurrentWeight,
            characters.MaxWeight,
            BackpackItems = characters.Backpacks.Select(e => new
            {
                ItemName = e.ItemNavigation.Name,
                ItemWeight = e.ItemNavigation.Weight,
                e.Amount
            }),
            Titles = characters.CharacterTitles.Select(e => new
            {
                Title = e.TitleNavigation.Name,
                AquiredAt =  e.AcquiredAt
            })
        };

        return result;
    }

    public async Task<bool> DoesItemExist(int idItem)
    {
        return await Context.Items.FindAsync(idItem) != null;
    }

    public async Task<bool> DoesCharacterHasAvailableWeight(int characterId, List<int> itemsId)
    {
        int ammount = 0;
        foreach (var itemId in itemsId)
        {
            Item? item = await Context.Items.FindAsync(itemId);
            if (item == null)
            {
                throw new Exception("Bad itemId");
            }
            ammount += item.Weight;
        }
        
        Character? character = await Context.Characters.FindAsync(characterId);
        
        if (character == null)
        {
            throw new Exception("Bad characterId");
        }
        
        return (character.CurrentWeight += ammount) <= character.MaxWeight;
    }

    public async Task AddItemsToBackpack(int characterId, List<int> itemsId)
    {
        using (TransactionScope scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            foreach (var itemId in itemsId)
            {
                Backpack? backpack = await Context.Backpacks
                    .FirstOrDefaultAsync(
                        e => e.ItemId == itemId && e.CharacterId == characterId
                        );
                
                if (backpack == null)
                {
                    Context.Backpacks.Add(new Backpack() { ItemId = itemId, CharacterId = characterId, Amount = 1 });
                }
                else
                {
                    backpack.Amount += 1;
                }
                
                Item? item = await Context.Items.FindAsync(itemId);
                if (item == null)
                {
                    throw new Exception("Bad itemId");
                }
            }

            await Context.SaveChangesAsync();

            Character? character = await Context.Characters.FindAsync(characterId);
            if (character == null)
            {
                throw new Exception("Bad characterId");
            }

            var bags = await Context.Backpacks
                .Include(e => e.ItemNavigation)
                .Where(e => e.CharacterId == characterId).ToListAsync();

            int ammount = 0;
            foreach (var bag in bags)
            {
                ammount += bag.Amount * bag.ItemNavigation.Weight;
            }

            character.CurrentWeight = ammount;
            
            await Context.SaveChangesAsync();
            scope.Complete();
        }
    }

    public async Task<object> GetCharacterItems(int characterId)
    {
        var result = await Context.Backpacks
            .Where(e => e.CharacterId == characterId)
            .Select(e => new
                    {
                        e.Amount,
                        e.ItemId,
                        characterId
                    }).ToListAsync();

        if (result == null || result.Count == 0)
        {
            throw new Exception("Bad characterId");
        }
        
        return result.Cast<object>().ToList();
    }
}