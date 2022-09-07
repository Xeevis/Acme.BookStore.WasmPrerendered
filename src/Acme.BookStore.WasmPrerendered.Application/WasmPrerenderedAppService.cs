using System;
using System.Collections.Generic;
using System.Text;
using Acme.BookStore.WasmPrerendered.Localization;
using Volo.Abp.Application.Services;

namespace Acme.BookStore.WasmPrerendered;

/* Inherit your application services from this class.
 */
public abstract class WasmPrerenderedAppService : ApplicationService
{
    protected WasmPrerenderedAppService()
    {
        LocalizationResource = typeof(WasmPrerenderedResource);
    }
}
