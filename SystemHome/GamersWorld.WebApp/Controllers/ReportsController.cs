using GamersWorld.Domain.Requests;
using GamersWorld.WebApp.Models;
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
        var data = new List<ReportModel>();
        foreach (var reportDocument in reportDocuments)
        {
            data.Add(new ReportModel
            {
                Id = reportDocument.Id,
                DocumentId = reportDocument.DocumentId,
                InsertTime = reportDocument.InsertTime,
            });
        }
        return View(data);
    }

    [HttpGet("Reports/Download")]
    public async Task<IActionResult> Download(string documentId)
    {
        var document = await _messengerServiceClient
            .GetReportDocumentByIdAsync(new GetReportDocumentByIdRequest
            {
                DocumentId = documentId
            });
        if (document?.Base64Content != null)
        {
            var content = Convert.FromBase64String(document.Base64Content);
            return File(content, "application/octet-stream", $"{documentId}.txt");
        }
        return NotFound();
    }
}
