//  1- make new solution dotnet web empty for IdentityServer4
//      - install IdentityServer4 package
//      - change port to 5443 
//      - add configuration to stratup
//      - add Config.cs 
//  2- make new solution dotnet webapi
//      - install package
//          - IdentityServer4.AccessTokenValidation
//      - add in ConfigureServices()
//          services.AddAuthentication("Bearer")
//           .AddIdentityServerAuthentication("Bearer", options =>
//           {
//               options.ApiName = "ExcpApi";
//               options.Authority = "https://localhost:5443";
//           });
//      - add in Configure()
//          - app.UseAuthentication();
//      - excute command in postman to get token
//          curl -XPOST -H 'Content-Type: application/x-www-form-urlencoded' -H 'Cache-Control: no-cache' -d 'client_id=m2m.client&scope=weatherapi.read&client_secret=SuperSecretPassword&grant_type=client_credentials' 'https://localhost:5443/connect/token'
//      - add [Authorize] on WeatherForecastController
//      - excute this command to access controller
//          curl -H 'Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IkIxMjk3Qjg0RjUxNTJFMkU5RDgxMjZEQ0JFOEM1MzMyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE2NDMxODMzMzIsImV4cCI6MTY0MzE4NjkzMiwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6NTQ0MyIsImF1ZCI6IndlYXRoZXJhcGkiLCJjbGllbnRfaWQiOiJtMm0uY2xpZW50IiwianRpIjoiQjY2RkM4OTlGM0JBNzUzRUQ4NzNEQzJCMTk5QjYyMzYiLCJpYXQiOjE2NDMxODMzMzIsInNjb3BlIjpbIndlYXRoZXJhcGkucmVhZCJdfQ.L3iAGVXP3X440aLoIFMrrAe61noLEiO5OQiBPx3me3pFeyuDvmAyi_VNauf8rR0i01ORNZ54oxorOtrKMo02jNxGHC0gGSOQL2jtc5t71hBHwk36nw3XZGgq07snpIpQGFUHk-PGHJCjz4YYp9jYrTB_XGlK-rO_-LA2J9qJPcu8o02RRv9Pd9e9NRFgGdhEv4FU4NswPc0KmiM2RRoayWK2B6J71uc8iWTbO4dwFZvi8wy7gpS-644aPPUmaw9G1dqKTrEhtmpRALd4k_-uF0jldeaBHfUJCIn-AY-k1HR2YgRhgWp4maqUcf-lYbMx-ROLMMPrvikkP3dU2TtYHA' -H 'Cache-Control: no-cache' 'https://localhost:5445/WeatherForecast'
//  3- make new solution mvc
//      - install IdentityModel package
//      - add in home controller => weather
//      - add Services folder
//          - add IdentityServiceSetting
//          - add ITokenService
//          - add TokenService
//      - add [Authorize] on home controller => weather  
//      - add in startup => ConfigureServices but don't install Microsoft.AspNetCore.Authentication.OpenIdConnect package
//           services.AddAuthentication(options =>
//              {
//                  options.DefaultScheme = "cookie"; // the name of the cookie that's going to be used to maintain the client's authentication state
//                  options.DefaultChallengeScheme = "oidc"; //  
//              })
//              .AddCookie("cookie")
//              .AddOpenIdConnect("oidc", options =>
//                  {
//                      options.Authority = Configuration["InteractiveServiceSettings:AuthorityUrl"];
//                      options.ClientId = Configuration["InteractiveServiceSettings:ClientId"];
//                      options.ClientSecret = Configuration["InteractiveServiceSettings:ClientSecret"];

//                      options.ResponseType = "code";
//                      options.UsePkce = true;
//                      options.ResponseMode = "query";

//                      options.Scope.Add(Configuration["InteractiveServiceSettings:Scopes:0"]);
//                      options.SaveTokens = true;

//                  });
//          services.Configure<IdentityServerSettings>(Configuration.GetSection("IdentityServerSettings"));
//          services.AddSingleton<ITokenService, TokenService>();
//      - Add Entity Framework
//            - Install package
//                  - IdentityServer4.EntityFramework
//                  - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
//                  - Microsoft.EntityFrameworkCore.Tools
//                  - Microsoft.EntityFrameworkCore.Sqlite
//           - change configuration service
//              public void ConfigureServices(IServiceCollection services)
//               {
//                   var connectionString = Configuration.GetConnectionString("");
//                   var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
//                   // services.AddIdentityServer()
//                   //    .AddInMemoryClients(Config.Clients)
//                   //    .AddInMemoryIdentityResources(Config.IdentityResources)
//                   //    .AddInMemoryApiResources(Config.ApiResources)
//                   //    .AddInMemoryApiScopes(Config.ApiScopes)
//                   //    .AddTestUsers(Config.Users)
//                   //    .AddDeveloperSigningCredential();

//                   services.AddIdentityServer()
//                      .AddTestUsers(Config.Users)
//                      .AddConfigurationStore(options =>
//                      {
//                          options.ConfigureDbContext = builder => builder.UseSqlite(connectionString,
//                          opt => opt.MigrationsAssembly(migrationsAssembly)); // 
//                      })
//                      .AddOperationalStore(options =>
//                      {
//                          options.ConfigureDbContext = builder => builder.UseSqlite(connectionString,
//                          opt => opt.MigrationsAssembly(migrationsAssembly));
//                      })
//                      .AddDeveloperSigningCredential();

//                   services.AddControllersWithViews();
//               }
//           - run migration
//             - dotnet ef migrations add InitialIdentityServerMigration -c PersistedGrantDbContext     // for operational store 
//             - dotnet ef migrations add InitialIdentityServerMigration -c ConfigurationDbContext     // for configuration store
//             - dotnet ef database update -c PersistedGrantDbContext
//             - dotnet ef database update -c ConfigurationDbContext
//      -Add Identity to IdentityServer4
//          - install package
//               - IdentityServer4.AspNetIdentity
//               - Microsoft.AspNetCore.Identity.EntityFrameworkCore
//          - in startup
//              services.AddDbContext<ApplicationDbContext>(options =>
//                  options.UseSqlite(connectionString, sqlOption => sqlOption.MigrationsAssembly(migrationsAssembly)));

//             - dotnet ef migrations add InitialIdentityServerMigration -c ApplicationDbContext     // for operational store 
//            
//          
//          
//          
//          
//          
//          
//          
//          
//          
//          
//          
//          
//          
// 