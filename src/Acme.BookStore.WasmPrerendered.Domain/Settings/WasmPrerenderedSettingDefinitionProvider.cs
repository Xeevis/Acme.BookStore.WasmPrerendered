using Volo.Abp.Settings;

namespace Acme.BookStore.WasmPrerendered.Settings;

public class WasmPrerenderedSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WasmPrerenderedSettings.MySetting1));
    }
}
