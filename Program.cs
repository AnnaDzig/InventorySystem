using InventorySystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using InventorySystem.Models;
using InventorySystem.Services;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");



builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
  if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
  {
    // Windows → SQL Server
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
  }
  else
  {
    // Mac/Linux → SQLite
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"));
  }
});


// Cookie-based Authentication (Claims)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
      options.LoginPath = "/Account/Login";
      options.LogoutPath = "/Account/Logout";
    });

//  Реєстрація потрібних сервісів
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<EmailService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
