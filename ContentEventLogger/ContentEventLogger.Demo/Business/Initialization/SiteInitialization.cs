using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace ContentEventLogger.Demo.Business.Initialization
{
    /// <summary>
    /// Module for registering filters which will be applied to controller actions.
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class SiteInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            context.UseContentEventLogger(
                x => x
                    .SubscribeTo(ContentEvent.Created)
                    .SubscribeTo(ContentEvent.Published));
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
