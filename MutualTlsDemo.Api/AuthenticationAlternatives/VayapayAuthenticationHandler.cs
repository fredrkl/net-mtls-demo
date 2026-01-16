using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MutualTlsDemo.Api.AuthenticationAlternatives;

public class VayapayAuthenticationHandler(IOptionsMonitor<VayapayAuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<VayapayAuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
      if (!Request.Headers.TryGetValue("X-API-KEY", out var key))
        return Task.FromResult(AuthenticateResult.NoResult());

      var providedKey = key.ToString();
      if(string.IsNullOrWhiteSpace(providedKey))
          return Task.FromResult(AuthenticateResult.Fail("No API Key"));

      if( providedKey != Options.ApiKey)
          return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));

      var claims = new[] { new Claim("client", "vayapay") };
      var identity = new ClaimsIdentity(claims, Scheme.Name);
      var principal = new ClaimsPrincipal(identity);
      var ticket = new AuthenticationTicket(principal, Scheme.Name);

      return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

public static class VayapayAuthenticationHandlerScheme
{
    public const string AuthenticationScheme = "ApiKey";
}

public class VayapayAuthenticationSchemeOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; } = string.Empty;
}
