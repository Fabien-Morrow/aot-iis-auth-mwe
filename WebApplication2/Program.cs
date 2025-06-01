using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default);
});

// Authentication / Authorization
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
       .AddNegotiate();
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    options.AddPolicy("DomainAdminsOnly", policy =>
        policy.RequireAssertion(ctx => ctx.User.IsInDomainAdmins())
    );
});


builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers()
    .ConfigureApplicationPartManager(apm =>
    {
        var razor = apm.ApplicationParts
            .OfType<Microsoft.AspNetCore.Mvc.ApplicationParts.AssemblyPart>()
            .FirstOrDefault(p => p.Name == "Microsoft.AspNetCore.Mvc.Razor");
        if (razor != null) apm.ApplicationParts.Remove(razor);
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.TypeInfoResolver = AppJsonSerializerContext.Default;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ThotWeb API v1");
    c.RoutePrefix = string.Empty;
});

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("✅ Mode Développement");
    // point d’entrée Swagger UI

}
else
{
    Console.WriteLine("⚠️ Mode Production ou autre");
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

// DTO + CONTEXT
public record Reponse(string Message);

[JsonSerializable(typeof(Reponse))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}

public static class ClaimsPrincipalExtensions
{
    public static bool IsInDomainAdmins(this ClaimsPrincipal user)
    {
        if (!user.Identity.IsAuthenticated)
            return false;

        using var ctx = new PrincipalContext(ContextType.Domain, null, "DC=test,DC=local");
        using var principal = UserPrincipal.FindByIdentity(ctx, user.Identity.Name);
        if (principal == null)
            return false;

        foreach (var group in principal.GetAuthorizationGroups())
        {
            if (group.Sid is not null && group.Sid.Value.EndsWith("-512"))
                return true;
        }

        return false;
    }
}

// CONTROLLER

[ApiController]
[Route("api/[controller]")]
public class MaintenanceController : ControllerBase
{
    [HttpPost("reset")]
    [Authorize(Policy = "DomainAdminsOnly")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(Reponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Reponse), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DummyCall()
    {
        try
        {
            await Task.CompletedTask;
            return Ok(new Reponse("success"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new Reponse(ex.Message));
        }
    }
}
