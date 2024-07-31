using GamersWorld.Domain.Dtos;
using GamersWorld.WebApp.Models;
using GamersWorld.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace GamersWorld.WebApp.Controllers;

public class AccountController(ILogger<AccountController> logger, IdentityServiceClient identityServiceClient) : Controller
{
    private readonly ILogger<AccountController> _logger = logger;
    private readonly IdentityServiceClient _identityServiceClient = identityServiceClient;

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var gtResponse = await _identityServiceClient.GetToken(new LoginDto
            {
                RegistrationId = model.RegistrationId,
                Password = model.Password,
            });
            if (gtResponse == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            else
            {
                _logger.LogInformation("Saving token to session");

                HttpContext.Session.SetString("JWToken", gtResponse.Token);
                HttpContext.Session.SetString("EmployeeId", model.RegistrationId);
                HttpContext.Session.SetString("EmployeeTitle", gtResponse.EmployeeTitle);
                HttpContext.Session.SetString("EmployeeFullname", gtResponse.EmployeeFullname);

                TempData["JWToken"] = gtResponse.Token;
                TempData["EmployeeId"] = model.RegistrationId;

                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        }
        return View(model);
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Account");
    }
}
