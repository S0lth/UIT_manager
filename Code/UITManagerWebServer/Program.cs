using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Data;
using UITManagerWebServer.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

/*builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>  {
    options.Password.RequireDigit = true; // Le mot de passe doit contenir au moins un chiffre
    options.Password.RequiredLength = 8; // Longueur minimale du mot de passe
    options.Password.RequireNonAlphanumeric = true; // Caractères spéciaux requis
    options.Password.RequireUppercase = true; // Majuscule obligatoire
    options.Password.RequireLowercase = true; // Minuscule obligatoire

    options.User.RequireUniqueEmail = true; // Chaque utilisateur doit avoir une adresse e-mail unique
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}





using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    Console.WriteLine("hello");
    try
    {
        Console.WriteLine("i'm in");
        await Populate.Initialize(services);
        Console.WriteLine("Database populated successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while populating the database: {ex.Message}");
    }
}




app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();