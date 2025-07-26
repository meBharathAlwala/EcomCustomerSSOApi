using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddCors(o => o.AddPolicy("default", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
}));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Adds Microsoft Identity platform (AAD v2.0) support to protect this Api
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(options =>

        {
            configuration.Bind("AzureAd", options);
            options.Events = new JwtBearerEvents();

            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();

                    // Access the scope claim (scp) directly
                    var scopeClaim = context.Principal?.Claims.FirstOrDefault(c => c.Type == "scp")?.Value;

                    if (scopeClaim != null)
                    {
                        logger.LogInformation("Scope found in token: {Scope}", scopeClaim);
                    }
                    else
                    {
                        logger.LogWarning("Scope claim not found in token.");
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError("Challenge error: {ErrorDescription}", context.ErrorDescription);
                    return Task.CompletedTask;
                }
            };
        }, options => { configuration.Bind("AzureAd", options); });

// The following flag can be used to get more descriptive errors in development environments
IdentityModelEventSource.ShowPII = false;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("default");

app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();

app.Run();
