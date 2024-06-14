namespace Kolokwium2.Services;

public interface IDbService
{
    Task<bool> DoesCharacterExist(int characterId);
    Task<object?> GetCharacters(int characterId);
    Task<bool> DoesItemExist(int idItem);
    Task<bool> DoesCharacterHasAvailableWeight(int characterId, List<int> itemsId);
    Task AddItemsToBackpack(int characterId, List<int> itemsId);
    Task<object> GetCharacterItems(int characterId);
}