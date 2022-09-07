using Volo.Abp.Modularity;

namespace Acme.BookStore.WasmPrerendered;

[DependsOn(
    typeof(WasmPrerenderedApplicationModule),
    typeof(WasmPrerenderedDomainTestModule)
    )]
public class WasmPrerenderedApplicationTestModule : AbpModule
{

}
