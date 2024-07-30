using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GamersWorld.Application;
using GamersWorld.Application.Contracts.Data;
using GamersWorld.Domain.Dtos;
using GamersWorld.Domain.Entity;
using GamersWorld.Repository;
using Microsoft.IdentityModel.Tokens;
using SecretsAgent;
using Steeltoe.Discovery.Client;
using Steeltoe.Discovery.Consul;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddData();
builder.Services.AddServiceDiscovery(o => o.UseConsul());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/register", async (EmployeeDto employeeDto, IEmployeeDataRepository repository, ILogger<Program> logger) =>
{
    var passwordHash = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password);

    logger.LogInformation("Password hash created");

    var insertedId = await repository.Register(new Employee
    {
        Fullname = employeeDto.Fullname,
        Email = employeeDto.Email,
        Title = employeeDto.Title,
        RegistrationId = employeeDto.RegistrationId,
        PasswordHash = passwordHash
    });

    logger.LogInformation("Employee created with Id {InsertedId}", insertedId);

    return Results.Ok("Employee registered successfully!");
});

app.MapPost("/api/login", async (LoginDto login, IEmployeeDataRepository repository, ISecretStoreService secretService, ILogger<Program> logger) =>
{
    var employee = await repository.Get(login.RegistrationId);
    if (employee == null)
    {
        logger.LogError("Employee did not found");
        return Results.Unauthorized();
    }

    if (employee.PasswordHash != null && BCrypt.Net.BCrypt.Verify(login.Password, employee.PasswordHash))
    {
        var jwtKey = await secretService.GetSecretAsync("JwtKey");
        var jwtExpiryMinutes = await secretService.GetSecretAsync("JwtExpiryMinutes");
        var jwtIssuer = await secretService.GetSecretAsync("JwtIssuer");
        var jwtAudience = await secretService.GetSecretAsync("JwtAudience");

        logger.LogInformation("Jwt Issuer {Issuer}/{Audience}. Expire in {ExpireIn} minutes.", jwtIssuer, jwtAudience, jwtExpiryMinutes);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtKey);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, login.RegistrationId),
                new Claim(JwtRegisteredClaimNames.Name, employee.Fullname),
                new Claim(JwtRegisteredClaimNames.Email, employee.Email),
                new Claim("id", login.RegistrationId)
            ]),
            Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtExpiryMinutes)),
            Issuer = jwtIssuer,
            Audience = jwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Results.Ok(new
        {
            //TODO@buraksenyurt Return more employee information for using UI
            Token = jwtToken
        });
    }
    return Results.Unauthorized();
});


app.Run();