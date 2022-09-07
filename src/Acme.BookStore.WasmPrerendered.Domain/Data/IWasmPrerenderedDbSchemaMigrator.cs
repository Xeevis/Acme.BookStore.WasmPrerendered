using System.Threading.Tasks;

namespace Acme.BookStore.WasmPrerendered.Data;

public interface IWasmPrerenderedDbSchemaMigrator
{
    Task MigrateAsync();
}
