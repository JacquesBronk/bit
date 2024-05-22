using Bit;
using Bit.Log;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.ConfigureAppConfiguration();
builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

app.MapDefaultEndpoints();
app.ConfigureMiddlewareAndEndpoints();

app.Run();




