using Bit;

//host setup
var builder = WebApplication.CreateBuilder(args);

// Configuration setup
builder.ConfigureAppConfiguration();

// Services registration
builder.Services.ConfigureServices(builder.Configuration);

// Build the application
var app = builder.Build();

// Middleware and endpoints
app.ConfigureMiddlewareAndEndpoints();

app.Run();




