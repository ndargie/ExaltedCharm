using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Extensions;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExaltedCharm.Api
{
    public class Startup
    {

        public IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(setupAction =>
                {
                    setupAction.ReturnHttpNotAcceptable = true;
                })
                .AddMvcOptions(o =>
                {
                    o.OutputFormatters
                        .Add(new XmlDataContractSerializerOutputFormatter());
                    o.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
                });

#if DEBUG
            services.AddTransient<IMailService, LocalMailService>();
#else
            services.AddTransient<IMailService, CloudMailService>();
#endif
            services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
            services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            var connectionString = Configuration["ConnectionStrings:CharmInfoConnectionString"];
            services.AddDbContext<CharmContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<IReadOnlyRepository, EntityFrameworkReadOnlyRepository<CharmContext>>();
            services.AddScoped<IRepository, EntityFrameworkRepository<CharmContext>>();
            //.AddJsonOptions(o =>
            //{
            //    if (o.SerializerSettings.ContractResolver != null)
            //    {
            //        var castedResolver = o.SerializerSettings.ContractResolver as DefaultContractResolver;
            //        castedResolver.NamingStrategy = null;
            //    }
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CharmContext charmTypeContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            charmTypeContext.EnsureSeedDataForContext();

            app.UseStatusCodePages();
            AutoMapper.Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<Entities.CharmType, Models.CharmTypeWithoutCharmsDto>();
                    cfg.CreateMap<Entities.Charm, Models.CharmDto>().ForMember(dest => dest.Cost,
                        opt => opt.MapFrom(src => src.GetCharmCost()));
                    cfg.CreateMap<Entities.CharmType, Models.CharmTypeDto>();
                    cfg.CreateMap<Models.CharmCreationDto, Entities.Charm>();
                    cfg.CreateMap<Models.CharmUpdateDto, Entities.Charm>();
                    cfg.CreateMap<Entities.Charm, Models.CharmUpdateDto>();
                    cfg.CreateMap<Entities.Keyword, Models.KeywordDto>();
                    cfg.CreateMap<Models.KeywordCreationDto, Entities.Keyword>();
                    cfg.CreateMap<Entities.Duration, Models.DurationDto>();
                    cfg.CreateMap<Models.SaveDurationDto, Entities.Duration>();
                    cfg.CreateMap<Models.DurationForUpdate, Entities.Duration>();
                    cfg.CreateMap<Entities.Duration, Models.DurationForUpdate>();
                });
            app.UseMvc(config =>
            {
                config.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
