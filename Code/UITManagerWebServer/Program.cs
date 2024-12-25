using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Hubs;
using UITManagerWebServer.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<SignInManager<ApplicationUser>, CustomSignInManager>();

builder.Services.Configure<IdentityOptions>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    options.User.RequireUniqueEmail = true;
});

builder.Services.AddControllersWithViews();
builder.Services.AddAntiforgery();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
}
else {
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    Console.WriteLine("hello");
    try {
        using var context = new ApplicationDbContext(
            services.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

        bool hasData = await context.Machines.AnyAsync();
        if (!hasData) {
            // Si aucune donnée n'existe, effectuer le populate
            Console.WriteLine("Database is empty. Starting population...");

            // Populate without alarm trigger today
            //await Populate.Initialize(services,true);
            // Populate with alarm trigger today
            await Populate.Initialize(services,false);            
            Console.WriteLine("Database populated successfully.");
        }
        else {
            Console.WriteLine("Database already contains data. Skipping population.");
        }
    }
    catch (Exception ex) {
        Console.WriteLine($"An error occurred while populating the database: {ex.Message}");
    }
}

// Ajouter l'en-tête X-Content-Type-Options: nosniff
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapHub<WebAppHub>("/WebAppHub");

app.Run();