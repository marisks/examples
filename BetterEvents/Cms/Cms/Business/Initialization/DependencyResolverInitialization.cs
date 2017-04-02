using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Cms.Business.Rendering;
using Cms.Helpers;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Html;
using MediatR;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using StructureMap;
using StructureMap.Graph;

namespace Cms.Business.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class DependencyResolverInitialization : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(ConfigureContainer);

            // Comment out when using only controller factory 
            DependencyResolver.SetResolver(new StructureMapDependencyResolver(context.Container));

            GlobalConfiguration.Configure(config =>
            {
                config.DependencyResolver = new StructureMapDependencyResolver(context.Container);
                config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.LocalOnly;
                config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings();
                config.Formatters.XmlFormatter.UseXmlSerializer = true;
                config.MapHttpAttributeRoutes();
            });

            // Comment out when using resolver but register controller factory in the container
            // ControllerBuilder.Current.SetControllerFactory(new CompositionRoot(context.Container));
        }

        private static void ConfigureContainer(ConfigurationExpression container)
        {
            //Swap out the default ContentRenderer for our custom
            container.For<IContentRenderer>().Use<ErrorHandlingContentRenderer>();
            container.For<ContentAreaRenderer>().Use<AlloyContentAreaRenderer>();

            //Implementations for custom interfaces can be registered here.
            container.For<IOAuthAuthorizationServerProvider>().Use<IdentityAuthorizationProvider>();

            Func<IOwinContext> owinContextFunc = () => HttpContext.Current.GetOwinContext();
            container.For<ApplicationUserManager<ApplicationUser>>().Use(() => owinContextFunc().GetUserManager<ApplicationUserManager<ApplicationUser>>());
            container.For<IAuthenticationManager>().Use(() => owinContextFunc().Authentication);

            container.For<IControllerFactory>().Use<CompositionRoot>();

            container.Scan(
                scanner =>
                {
                    scanner.TheCallingAssembly();
                    scanner.AssemblyContainingType<IMediator>();
                    scanner.WithDefaultConventions();
                    scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncRequestHandler<,>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(ICancellableAsyncRequestHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IAsyncNotificationHandler<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(ICancellableAsyncNotificationHandler<>));
                });
            container.For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            container.For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
