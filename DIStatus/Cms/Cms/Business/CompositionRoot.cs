using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiServer.Web.Mvc;
using StructureMap;

namespace Cms.Business
{
    public class CompositionRoot : ControllerTypeControllerFactory
    {
        private readonly IContainer _container;

        public CompositionRoot(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));
            _container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                throw new HttpException(
                    404,
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "No controller found for path: {0}",
                        requestContext.HttpContext.Request.Path as object));
            if (!typeof(IController).IsAssignableFrom(controllerType))
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "Type {0} does not subclass controller base",
                        (object) controllerType),
                    nameof(controllerType));

            return (IController) _container.GetInstance(controllerType);
        }
    }
}