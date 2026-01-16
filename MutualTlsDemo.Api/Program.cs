using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using MutualTlsDemo.Api.AuthenticationAlternatives;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication(options => { options.DefaultAuthenticateScheme = VayapayAuthenticationHandlerScheme.AuthenticationScheme; })
                .AddScheme<VayapayAuthenticationSchemeOptions, VayapayAuthenticationHandler>(VayapayAuthenticationHandlerScheme.AuthenticationScheme, x => { x.ApiKey = "supersecret"; });

builder.Services.AddHttpClient<VayapayClient>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    Uri theuri = new(config["Vayapay:BaseUrl"]!);
    client.BaseAddress = theuri;
}).ConfigurePrimaryHttpMessageHandler((sp) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var handler = new HttpClientHandler();
    var base64Cert = config["Vayapay:ClientCertificate:CertBase64Second"];
    var base64CertKey = config["Vayapay:ClientCertificate:CertKeyBase64Second"];

    var certificate = X509CertificateLoader.LoadCertificate(Convert.FromBase64String(base64Cert!));

    var keyPem = Encoding.UTF8.GetString(Convert.FromBase64String(base64CertKey!));
    using var ecdsa = ECDsa.Create();
    ecdsa.ImportFromPem(keyPem);

    var certWithKey = certificate.CopyWithPrivateKey(ecdsa);
    handler.ClientCertificates.Add(certWithKey);
    return handler;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
