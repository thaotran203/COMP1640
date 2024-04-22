using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebEnterprise_1640.Data;
using WebEnterprise_1640.Models;
using WebEnterprise_1640.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddSession();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<UserModel>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentity<UserModel, RoleModel>()
              .AddDefaultUI()
              .AddEntityFrameworkStores<ApplicationDbContext>()
              .AddDefaultTokenProviders();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Account/Login";

    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireManager", policy => policy.RequireRole(Role.Admin, Role.Admin));
    //options.AddPolicy("RequireManager", policy => policy.RequireRole(Role.Admin, Role.Admin));
    //options.AddPolicy("RequireManager", policy => policy.RequireRole(Role.Admin, Role.Admin));
});


builder.Services.AddSession();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "Cookie";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(720);
    options.LoginPath = new PathString("/Login");
    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
    options.SlidingExpiration = true;
});


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
  );
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapAreaControllerRoute(
    name: "Coordinator",
    areaName: "Coordinator",
    pattern: "SelectedArticle",
    defaults: new { controller = "SelectedArticle", action = "Index" });

app.MapAreaControllerRoute(
    name: "User",
    areaName: "User",
    pattern: "Profile",
    defaults: new { controller = "User", action = "Index" });
app.MapAreaControllerRoute(
    name: "Coordinator",
    areaName: "Coordinator",
    pattern: "Magazine",
    defaults: new { controller = "Magazine", action = "Index" });
app.MapAreaControllerRoute(
    name: "Coordinator",
    areaName: "Coordinator",
    pattern: "MagazineSelected",
    defaults: new { controller = "MagazineSelected", action = "Index" });
app.MapAreaControllerRoute(
	  name: "User",
	  areaName: "User",
	  pattern: "Profile/Test/{id?}",
	  defaults: new { controller = "Profile", action = "Test" });
app.MapAreaControllerRoute(
    name: "Coordinator",
    areaName: "Coordinator",
    pattern: "Article",
    defaults: new { controller = "Articles", action = "Index" });
app.MapAreaControllerRoute(
    name: "Manager",
    areaName: "Manager",
    pattern: "Dashboard",
    defaults: new { controller = "Dashboard", action = "Index" });
app.MapAreaControllerRoute(
    name: "Coordinator",
    areaName: "Coordinator",
    pattern: "Dashboard",
    defaults: new { controller = "Dashboard", action = "Index" });
//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//    name: "default",
//    pattern: "{area=Unauthenticated}/{controller=Home}/{action=Index}/{id?}");
//    endpoints.MapRazorPages();
//});

app.MapRazorPages();

app.Run();
