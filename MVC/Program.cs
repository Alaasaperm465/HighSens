using InfraStructure.Context;
using InfraStructure.Repos;
using InfraStructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using HighSens.Application.Interfaces.IServices;
using HighSens.Application.Services;
using HighSens.Domain.Interfaces;
using MVC.Mapping;
using HighSens.Domain;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container - full MVC with views
builder.Services.AddControllersWithViews(options =>
{
    // Add global authorization policy (require authenticated users by default)
    var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});

// Configure Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// Configure DbContext: prefer SQL Server using configuration, fallback to in-memory for local/dev
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(DBContext).Assembly.GetName().Name;
if (!string.IsNullOrWhiteSpace(connectionString))
{
    builder.Services.AddDbContext<DBContext>(opt =>
        opt.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly))
           .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
}
else
{
    builder.Services.AddDbContext<DBContext>(opt => opt.UseInMemoryDatabase("InMemDB")
        .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning)));
}

// AutoMapper
builder.Services.AddAutoMapper(typeof(InboundProfile));

// register repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<HighSens.Application.Interfaces.IRepositories.IProductRepository, ProductRepository>();
// register domain product repository (some services depend on the domain interface)
builder.Services.AddScoped<HighSens.Domain.Interfaces.IProductRepository, ProductRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IProductStockRepository, ProductStockRepository>();
builder.Services.AddScoped<ISectionStockRepository, SectionStockRepository>();
// register generic inbound repository used by InboundService
builder.Services.AddScoped<HighSens.Domain.Interfaces.IRepository<HighSens.Domain.Inbound>, InboundRepository>();

// register services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IInboundService, InboundService>();

// unit of work
builder.Services.AddScoped<HighSens.Domain.Interfaces.IUnitOfWork, UnitOfWork>();

var app = builder.Build();

// Ensure authentication middleware is used before authorization
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
