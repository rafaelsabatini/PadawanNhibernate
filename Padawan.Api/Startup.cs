using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Padawan.Domain.Handlers;
using Padawan.Domain.Repositories;
using Padawan.Infra.Context;
using Padawan.Infra.Repositories;
using Padawan.Infra.Transations;
using Padawan.Shared;
using System.IO;

namespace Padawan.Api
{
    public class Startup
    {
        public static IConfigurationRoot Configuration { get; set; }


        public Startup(IHostingEnvironment hostingEnvironment)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.local.json", optional: true)
                .AddEnvironmentVariables();

            // serve para incluir as variaveis locais do desenvolvedor sem ter que mudar o arquivo appsettings.json
            //para incluir suas configurações pessoais clicar com o botão diretio em cima do projeto Padawan.Api e clicar em manage user sercrets
            if (hostingEnvironment.IsDevelopment())
                builder.AddUserSecrets<Startup>();

            Configuration = builder.Build();
            
        }

        public void ConfigureServices(IServiceCollection services)
        {
          

            AppSettings.ConnectionString = $"{Configuration["connectionString"]}";

            services.AddMvc();
            services.AddCors();

            services.Configure<IISOptions>(o =>
            {
                o.ForwardClientCertificate = false;
            });

            services.AddResponseCompression();

            services.AddSingleton(PadawanNHibernateHelper.SessionFactory());

            services.AddScoped<IUow, Uow>();

            services.AddTransient<IAccountRepository, AccountRepository>();

            services.AddTransient<AccountHandler, AccountHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseCors(x => {
                x.AllowAnyHeader();
                x.AllowAnyMethod();
                x.AllowAnyOrigin();
            });

            app.UseResponseCompression();

            app.UseMvc();
        }
    }
}
