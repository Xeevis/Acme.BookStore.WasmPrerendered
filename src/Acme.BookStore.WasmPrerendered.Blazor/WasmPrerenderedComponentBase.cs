using Acme.BookStore.WasmPrerendered.Localization;
using Volo.Abp.AspNetCore.Components;

namespace Acme.BookStore.WasmPrerendered.Blazor;

public abstract class WasmPrerenderedComponentBase : AbpComponentBase
{
    protected WasmPrerenderedComponentBase()
    {
        LocalizationResource = typeof(WasmPrerenderedResource);
    }
}
