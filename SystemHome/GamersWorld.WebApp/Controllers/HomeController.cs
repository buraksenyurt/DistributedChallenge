using Microsoft.AspNetCore.Mvc;
using GamersWorld.WebApp.Models;
using GamersWorld.Domain.Requests;
using GamersWorld.Domain.Responses;
using GamersWorld.WebApp.Services;

namespace GamersWorld.WebApp.Controllers;

public class HomeController(ILogger<HomeController> logger, MessengerServiceClient messengerServiceClient)
    : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly MessengerServiceClient _messengerServiceClient = messengerServiceClient;

    public IActionResult Index()
    {
        var token = HttpContext.Session.GetString("JWToken");
        var employeeId = HttpContext.Session.GetString("EmployeeId");

        ViewBag.JWToken = TempData["JWToken"];
        ViewBag.EmployeeId = TempData["EmployeeId"];

        if (string.IsNullOrEmpty(employeeId) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Account");
        }

        var model = new ReportRequestModel
        {
            Owner = new OwnerModel
            {
                EmployeeId = employeeId
            }
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitReport(ReportRequestModel report)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation("'{ReportOwner}' requested a new report.", report.Owner.EmployeeId);

            var payload = new NewReportRequest
            {
                Title = report.ReportTitle ?? "None",
                Expression = report.Expression ?? "None",
                EmployeeId = report.Owner.EmployeeId,
                Lifetime = report.Lifetime
            };

            var response = await _messengerServiceClient.SendNewReportRequestAsync(payload);
            _logger.LogInformation("Messenger service response is '{Response}'", response);
            if (response.Status == Domain.Enums.Status.Success)
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