using Acme.BookStore.WasmPrerendered.MongoDB;
using Xunit;

namespace Acme.BookStore.WasmPrerendered;

[CollectionDefinition(WasmPrerenderedTestConsts.CollectionDefinitionName)]
public class WasmPrerenderedDomainCollection : WasmPrerenderedMongoDbCollectionFixtureBase
{

}
