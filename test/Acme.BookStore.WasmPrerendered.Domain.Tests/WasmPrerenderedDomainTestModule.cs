using Acme.BookStore.WasmPrerendered.MongoDB;
using Volo.Abp.Modularity;

namespace Acme.BookStore.WasmPrerendered;

[DependsOn(
    typeof(WasmPrerenderedMongoDbTestModule)
    )]
public class WasmPrerenderedDomainTestModule : AbpModule
{

}
