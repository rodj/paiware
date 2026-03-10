using Library.Core.Services;
using Library.EF.Data;
using Library.EF.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Allow the Vite dev server to call the API during local development.
// In production the React app is on the same domain, so CORS is not needed.
builder.Services.AddCors(options =>
    options.AddPolicy("DevCors", policy =>
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()));
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICheckoutService, EfCheckoutService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger  = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                       .CreateLogger("Startup");
    var context = scope.ServiceProvider.GetRequiredService<LibraryContext>();

    logger.LogInformation("Applying database migrations...");
    context.Database.Migrate();
    logger.LogInformation("Migrations applied.");

    DbSeeder.Seed(context, logger);
}

app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
if (app.Environment.IsDevelopment()) app.UseCors("DevCors");
app.MapControllers();

app.Run();
