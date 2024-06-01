using Microsoft.AspNetCore.Mvc;
using GamersWorld.WebApp.Models;

namespace GamersWorld.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult SubmitReport(ReportRequestModel report)
    {
        if (ModelState.IsValid)
        {
            _logger.LogInformation("{} bir rapor talebinde bulundu.", report.Owner.ToString());

            return RedirectToAction("RequestConfirmed");
        }
        return View("Index", report);
    }

    public IActionResult RequestConfirmed()
    {
        return View();
    }
}
