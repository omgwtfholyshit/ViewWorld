using Autofac;
using Autofac.Integration.Mvc;
using CacheManager.Core;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ViewWorld.Core;
using ViewWorld.Core.Dal;
using Newtonsoft.Json;

namespace ViewWorld
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            #region RegisterConfigs
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            #endregion

            #region ioC config
            
            var builder = new ContainerBuilder();

            //Register Data Access Layer & Services
            var dal = Assembly.Load("ViewWorld.Core");
            var services = Assembly.Load("ViewWorld.Services");
            builder.RegisterAssemblyTypes(dal,services)
            .Where(t => t.Name.EndsWith("Repository")|| t.Name.EndsWith("Service"))
            .AsImplementedInterfaces().InstancePerLifetimeScope();

            builder.RegisterAssemblyTypes(typeof(ApplicationIdentityContext).Assembly)
                .AsSelf().InstancePerLifetimeScope();

            // Register MVC controllers.
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            // OPTIONAL: Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();
            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            //Register Cache
            var cacheConfig = ConfigurationBuilder.BuildConfiguration(settings =>
            {
                settings.WithSystemRuntimeCacheHandle("inprocess")
                .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(15))
                .And
                .WithRedisConfiguration("redisConnection", config => 
                config
                .WithConnectionTimeout(10000)
                .WithEndpoint(ConfigurationManager.AppSettings["cacheHostIP"],
                 Convert.ToInt32(ConfigurationManager.AppSettings["cachePort"])))
                .WithJsonSerializer()
                .WithMaxRetries(100)
                .WithRetryTimeout(50)
                .WithRedisCacheHandle("redisConnection", true)
                .WithExpiration(ExpirationMode.Sliding,TimeSpan.FromMinutes(15));
            });
            builder
                .RegisterGeneric(typeof(BaseCacheManager<>))
                .WithParameters(new[]
                {
                    new TypedParameter(typeof(string), "redisCache"),
                    new TypedParameter(typeof(CacheManagerConfiguration), cacheConfig)
                })
                .As(typeof(ICacheManager<>))
                .SingleInstance();
            // OPTIONAL: Enable action method parameter injection (RARE).
            //builder.InjectActionInvoker();
            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
           
            
            #endregion
        }
    }
}
