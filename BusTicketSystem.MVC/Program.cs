using BusTicketSystem.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

// ── MVC ──────────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

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

// ─────────────────────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session must come before Authorization
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
