using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using IdentityServerHost.Quickstart.UI;

namespace Identity
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; set; }
        public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; set; }
        public Startup(IWebHostEnvironment environment, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            Configuration = configuration;
            Environment = environment;

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireDigit = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequiredLength= 6;
                                                                                                                              // opt.SignIn.RequireConfirmedEmail = true;
            })

                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
                 
            // services.AddIdentityServer()
            //    .AddInMemoryClients(Config.Clients)
            //    .AddInMemoryIdentityResources(Config.IdentityResources)
            //    .AddInMemoryApiResources(Config.ApiResources)
            //    .AddInMemoryApiScopes(Config.ApiScopes)
            //    .AddTestUsers(Config.Users)
            //    .AddDeveloperSigningCredential();

            services.AddIdentityServer()
               .AddAspNetIdentity<ApplicationUser>()
               //.AddConfigurationStore(options =>
               //{
               //    options.ConfigureDbContext = builder => builder.UseSqlServer(connectionString); // to tell identity framework which assembly is hosting the migration 
               //})
                   //.AddOperationalStore(options =>
                   //{
                   //    options.ConfigureDbContext = builder => builder.UseSqlite(connectionString,
                   //    opt => opt.MigrationsAssembly(migrationsAssembly));
                   //})
                   .AddInMemoryClients(Config.Clients)
                   .AddInMemoryIdentityResources(Config.IdentityResources)
                   .AddInMemoryApiResources(Config.ApiResources)
                   .AddInMemoryApiScopes(Config.ApiScopes)
               .AddDeveloperSigningCredential();
            services.Configure<IdentityServerSettings>(Configuration.GetSection("IdentityServerSettings"));
            services.AddSingleton<ITokenService, TokenService>();
            services.AddControllersWithViews();
            services.AddControllers()
                .AddNewtonsoftJson(options =>
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddSwaggerGen(c =>
           {
               c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
           });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }
    }
}
