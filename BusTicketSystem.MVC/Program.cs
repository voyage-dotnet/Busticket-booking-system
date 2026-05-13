using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("BusApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5135/api/");
});


builder.Services.AddAutoMapper(typeof(BusTicketSystem.Web.Mapping.TripMappingProfile).Assembly);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
