using Microsoft.EntityFrameworkCore;
using TransactionalOutboxPatternApp.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddDbContext<EventLogDbContext>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));
builder.Services.AddDbContext<OrderDbContext>(options => options.UseSqlServer("name=ConnectionStrings:DefaultConnection"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
{
    serviceScope.ServiceProvider.GetRequiredService<EventLogDbContext>().Database.EnsureCreated();
    serviceScope.ServiceProvider.GetRequiredService<OrderDbContext>().Database.EnsureCreated();
}

app.Run();
