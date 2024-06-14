using Kolokwium2.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kolokwium2.Controllers;

[ApiController]
public class GameController : ControllerBase
{
    private IDbService Service { get; set; }

    public GameController(IDbService service)
    {
        Service = service;
    }
    
    [HttpGet]
    [Route("api/characters/{characterId:int}")]
    public async Task<IActionResult> GetCharacters(int characterId)
    {
        if (!await Service.DoesCharacterExist(characterId))
        {
            return NotFound($"Character with id: {characterId} doesn't exist");
        }

        var result = await Service.GetCharacters(characterId);

        return Ok(result);
    }
    
    [HttpPost]
    [Route("api/characters/{characterId:int}/backpacks")]
    public async Task<IActionResult> AddItems(int characterId, List<int> itemsId)
    {
        if (!await Service.DoesCharacterExist(characterId))
        {
            return NotFound($"Character with id: {characterId} doesn't exist");
        }
        
        foreach (var idItem in itemsId)
        {
            if (!await Service.DoesItemExist(idItem))
            {
                return NotFound($"Item with id: {idItem} doesn't exist");
            }
        }
        
        if (!await Service.DoesCharacterHasAvailableWeight(characterId, itemsId))
        {
            return NotFound($"Character with id: {characterId} doesn't have enough space (maxWeight is too small to add all those items)");
        }
        
        await Service.AddItemsToBackpack(characterId, itemsId);
        
        var result = await Service.GetCharacterItems(characterId);

        return Ok(result);
    }
}