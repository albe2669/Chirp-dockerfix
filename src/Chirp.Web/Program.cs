using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SimpleDB;
using Chirp.Razor;
using Chirp.Infrastructure;
using Chirp.Core;

var builder = WebApplication.CreateBuilder(args);
//var dbPath = "";

//builder.Logging.AddConsole();


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddDbContext<ChirpDBContext>();

/*builder.Services.AddDbContext<ChirpDBContext>((serviceProvider, options) =>
{
    var dbPath = Environment.GetEnvironmentVariable("CHIRPDBPATH") ??
    Path.Combine(Path.GetTempPath(), "chirp.db");
    options.UseSqlite($"Data Source={dbPath}"); 
}, ServiceLifetime.Scoped);*/

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<ChirpDBContext>();
        context.Database.Migrate();
        DbInitializer.SeedDatabase(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB.");
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.Run();
public partial class Program { }