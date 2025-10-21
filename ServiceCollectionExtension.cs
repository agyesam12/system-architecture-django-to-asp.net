using Microsoft.Extensions.DependencyInjection;
using ArtisanMarketplace.Services;

namespace ArtisanMarketplace.Extensions
{
    /// <summary>
    /// Extension methods for configuring role services
    /// </summary>
    public static class RoleServiceExtensions
    {
        /// <summary>
        /// Add role management services to the dependency injection container
        /// </summary>
        public static IServiceCollection AddRoleServices(this IServiceCollection services)
        {
            services.AddScoped<IRoleService, RoleService>();
            
            return services;
        }
    }
}

// In your Program.cs or Startup.cs, add this:
/*
using ArtisanMarketplace.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add role services
builder.Services.AddRoleServices();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
*/