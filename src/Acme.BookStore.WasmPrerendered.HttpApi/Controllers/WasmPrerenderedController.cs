using Acme.BookStore.WasmPrerendered.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Acme.BookStore.WasmPrerendered.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WasmPrerenderedController : AbpControllerBase
{
    protected WasmPrerenderedController()
    {
        LocalizationResource = typeof(WasmPrerenderedResource);
    }
}
