using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQExcelCreate.Web.Models;
using RabbitMQExcelCreate.Web.Services;

namespace RabbitMQExcelCreate.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton(sp => new ConnectionFactory()
            {
                Uri = new Uri(builder.Configuration.GetConnectionString("RabbitMQ")),
                DispatchConsumersAsync = true
            });
            builder.Services.AddSingleton<RabbitMQClientService>();

            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
            });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<AppDbContext>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                appDbContext.Database.Migrate();
                if (!appDbContext.Users.Any())
                {
                    userManager.CreateAsync(new IdentityUser()
                    {
                        UserName = "deneme1",
                        Email = "deneme1@gmail.com",
                    }, "Password12*").Wait();
                    userManager.CreateAsync(new IdentityUser()
                    {
                        UserName = "deneme2",
                        Email = "deneme2@gmail.com",
                    }, "Password12*").Wait();
                }
            }
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}