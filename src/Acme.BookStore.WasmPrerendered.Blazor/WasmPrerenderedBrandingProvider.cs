using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Acme.BookStore.WasmPrerendered.Blazor;

[Dependency(ReplaceServices = true)]
public class WasmPrerenderedBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "WasmPrerendered";
}
