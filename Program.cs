using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GolfBookingApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// Add DbContext with SQL Server
builder.Services.AddDbContext<GolfClubContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("GolfClubContext"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)
    ));

// Register the DbSeeder
builder.Services.AddScoped<DbSeeder>();

var app = builder.Build();

// checking if the database is created and seeded
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GolfClubContext>();

    // Create the database if it doesn't exist
    context.Database.EnsureCreated();

    // Seed the database
    var seeder = services.GetRequiredService<DbSeeder>();
    seeder.SeedDatabase().Wait();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
