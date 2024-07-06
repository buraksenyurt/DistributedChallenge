using GamersWorld.Domain.Requests;
using GamersWorld.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamersWorld.WebApp.Controllers;

public class ReportsController(ILogger<ReportsController> logger, MessengerServiceClient messengerServiceClient)
    : Controller
{
    private readonly ILogger<ReportsController> _logger = logger;
    private readonly MessengerServiceClient _messengerServiceClient = messengerServiceClient;

    public async Task<IActionResult> Index()
    {
        var ownerEmployeeId = HttpContext.Session.GetString("OwnerEmployeeId");

        if (string.IsNullOrEmpty(ownerEmployeeId))
        {
            return RedirectToAction("Lobby", "Home");
        }
        _logger.LogInformation("Getting reports for {OwnerEmployeeId}", ownerEmployeeId);

        var request = new GetReportsByEmployeeRequest
        {
            EmployeeId = ownerEmployeeId
        };
        var reportDocuments = await _messengerServiceClient.GetReportDocumentsByEmployeeAsync(request);
        return View(reportDocuments);
    }
}
