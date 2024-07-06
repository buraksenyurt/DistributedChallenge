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
        var ownerFullName = HttpContext.Session.GetString("OwnerFullName");
        var ownerTitle = HttpContext.Session.GetString("OwnerTitle");
        var ownerEmployeeId = HttpContext.Session.GetString("OwnerEmployeeId");

        if (string.IsNullOrEmpty(ownerEmployeeId))
        {
            return RedirectToAction("Lobby");
        }

        var model = new ReportRequestModel
        {
            Owner = new OwnerModel
            {
                FullName = ownerFullName,
                Title = ownerTitle,
                EmployeeId = ownerEmployeeId
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
            };

            var response = await _messengerServiceClient.SendNewReportRequestAsync(payload);
            _logger.LogInformation("Messenger service response is '{Response}'", response);
            if (response.StatusCode == Domain.Enums.StatusCode.Success)
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

    public IActionResult Lobby()
    {
        var model = new OwnerModel();
        return View(model);
    }

    [HttpPost]
    public IActionResult Entry(OwnerModel owner)
    {
        if (ModelState.IsValid)
        {
            HttpContext.Session.SetString("OwnerFullName", owner.FullName ?? "John Doe");
            HttpContext.Session.SetString("OwnerTitle", owner.Title ?? "CTO");
            HttpContext.Session.SetString("OwnerEmployeeId", owner.EmployeeId ?? "CTO-1001");
            return RedirectToAction("Index");
        }
        return View(owner);
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Lobby");
    }
}