using Microsoft.AspNetCore.ResponseCompression;

using Smart.AspNetCore.ApplicationModels;

using System.IO.Compression;
using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

//--------------------------------------------------------------------------------
// Add services to the container.
//--------------------------------------------------------------------------------

builder.Services.Configure<RouteOptions>(static options =>
{
    options.AppendTrailingSlash = true;
});

builder.Services
    .AddControllersWithViews(static options =>
    {
        options.Conventions.Add(new LowercaseControllerModelConvention());
    })
    .AddJsonOptions(static options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
        options.JsonSerializerOptions.Converters.Add(new WorkServer.DateTimeConverter());
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Compression
builder.Services.AddRequestDecompression();
builder.Services.AddResponseCompression(static options =>
{
    // Default false (for CRIME and BREACH attacks)
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = [MediaTypeNames.Application.Json];
});
builder.Services.Configure<GzipCompressionProviderOptions>(static options =>
{
    options.Level = CompressionLevel.Fastest;
});

var app = builder.Build();

//--------------------------------------------------------------------------------
// Configure the HTTP request pipeline.
//--------------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(static o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseAuthorization();
app.UseAuthentication();

// Compression
app.UseRequestDecompression();
app.UseResponseCompression();

app.MapControllers();

app.Run();
