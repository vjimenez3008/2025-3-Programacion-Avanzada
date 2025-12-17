using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using HotelmasCarga.Data;

public class Startup
{
    public IConfiguration Configuration { get; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews();
        services.AddDistributedMemoryCache();
        services.AddSession();
        services.AddHttpContextAccessor();

        services.AddDbContext<HotelmasCargaContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
    }

    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        // For debugging in this academic project, always show detailed exceptions
        app.UseDeveloperExceptionPage();

        // Apply pending EF Core migrations at startup (development convenience)
        try
        {
            using var scope = app.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<HotelmasCargaContext>();
            ctx.Database.Migrate();
        }
        catch
        {
            // swallow migration errors in this academic/demo environment
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
    }
}
