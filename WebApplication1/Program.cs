using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using System.DirectoryServices.AccountManagement;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonContext.Default);
});

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
       .AddNegotiate();
/*
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddHttpContextAccessor();
*/

var app = builder.Build();

app.UseAuthentication();
//app.UseAuthorization();

app.MapGet("/hello", () => new Message("Hello, world!"));

app.Run();

public record Message(string Text);

[JsonSerializable(typeof(Message))]
internal partial class AppJsonContext : JsonSerializerContext
{
}
