using Microsoft.AspNetCore.Mvc;
using GamersWorld.WebApp.Models;
using GamersWorld.Common.Requests;
using GamersWorld.WebApp.Utility;
using GamersWorld.Common.Responses;

namespace GamersWorld.WebApp.Controllers;

public class HomeController(ILogger<HomeController> logger, MessengerServiceClient messengerServiceClient)
    : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly MessengerServiceClient _messengerServiceClient = messengerServiceClient;

    public IActionResult Index()
    {
        var model = new ReportRequestModel();
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReport(ReportRequestModel report)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation("{ReportOwner} requested a new report. Employee is {EmployeeId}"
                , report.Owner.ToString(), report.Owner.EmployeeId);

            var payload = new NewReportRequest
            {
                Title = report.ReportTitle ?? "None",
                Expression = report.Expression ?? "None",
                EmployeeId = report.Owner.EmployeeId
            };

            var response = await _messengerServiceClient.SendNewReportRequestAsync(payload);
            _logger.LogInformation("Messenger service response is '{Response}'", response);
            if (response.StatusCode == Common.Enums.StatusCode.Success)
            {
                return RedirectToAction("RequestConfirmed");
            }
            else
            {
                CheckValidations(response);

                return View("RequestFailed", report);
            }
        }

        return View("Index", report);
    }

    private void CheckValidations(BusinessResponse response)
    {
        if (response.ValidationErrors != null)
        {
            foreach (var error in response.ValidationErrors)
            {
                foreach (var errorMessage in error.Value)
                {
                    ModelState.AddModelError(error.Key, errorMessage);
                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, response.Message);
        }
    }

    public IActionResult RequestConfirmed()
    {
        return View();
    }
}