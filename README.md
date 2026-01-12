# mTLS Demo Application

This is a simple demo application that showcases mutual TLS (mTLS)
authentication between a client and a server. The application consists of a
server that requires clients to present valid certificates for authentication,
and a client that connects to the server using its own certificate.

## Vayapay

We will use Vayapays staging environment for this demo application:

- <https://api-gw.staging.vayapay.com>

```bash
curl -v --cert ./vayapay.crt --key ./company.key \
https://api-gw.staging.vayapay.com
```

## Notes on `AddHttpClient<T>`

In our Program.cs file, you may notice the following line of code:

```csharp
builder.Services.AddHttpClient<VayapayClient>(config => {
  config.BaseAddress = theuri;
});
```

The `.AddHttpClient<T>` is an extension method provided by the
`Microsoft.Extensions.Http` nuget package that extends the ServiceCollection in
the `Microsoft.Extensions.DependencyInjection` namespace.

`Microsoft.Extensions.Http` provides a managed, DependencyInjection-integrated,
resilient way to create and use HttpClient instances. At the core is the
IHttpClientFactory with the default DefaultHttpClientFactory implementation.

### Inner Workings

The extension method `AddHttpClient<T>` adds the IHttpClientFactory and related
services to the IServiceCollection and configures a binding between the
VayapayClient type and a named HttpClient. The client name will be set to the
type name of `VayapayClient`.
