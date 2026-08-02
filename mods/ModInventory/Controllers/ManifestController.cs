using Microsoft.AspNetCore.Mvc;

namespace ModInventory;

[ApiController]
[Route("modinventory/api/manifest")]
public class ManifestController(InventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        try
        {
            return Ok(inventoryService.BuildManifest());
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}
