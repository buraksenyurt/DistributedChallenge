using Microsoft.AspNetCore.Mvc;
using GamersWorld.WebApp.Models;
using GamersWorld.Common.Messages.Requests;
using GamersWorld.WebApp.Utility;

namespace GamersWorld.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly MessengerServiceClient _messengerServiceClient;

    public HomeController(ILogger<HomeController> logger, MessengerServiceClient messengerServiceClient)
    {
        _logger = logger;
        _messengerServiceClient = messengerServiceClient;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReport(ReportRequestModel report)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation("{ReportOwner} bir rapor talebinde bulundu.", report.Owner.ToString());

            var payload = new NewReportRequest
            {
                Title = report.ReportTitle ?? "None",
                Expression = report.Expression ?? "None",
            };

            var response = await _messengerServiceClient.SendNewReportRequestAsync(payload);

            if (response.StatusCode == Common.Enums.StatusCode.Success)
            {
                return RedirectToAction("RequestConfirmed");
            }
            else
            {
                ModelState.AddModelError(string.Empty, response.Message);
            }

            return RedirectToAction("RequestConfirmed");
        }
        return View("Index", report);
    }

    public IActionResult RequestConfirmed()
    {
        return View();
    }
}
