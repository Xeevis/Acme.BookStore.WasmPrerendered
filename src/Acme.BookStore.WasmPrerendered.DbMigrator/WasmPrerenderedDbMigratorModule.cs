using Acme.BookStore.WasmPrerendered.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Acme.BookStore.WasmPrerendered.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WasmPrerenderedMongoDbModule),
    typeof(WasmPrerenderedApplicationContractsModule)
    )]
public class WasmPrerenderedDbMigratorModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options => options.IsJobExecutionEnabled = false);
    }
}
