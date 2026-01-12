using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Console.WriteLine("Certificate Base64 from Configuration:");
Console.WriteLine(builder.Configuration["Vayapay:ClientCertificate:CertBase64"]);

Uri theuri = new(builder.Configuration["Vayapay:BaseUrl"]!);

builder.Services.AddHttpClient<VayapayClient>(config => {
    config.BaseAddress = theuri;
  }).ConfigurePrimaryHttpMessageHandler(() => {
    var handler = new HttpClientHandler();
    var base64Cert = builder.Configuration["Vayapay:ClientCertificate:CertBase64"];
    var cert = new X509Certificate2(Convert.FromBase64String(base64Cert!));
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
