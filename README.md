# mTLS Demo Application

This is a simple demo application that showcases mutual TLS (mTLS)
authentication between a client and a server. The application also shows how to
setup HEADER based authentication.

## Vayapay

We will use Vayapays staging environment for this demo application:

- <https://api-gw.staging.vayapay.com>

```bash
curl -v --cert ./vayapay.crt --key ./company.key \
https://api-gw.staging.vayapay.com
```

## Notes on the `X-API-KEY` Authentication

Vayapay uses an API key-based authentication mechanism to secure calls to
`Merchant APIs`. We can implement this in mulitple ways:

1. AuthenticationHandler to validate the API key on incoming requests.
2. Using customer Middleware to validate the API key on incoming requests.
   Brittle path based logic.
3. Using IEndpointFilter to validate the API key on incoming requests.

This is implemented in our API using IEndpointFilter to intercept incoming
requests and validate the presence and correctness of the `X-API-KEY` header.

### AuthenticationHandler Approach

Gives me more control over the authentication process and allows me to leverage
built-in ASP.NET Core authentication and authorization features.

```markdown
Request
 ↓
UseAuthentication
   (no auth yet)
 ↓
UseAuthorization
   ↓
   [Authorize] attribute
      ↓
      Which scheme?
         → none specified
         → use DefaultScheme = "ApiKey"
      ↓
      AuthenticationService.AuthenticateAsync("ApiKey")
      ↓
      VayapayAuthenticationHandler.HandleAuthenticateAsync()
```

## Notes on `AddHttpClient<T>`

In our Program.cs file, you may notice the following line of code:

```csharp
builder.Services.AddHttpClient<VayapayClient>(config => {
  config.BaseAddress = theuri;
});
```

The `.AddHttpClient<T>` is an extension method provided by the
`Microsoft.Extensions.Http` nuget package that extends the `ServiceCollection`
in the `Microsoft.Extensions.DependencyInjection` namespace.

`Microsoft.Extensions.Http` provides a managed, DependencyInjection-integrated,
resilient way to create and use HttpClient instances. At the core is the
IHttpClientFactory with the default DefaultHttpClientFactory implementation.

### Inner Workings

The extension method `AddHttpClient<T>` adds the IHttpClientFactory and related
services to the IServiceCollection and configures a binding between the
VayapayClient type and a named HttpClient. The client name will be set to the
type name of `VayapayClient`.

Calling `AddHttpClient<T>()` multiple times:

1. Registers IHttpClientFactory once (first call)
2. Adds a separate named configuration “bucket” per typed client
3. Registers each typed client as transient, wired to call the shared factory
  at runtime
