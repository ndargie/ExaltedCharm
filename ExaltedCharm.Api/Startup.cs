using System.Linq;
using ExaltedCharm.Api.Entities;
using ExaltedCharm.Api.Extensions;
using ExaltedCharm.Api.Models;
using ExaltedCharm.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

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
            services.AddMvc(setupAction => { setupAction.ReturnHttpNotAcceptable = true; })
                .AddMvcOptions(o =>
                {
                    o.OutputFormatters
                        .Add(new XmlDataContractSerializerOutputFormatter());
                    o.InputFormatters.Add(new XmlDataContractSerializerInputFormatter());
                    var jsonOutputFormatter = o.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();
                    jsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.exalted.hateoas+json");
                }).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
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
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper, UrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>().ActionContext;
                return new UrlHelper(actionContext);
            });
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
            services.AddTransient<ITypeHelperService, TypeHelperService>();
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CharmContext charmTypeContext, ILogger<Startup> logger)
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
                        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (exceptionHandlerFeature != null)
                        {
                            logger.LogError(500, exceptionHandlerFeature.Error, exceptionHandlerFeature.Error.Message);
                        }
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
                    cfg.CreateMap<Models.CharmTypeCreationDto, Entities.CharmType>();
                    cfg.CreateMap<Models.CharmUpdateDto, Entities.CharmType>();
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
