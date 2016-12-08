﻿using Autofac;
using Autofac.Integration.Mvc;
using CacheManager.Core;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ViewWorld.Core;

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
            // Register MVC controllers.
            var builder = new ContainerBuilder();
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
                .WithEndpoint(Config.Cache_HostIP, Config.Cache_Port))
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
