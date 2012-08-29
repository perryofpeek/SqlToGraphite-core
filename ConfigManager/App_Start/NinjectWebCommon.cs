using System.Diagnostics;
using System.Web.Mvc;

using Ninject.Web.Mvc;

using SqlToGraphite;
using SqlToGraphite.Clients;
using SqlToGraphite.Conf;

using log4net;

[assembly: WebActivator.PreApplicationStartMethod(typeof(ConfigManager.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(ConfigManager.App_Start.NinjectWebCommon), "Stop")]

namespace ConfigManager.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var path = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();

            string uri = string.Format("file://{0}/Config.xml", path);
            const string username = "";
            const string password = "";
            var cacheLength = new TimeSpan(0, 0, 0, 1);

            // Create Ninject DI kernel                
            kernel.Bind<ILog>().ToMethod(context => LogManager.GetLogger(context.Request.Target.Member.ReflectedType));
            // Register services with Ninject DI Container
            kernel.Bind<IConfigReader>().To<ConfigHttpReader>().WithConstructorArgument("uri", uri).WithConstructorArgument("username", username).WithConstructorArgument("password", password);
            kernel.Bind<IKnownGraphiteClients>().To<KnownGraphiteClients>();
            kernel.Bind<ISleep>().To<Sleeper>();


            var filename = string.Format(@"{0}/Config.xml", path);

            kernel.Bind<IConfigWriter>().To<ConfigFileWriter>().WithConstructorArgument("fileName", filename);
            kernel.Bind<IConfigPersister>().To<ConfigPersister>();
            kernel.Bind<ICache>().To<Cache>().WithConstructorArgument("cacheLength", cacheLength);
            kernel.Bind<IConfigRepository>().To<ConfigRepository>().WithConstructorArgument("errorReadingConfigSleepTime", 1000);

            // Tell ASP.NET MVC 3 to use our Ninject DI Container
            DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel));
        }
    }
}
