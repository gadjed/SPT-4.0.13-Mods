using Microsoft.AspNetCore.Mvc;

namespace ModInventory;

[ApiController]
[Route("modinventory/api/file")]
public class FileController(InventoryService inventoryService) : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromQuery] string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return BadRequest("Missing path.");
        }

        var (ok, absolute, error) = inventoryService.ResolveAllowedFile(path);
        if (!ok || absolute is null)
        {
            return BadRequest(error ?? "Invalid path.");
        }

        var contentType = absolute.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? "application/json"
            : "application/octet-stream";

        return PhysicalFile(absolute, contentType, enableRangeProcessing: true);
    }
}
