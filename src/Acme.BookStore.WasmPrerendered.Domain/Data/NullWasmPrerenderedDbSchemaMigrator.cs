using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Acme.BookStore.WasmPrerendered.Data;

/* This is used if database provider does't define
 * IWasmPrerenderedDbSchemaMigrator implementation.
 */
public class NullWasmPrerenderedDbSchemaMigrator : IWasmPrerenderedDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
