using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Principal;
using WebQuizApp.Data;
using WebQuizApp.Models;
using WebQuizApp.Repositories;
using WebQuizApp.Repositories.Interfaces;
using WebQuizApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add the Unit of work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add role manager and user manager 
builder.Services.AddScoped<RoleManager<IdentityRole>>();
builder.Services.AddScoped<UserManager<ApplicationUser>>();

// Register the database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add the authentication and authorization
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Disable email confirmation
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireDigit = true;
})
.AddRoles<IdentityRole>() // Enable role management
.AddEntityFrameworkStores<ApplicationDbContext>();

// Handle the Email sevice
builder.Services.AddTransient<IEmailSender, DummyEmailSender>();



// Add other login options like Google
builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    });

// Explicitly configure Cookie Authentication options to set LoginPath correctly
builder.Services.Configure<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme, options =>
{
    options.LoginPath = "/Identity/Account/Login";  // This should be Login, NOT Logout!
    options.LogoutPath = "/Identity/Account/Logout";  // Correct for logout.
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";  // Optional, for forbidden access.
    options.Cookie.SameSite = SameSiteMode.Lax;  // Helps with cookie issues in modern browsers.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Use 'SameAsRequest' if testing on HTTP localhost.
});

builder.Services.AddRazorPages();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbInitializer.Initialize(services);
    }
    catch ( Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occured seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Prevent loop redirection 
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Home/Error";
        await next();
    }
});

app.UseHttpsRedirection();
app.UseRouting();

app.MapRazorPages();

app.UseAuthentication();

app.UseAuthorization();



// Add this middleware to handle unauthorized access
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401)
    {
        context.Response.Redirect($"/Identity/Account/Login?returnUrl={context.Request.Path}");
    }
});

app.MapStaticAssets();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Tests}/{id?}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "tests",
    pattern: "Tests/{action=Index}/{id}",
    defaults: new { controller = "Tests" }
    );




app.Run();
