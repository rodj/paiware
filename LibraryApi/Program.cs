using LibraryApi.Data;
using LibraryApi.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<InMemoryDataStore>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
