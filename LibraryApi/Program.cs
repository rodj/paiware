using Library.Core.Services;
using Library.EF.Data;
using Library.EF.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
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
app.MapControllers();

app.Run();
