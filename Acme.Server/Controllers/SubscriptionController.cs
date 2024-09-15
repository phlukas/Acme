using Acme.Server.Controllers.Dtos.Subscription;
using Acme.Server.Extensions;
using Acme.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Acme.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionController : ControllerBase
{
    private readonly SubscriptionService _subscriptionService;

    public SubscriptionController(SubscriptionService subscriptionService)
    {
        _subscriptionService = subscriptionService;
    }

    [HttpGet]
    public async Task<SubscriptionDto[]> GetSubscriptions()
    {
        var result = await _subscriptionService.GetAllAsync();
        return result;
    }

    [HttpPost]
    public async Task<ActionResult> UploadSubscriptions([FromForm] List<IFormFile> files)
    {
        var result = await _subscriptionService.UploadSubscriptions(files);
        return result.ToActionResult();
    }
}
