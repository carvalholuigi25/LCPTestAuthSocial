using LCPTestAuthSocial.Server.Data;
using LCPTestAuthSocial.Server.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer()
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    })
    .AddFacebook(fbOptions =>
    {
        fbOptions.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        fbOptions.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
    })
    .AddTwitter(twOptions =>
    {
        twOptions.ConsumerKey = builder.Configuration["Authentication:Twitter:ConsumerKey"]!;
        twOptions.ConsumerSecret = builder.Configuration["Authentication:Twitter:ConsumerSecret"]!;
    })
    .AddGitHub(ghOptions =>
    {
        ghOptions.ClientId = builder.Configuration["Authentication:Github:ClientId"]!;
        ghOptions.ClientSecret = builder.Configuration["Authentication:Github:ClientSecret"]!;
    });
    //.AddMicrosoftAccount(micOptions =>
    //{
    //    micOptions.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"]!;
    //    micOptions.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"]!;
    //});

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseIdentityServer();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
