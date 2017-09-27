﻿using EPiServer.Core;
using EPiServer.Recommendations.Tracking.Data;
using EPiServer.Reference.Commerce.Site.Features.Product.ViewModels;
using EPiServer.Reference.Commerce.Site.Features.Recommendations.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.Controllers;
using EPiServer.Reference.Commerce.Site.Features.Search.Pages;
using EPiServer.Reference.Commerce.Site.Features.Search.Services;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModelFactories;
using EPiServer.Reference.Commerce.Site.Features.Search.ViewModels;
using FluentAssertions;
using Mediachase.Commerce;
using Mediachase.Commerce.Catalog;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;

namespace EPiServer.Reference.Commerce.Site.Tests.Features.Search.Controllers
{
    public class SearchControllerTests
    {
        [Fact]
        public void Index_ShouldReturnViewModel()
        {
            var result = ((ViewResult)_subject.Index(new SearchPage(), new FilterOptionViewModel())).Model as SearchViewModel<SearchPage>;
            result.ShouldBeEquivalentTo(_searchViewModel);
        }

        [Fact]
        public void Index_WhenBrowsingFirstResultPage_ShouldSendSearchTracking()
        {
            _subject.Index(new SearchPage(), new FilterOptionViewModel() { Page = 1 });
            _recommendationServiceMock.Verify(
               x => x.SendSearchTracking(
                       It.IsAny<HttpContextBase>(),
                       It.IsAny<string>(),
                       It.Is<IEnumerable<string>>(y => y.Single() == _searchViewModel.ProductViewModels.Single().Code)
                       ),
               Times.Once);
        }

        [Fact]
        public void Index_WhenBrowsingFirstResultPage_ShouldSetHttpContextVariable()
        {
            _subject.Index(new SearchPage(), new FilterOptionViewModel() { Page = 1 });
            Assert.Equal(1, _httpContextMock.Object.Items.Count);
            Assert.True(_httpContextMock.Object.Items.Contains(SearchTrackingData.TotalSearchResultsKey));
        }

        [Fact]
        public void Index_WhenBrowsingNextResultPage_ShouldNotSendSearchTracking()
        {
            _subject.Index(new SearchPage(), new FilterOptionViewModel() { Page = 2 });
            _recommendationServiceMock.Verify(
               x => x.SendSearchTracking(
                       It.IsAny<HttpContextBase>(),
                       It.IsAny<string>(),
                       It.IsAny<IEnumerable<string>>()
                       ),
               Times.Never);
        }

        [Fact]
        public void Index_WhenUsingFacetFiltersInFirstResultPage_ShouldNotSendSearchTracking()
        {
            _httpRequestMock.SetupGet(x => x.HttpMethod).Returns("POST");

            _subject.Index(new SearchPage(), new FilterOptionViewModel() { Page = 1 });
            _recommendationServiceMock.Verify(
               x => x.SendSearchTracking(
                       It.IsAny<HttpContextBase>(),
                       It.IsAny<string>(),
                       It.IsAny<IEnumerable<string>>()
                       ),
               Times.Never);
        }

        [Fact]
        public void QuickSearch_WhenSearch_ShouldReturnIEnumerableProductViewModel()
        {
            var result = ((ViewResult)_subject.QuickSearch("test")).Model as ProductTileViewModel[];
            var expectedResult = new [] 
            { 
                new ProductTileViewModel
                {
                    DisplayName = "Test",
                    PlacedPrice =  new Money(10, Currency.USD),
                    DiscountedPrice = new Money(10, Currency.USD)
                }
            };
            result.ShouldAllBeEquivalentTo(expectedResult);
        }

        [Fact]
        public void QuickSearch_WhenSearch_ShouldNotSendSearchTracking()
        {
            var result = ((ViewResult)_subject.QuickSearch("test")).Model as ProductTileViewModel[];           
            _recommendationServiceMock.Verify(
                x => x.SendSearchTracking(
                    It.IsAny<HttpContextBase>(),
                    It.IsAny<string>(),
                    It.IsAny<IEnumerable<string>>()),
                Times.Never);
        }

        SearchController _subject;
        Mock<SearchViewModelFactory> _searchViewModelFactoryMock;
        Mock<ISearchService> _searchServiceMock;
        Mock<ReferenceConverter> _referenceConverterMock;
        Mock<IRecommendationService> _recommendationServiceMock;
        SearchViewModel<SearchPage> _searchViewModel;
        Mock<HttpRequestBase> _httpRequestMock;
        Mock<HttpContextBase> _httpContextMock;
        Dictionary<string, object> _items;

        public SearchControllerTests()
        {
            var contentLink = new ContentReference();
            _items = new Dictionary<string, object>();
            _searchServiceMock = new Mock<ISearchService>();
            _searchViewModelFactoryMock = new Mock<SearchViewModelFactory>(null,null);
            _referenceConverterMock = new Mock<ReferenceConverter>(null, null);
            _recommendationServiceMock = new Mock<IRecommendationService>();
            _searchViewModel = new SearchViewModel<SearchPage>
            {
                ErrorMessage = "Success",
                ProductViewModels = new[] { new ProductTileViewModel() { Code = "12345", PlacedPrice = new Money(10, Currency.USD) } } 
            };

            _searchViewModelFactoryMock.Setup(
                x => x.Create(
                    It.IsAny<SearchPage>(), It.IsAny<FilterOptionViewModel>()))
                .Returns((SearchPage content, FilterOptionViewModel model) => _searchViewModel);

            _searchServiceMock.Setup(x => x.QuickSearch("test"))
                .Returns(new[] 
                { 
                    new ProductTileViewModel
                    {
                        DisplayName = "Test",
                        PlacedPrice =  new Money(10, Currency.USD),
                        DiscountedPrice = new Money(10, Currency.USD)
                    }
                });

            _recommendationServiceMock
                .Setup(x => x.SendSearchTracking(It.IsAny<HttpContextBase>(), It.IsAny<string>(), It.IsAny<IEnumerable<string>>()))
                .Returns(() => GetTestResponseData());

            _referenceConverterMock
                .Setup(x => x.GetContentLink(It.IsAny<string>()))
                .Returns(contentLink);

            _subject = new SearchController(
                _searchViewModelFactoryMock.Object, 
                _searchServiceMock.Object, 
                _recommendationServiceMock.Object,
                _referenceConverterMock.Object);

            _httpRequestMock = new Mock<HttpRequestBase>();
            _httpRequestMock.SetupGet(x => x.HttpMethod).Returns("GET");

            _httpContextMock = new Mock<HttpContextBase>();
            _httpContextMock.Setup(x => x.Items).Returns(_items);
            _httpContextMock.SetupGet(x => x.Request).Returns(_httpRequestMock.Object);
            _subject.ControllerContext = new ControllerContext(_httpContextMock.Object, new RouteData(), _subject);
        }

        private TrackingResponseData GetTestResponseData()
        {
            return new TrackingResponseData
            {
                 Status = "OK",
                 SmartRecs = new[] 
                 {
                     new RecommendationsResponseData
                     {
                         Recs = new []
                         {
                             new RecommendationData
                             {
                                 RefCode = "POPULAR-PRODUCT-CODE"
                             }
                         }
                     }
                 }
            };
        }
    }
}