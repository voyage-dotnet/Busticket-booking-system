using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;
using BusTicketSystem.MVC.Services;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();



builder.Services.AddControllersWithViews();
builder.Services.AddTransient<BusTicketSystem.MVC.Handlers.AuthHeaderHandler>();
builder.Services.AddHttpClient("VoyageAPI", client => { })
    .AddHttpMessageHandler<BusTicketSystem.MVC.Handlers.AuthHeaderHandler>();
builder.Services.AddScoped<BusTicketSystem.MVC.Services.VoyageApiClient>();
builder.Services.AddAuthentication();

builder.Services.AddHttpClient("BusApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5135/api/");
});


builder.Services.AddAutoMapper(typeof(BusTicketSystem.Web.Mapping.TripMappingProfile).Assembly);

// ── Session (stores JWT token after login) ────────────────────────────────────
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// ── HttpContextAccessor (needed by ApiService to read session) ────────────────
builder.Services.AddHttpContextAccessor();

// ── HttpClient + ApiService ───────────────────────────────────────────────────
// Base address is configured in appsettings.json under "ApiBaseUrl"
builder.Services.AddHttpClient<ApiService>(client =>
{
    var baseUrl = builder.Configuration["ApiBaseUrl"];

    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        baseUrl = "https://localhost:7001/";
    }

    if (!baseUrl.EndsWith("/"))
    {
        baseUrl += "/";
    }

    Console.WriteLine("====================================");
    Console.WriteLine($"MVC API BASE URL = {baseUrl}");
    Console.WriteLine("====================================");

    client.BaseAddress = new Uri(baseUrl);

    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseSession();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
