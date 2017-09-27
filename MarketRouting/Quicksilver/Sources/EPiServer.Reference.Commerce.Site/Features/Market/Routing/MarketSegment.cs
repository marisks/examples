using System;
using System.Web.Routing;
using EPiServer.Core;
using EPiServer.Web.Routing;
using EPiServer.Web.Routing.Segments;
using Mediachase.Commerce;
using Mediachase.Commerce.Markets;

namespace EPiServer.Reference.Commerce.Site.Features.Market.Routing
{
    public class MarketSegment : SegmentBase
    {
        private readonly IMarketService _marketService;
        private readonly ICurrentMarket _currentMarket;

        public const string SegmentName = "market";

        public MarketSegment(IMarketService marketService, ICurrentMarket currentMarket)
            : base(SegmentName)
        {
            if (marketService == null) throw new ArgumentNullException(nameof(marketService));
            if (currentMarket == null) throw new ArgumentNullException(nameof(currentMarket));
            _marketService = marketService;
            _currentMarket = currentMarket;
        }

        public override bool RouteDataMatch(SegmentContext context)
        {
            var segmentPair = context.GetNextValue(context.RemainingPath);
            var marketCode = segmentPair.Next;

            if (!string.IsNullOrEmpty(marketCode))
            {
                return ProcessSegment(context, segmentPair);
            }

            if (context.Defaults.ContainsKey(Name))
            {
                context.RouteData.Values[Name] = context.Defaults[Name];
                return true;
            }

            return false;
        }

        public override string GetVirtualPathSegment(RequestContext requestContext, RouteValueDictionary values)
        {
            var contentLink = requestContext.GetRouteValue("node", values) as ContentReference;
            if (ContentReference.IsNullOrEmpty(contentLink)) // Skips for non-content items.
            {
                return null;
            }

            var currentMarket = _currentMarket.GetCurrentMarket();
            return currentMarket.MarketId.Value.ToLower();
        }

        private bool ProcessSegment(SegmentContext context, SegmentPair segmentPair)
        {
            var marketCode = segmentPair.Next;
            var marketId = new MarketId(marketCode);
            var market = _marketService.GetMarket(marketId);
            if (market == null) return false;

            context.RouteData.Values[Name] = marketCode;
            context.RemainingPath = segmentPair.Remaining;

            _currentMarket.SetCurrentMarket(marketId);

            return true;
        }
    }
}