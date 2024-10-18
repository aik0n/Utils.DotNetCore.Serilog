using SerilogExtended.Implementation;

namespace SerilogExtendedWebExample
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddSerilogLogging(Configuration); // Add this line
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // add this to use diagnostics HTTP logs
            // log enricher implementation can be replaced with custom one
            app.UseSerilogRequestMiddleware(new LogEnricher());
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Main}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}