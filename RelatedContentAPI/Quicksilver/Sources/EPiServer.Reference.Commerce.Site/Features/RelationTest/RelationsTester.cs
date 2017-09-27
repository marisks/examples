using System.Linq;
using EPiServer.Commerce.Catalog.Linking;
using EPiServer.Core;

namespace EPiServer.Reference.Commerce.Site.Features.RelationTest
{
    public class RelationsTester
    {
        private readonly IRelationRepository _relationRepository;

        public RelationsTester(IRelationRepository relationRepository)
        {
            _relationRepository = relationRepository;
        }

        public void IReadRelation()
        {
            ContentReference parentLink = null;
            var children = _relationRepository.GetChildren<NodeEntryRelation>(parentLink);
            ContentReference childLink = null;
            var parents =
                _relationRepository
                    .GetParents<NodeEntryRelation>(childLink)
                    .Where(x => !x.IsPrimary);
            _relationRepository.RemoveRelations(parents);
        }
    }
}