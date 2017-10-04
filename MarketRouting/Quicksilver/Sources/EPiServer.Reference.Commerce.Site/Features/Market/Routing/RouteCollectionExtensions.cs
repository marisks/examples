using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using EPiServer.Configuration;
using EPiServer.Core;
using EPiServer.Globalization.Internal;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Mvc;
using EPiServer.Web.Mvc.Internal;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Internal;
using EPiServer.Web.Routing.Segments;
using EPiServer.Web.Routing.Segments.Internal;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Routing
{
    public static class RouteCollectionExtensions
    {
        // ReSharper disable UnassignedGetOnlyAutoProperty
        private static Injected<IMarketService> InjectedMarketService { get; }
        private static IMarketService MarketService => InjectedMarketService.Service;

        private static Injected<ICurrentMarket> InjectedCurrentMarket { get; }
        private static ICurrentMarket CurrentMarket => InjectedCurrentMarket.Service;
        // ReSharper restore UnassignedGetOnlyAutoProperty

        public static void MapMarketSegment(this RouteCollection routes)
        {
            var segment = new MarketSegment(MarketService, CurrentMarket);
            var segmentMappings = new Dictionary<string, ISegment> { { MarketSegment.SegmentName, segment } };
            var parameters = new MapContentRouteParameters
            {
                Direction = SupportedDirection.Both,
                SegmentMappings = segmentMappings
            };
            routes.InsertAndMapContentRoute(
                index: routes.IndexOf("pages"),
                name: MarketSegment.SegmentName,
                url: "{language}/{market}/{node}/{partial}/{action}",
                defaults: new { action = "index" },
                parameters: parameters);
        }

        public static IContentRoute InsertAndMapContentRoute(
            this RouteCollection routes, int index, string name, string url, object defaults, MapContentRouteParameters parameters)
        {
            parameters.ServiceLocator = parameters.ServiceLocator ?? ServiceLocator.Current;
            var routeHandler = parameters.RouteHandler;
            if (parameters.ServiceLocator.AssignNullService(ref routeHandler))
                parameters.RouteHandler = routeHandler;
            var urlSegmentRouter = parameters.UrlSegmentRouter;
            if (parameters.ServiceLocator.AssignNullService(ref urlSegmentRouter))
                parameters.UrlSegmentRouter = urlSegmentRouter;
            var routeParser = parameters.RouteParser;
            if (parameters.ServiceLocator.AssignNullService(ref routeParser))
                parameters.RouteParser = routeParser;
            var branchRepository = parameters.LanguageBranchRepository;
            if (parameters.ServiceLocator.AssignNullService(ref branchRepository))
                parameters.LanguageBranchRepository = branchRepository;
            var viewRegistrator = parameters.ViewRegistrator;
            if (parameters.ServiceLocator.AssignNullService(ref viewRegistrator))
                parameters.ViewRegistrator = viewRegistrator;
            var contentLoader = parameters.ContentLoader;
            if (parameters.ServiceLocator.AssignNullService(ref contentLoader))
                parameters.ContentLoader = contentLoader;
            var partialRouteHandler = parameters.PartialRouteHandler;
            if (parameters.ServiceLocator.AssignNullService(ref partialRouteHandler))
                parameters.PartialRouteHandler = partialRouteHandler;
            var templateResolver = parameters.TemplateResolver;
            if (parameters.ServiceLocator.AssignNullService(ref templateResolver))
                parameters.TemplateResolver = templateResolver;
            var permanentLinkMapper = parameters.PermanentLinkMapper;
            if (parameters.ServiceLocator.AssignNullService(ref permanentLinkMapper))
                parameters.PermanentLinkMapper = permanentLinkMapper;
            var urlResolver = parameters.UrlResolver;
            if (parameters.ServiceLocator.AssignNullService(ref urlResolver))
                parameters.UrlResolver = urlResolver;
            var versionRepository = parameters.ContentVersionRepository;
            if (parameters.ServiceLocator.AssignNullService(ref versionRepository))
                parameters.ContentVersionRepository = versionRepository;
            var updateCurrentLanguage = parameters.UpdateCurrentLanguage;
            if (parameters.ServiceLocator.AssignNullService(ref updateCurrentLanguage))
                parameters.UpdateCurrentLanguage = updateCurrentLanguage;
            if (parameters.BasePathResolver == null)
            {
                var instance = parameters.ServiceLocator.GetInstance<IBasePathResolver>();
                parameters.BasePathResolver = instance.Resolve;
            }
            if (parameters.StrictLanguageRoutingResolver == null)
                parameters.StrictLanguageRoutingResolver = () => Settings.Instance.StrictLanguageRouting;
            var dictionary = new Dictionary<string, ISegment>
            {
                {
                    RoutingConstants.NodeKey,
                    new NodeSegment(
                        RoutingConstants.NodeKey,
                        UrlRewriteProvider.FriendlyUrlExtension,
                        urlSegmentRouter,
                        parameters.ContentLoader,
                        parameters.UrlResolver,
                        ServiceLocator.Current.GetInstance<IContentLanguageSettingsHandler>())
                },
                {
                    "partial",
                    new PartialSegment("partial", parameters.ContentLoader, parameters.PartialRouteHandler)
                },
                {
                    RoutingConstants.LanguageKey,
                    new LanguageSegment(
                        RoutingConstants.LanguageKey,
                        new LanguageSegmentMatcher(
                            parameters.LanguageBranchRepository,
                            ServiceLocator.Current.GetInstance<HostLanguageResolver>()),
                        ServiceLocator.Current.GetInstance<HostLanguageResolver>(),
                        new VirtualPathHostResolver(
                            parameters.BasePathResolver,
                            ServiceLocator.Current.GetInstance<ServiceAccessor<SiteDefinition>>(),
                            ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>()),
                        ServiceLocator.Current.GetInstance<HostNameResolver>())
                }
            };
            if (parameters.SegmentMappings != null)
            {
                foreach (var segmentMapping in parameters.SegmentMappings)
                    dictionary[segmentMapping.Key] = segmentMapping.Value;
            }
            var constraints = new RouteValueDictionary(parameters.Constraints);
            if (!constraints.ContainsKey(RoutingConstants.ActionKey))
            {
                var controllerTypeMap = parameters.ControllerTypeMap;
                parameters.ServiceLocator.AssignNullService(ref controllerTypeMap);
                parameters.ActionHandlers = parameters.ActionHandlers ?? parameters.ServiceLocator.GetAllInstances<IUnknownActionHandler>().ToArray();
                constraints[RoutingConstants.ActionKey] = new ExistingActionRouteConstraint(parameters);
            }
            var urlSegments = routeParser.Parse(url, dictionary);
            var defaultContentRoute1 = new DefaultContentRoute(
                routeHandler,
                urlSegments,
                new RouteValueDictionary(defaults),
                constraints,
                parameters.Direction,
                parameters.BasePathResolver,
                parameters.ViewRegistrator,
                parameters.UpdateCurrentLanguage,
                parameters.ServiceLocator.GetInstance<RouteRedirector>(),
                new VirtualPathHostResolver(
                    parameters.BasePathResolver,
                    ServiceLocator.Current.GetInstance<ServiceAccessor<SiteDefinition>>(),
                    ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>()),
                ServiceLocator.Current.GetInstance<IContentRouteEventsRaiser>(),
                ServiceLocator.Current.GetInstance<ServiceAccessor<RoutingOptions>>())
            { Name = name };
            var languageRoutingResolver = parameters.StrictLanguageRoutingResolver;
            defaultContentRoute1.StrictLanguageRoutingResolver = languageRoutingResolver;
            if (routes[name] != null)
                routes.Remove(routes[name]);
            routes.Insert(index, defaultContentRoute1);
            return defaultContentRoute1;
        }

        public static int IndexOf(this RouteCollection routes, string name)
        {
            var defaultRoute = routes
                .Select(r => r as DefaultContentRoute)
                .Where(x => x != null)
                .First(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
            return routes.IndexOf(defaultRoute);
        }
    }
}