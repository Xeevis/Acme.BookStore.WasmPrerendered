# Prerendering your Blazor Wasm application with ABP 6.x

## Introduction

In this article, we will demonstrate how to create basic hosted monolith **Blazor WebAssembly application** that will be prerendered by the server. 

Prerendering has following advantages:

* 💚 Instant initial load of page content while WebAssembly application downloads and hydrates in the background
* 💚 Improved scores in Chromium Lighthouse
* 💚 Static markup analysis by search engines (SEO) and tools that don't render WebAssembly apps

Prerendering also has some drawbacks:

* 🛑 You can no longer host your app on static file hosting
* 🛑 Different approach is required, no JS Interop calls in `OnInitialized()`, may be a problem with 3rd party components that use JS, but didn't account for non-browser rendering.
* 🛑 Double rendering, once on server, once on client. Takes extra steps to preserve state in order to avoid duplicate calls and content swap.
* 🛑 Server rendering doesn't support authentication, so it works only on public pages. For best experience, parts of content specific to unauthenticated/authenticated state should use placeholders until it's decided which one it will be.

> ⚠ **Warning: This isn't production ready solution, it's a rudimentary starter guide on how to make prerendering work. Expect  problems.**

### Source Code

Source code for the final application is [available on GitHub](https://github.com/Xeevis/Acme.BookStore.WasmPrerendered).

### Screenshot
![image](https://user-images.githubusercontent.com/5835044/188908662-e42d9430-4a1b-41b4-9552-514ada10de89.png)


## Requirements

The following tools are needed to be able to run the solution.

* .NET 6.0 SDK
* ABP CLI
* Compatible database engine

## Step by step guide

### Preparing the solution

1. Use the following ABP CLI command to create Blazor WebAssembly app:

```ps
abp new Acme.BookStore.WasmPrerendered -u blazor -d mongodb --create-solution-folder --theme basic --preview
```
```ps
cd Acme.BookStore.WasmPrerendered
```

2. Reference `Blazor` project in project `HttpApi.Host`
```ps
dotnet add src/Acme.BookStore.WasmPrerendered.HttpApi.Host reference src/Acme.BookStore.WasmPrerendered.Blazor
```

> ℹ For simplicity we are reusing `HttpApi.Host` project to have just a single point of entry to run, but nothing stops you from creating separate project to decouple client (+ host) & server API like it's done by default.

3. While this step isn't absolutely necessary, when using IDE like **Visual Studio Code** it's better to build solution for IntelliSense to work correctly.

```ps
dotnet build
# Open solution in Visual Studio Code
code .
```

### Adding the prerendering

4. Move and rename Blazor's index.html file to our host (`Pages` folder needs to be created first)
```diff
- src/Acme.BookStore.WasmPrerendered.Blazor/wwwroot/index.html
+ src/Acme.BookStore.WasmPrerendered.HttpApi.Host/Pages/_Layout.cshtml
```

5. On top of this `_Layout.cshtml` file prepend
```diff
+ @using Microsoft.AspNetCore.Components.Web
+ @addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
```

6. Inside `<head>` element we can replace static tag `<title>` with new dynamic component that will prerender title and metatags coming from pages
```diff
- <title>Acme.BookStore.WasmPrerendered.Blazor</title>
+ <component type="typeof(HeadOutlet)" render-mode="WebAssemblyPrerendered" />
```

7. Last thing to do in this file is to replace loading animation which won't be needed anymore
```diff
- <div id="ApplicationContainer">...</div>
+ @RenderBody()
```

8. Next to `_Layout.cshtml` create new Razor page `_Host.cshtml` that will use _Layout and prerender the application
```cshtml
@page
@using Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic
@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers
@{
    Layout = "_Layout";
}

@if (HttpContext.Request.Path.StartsWithSegments("/authentication"))
{
    <component type="typeof(App)" render-mode="WebAssembly" />
}
else
{
    <component type="typeof(App)" render-mode="WebAssemblyPrerendered" />
    <persist-component-state />
}
```

> Tag helper `<persist-component-state />` allows us to persist state from first render on server to second render in client preventing duplicate calls and content swap. This doesn't work automatically and has to be implemented for every component separately.

9. Open `src\Acme.BookStore.WasmPrerendered.HttpApi.Host\WasmPrerenderedHttpApiHostModule.cs` and to the botom of `OnApplicationInitialization` append

```diff
    app.UseAbpSerilogEnrichers();
    app.UseConfiguredEndpoints();
+   ((WebApplication)app).MapFallbackToPage("/_Host");
}
```

10. To use `_Host` instead of swagger delete the controller
```diff
- src\Acme.BookStore.WasmPrerendered.HttpApi.Host\Controllers\HomeController.cs
```

11. Since we are using `HttpApi.Host` project to serve our Blazor application we are interiting its port. To avoid problems with authentication in `src\Acme.BookStore.WasmPrerendered.DbMigrator\appsettings.json` change `WasmPrerendered_Blazor` RootUrl to match `WasmPrerendered_Swagger`.

```diff
"WasmPrerendered_Blazor": {
  "ClientId": "WasmPrerendered_Blazor",
- "RootUrl": "https://localhost:44351"
+ "RootUrl": "https://localhost:44314"
},
"WasmPrerendered_Swagger": {
  "ClientId": "WasmPrerendered_Swagger",
  "RootUrl": "https://localhost:44314"
}
```

12. We can now migrate our database, run `Acme.BookStore.WasmPrerendered.DbMigrator` project

13. Now is a good time to run our project to see if everything compiles and host is actually at least trying to prerender our WebAssembly application, don't worry error is to be expected.

```ps
dotnet run --project src\Acme.BookStore.WasmPrerendered.HttpApi.Host
```

> ⚠ Please take note that you should always run the `HttpApi.Host` project and never the `Blazor` project!
### Fixing the errors

* Right off the bat we should get DI exception
> There is no registered service of type 'Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider'.

Let's fix that, in file `WasmPrerenderedHttpApiHostModule.cs` under `ConfigureServices` method we need to append

```diff
+ context.Services.AddScoped<Microsoft.AspNetCore.Components.Authorization.AuthenticationStateProvider, Microsoft.AspNetCore.Components.Server.ServerAuthenticationStateProvider>();
```

14. Next in line is router problem
> The Router component requires a value for the parameter AppAssembly.

Same file, same method as in previous step
```diff
+ Configure<Volo.Abp.AspNetCore.Components.Web.Theming.Routing.AbpRouterOptions>(options => options.AppAssembly = typeof(Blazor.WasmPrerenderedBlazorModule).Assembly);
```

15. Now it's another DI exception
> There is no registered service of type 'Volo.Abp.AspNetCore.Components.Web.Theming.Toolbars.IToolbarManager'.


We can solve this with dependency 

```diff
[DependsOn(
    (...)
    typeof(AbpSwashbuckleModule),
+   typeof(Volo.Abp.AspNetCore.Components.Web.Theming.AbpAspNetCoreComponentsWebThemingModule)
)]
public class WasmPrerenderedHttpApiHostModule : AbpModule
```

16. Again ... DI exception
> InvalidOperationException: Cannot provide a value for property 'ClassProvider' on type 'Blazorise.Badge'. There is no registered service of type 'Blazorise.IClassProvider'.

This is another one for file `WasmPrerenderedHttpApiHostModule.cs` under `ConfigureServices`

```cs
context.Services
  .AddBootstrap5Providers()
  .AddFontAwesomeIcons();
```

Don't forget the usings
```cs
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
```

> ⚠ Please note this simplification is duplicating logic from Blazor app, you should definitely create shared code both `Blazor` and `HttpApi.Host` will use, otherwise you'll have to make changes in 2 places, asking for trouble.

* Hooray! We should be able to prerender some markup now. But if you run the app now, you'll get flooded by integrity errors and app won't hydrate. It's still far from over.

---

Before continuing we can do a simple trick that will tell us if we are looking at prerendered markup or at fully loaded WebAssembly app without devtools.

For `HttpApi.Host` we can change `WasmPrerenderedBrandingProvider.cs` to include message about loading
```cs
public override string AppName => "🔁 Wasm is loading ..";
```

While for `Blazor` we can change `WasmPrerenderedBrandingProvider.cs` to include message that it finished

```cs
public override string AppName => "✅ Wasm has loaded!";
```

---

17. To fix integrity errors we need a package `Microsoft.AspNetCore.Components.WebAssembly.Server` on our `HttpApi.Host` project

```ps
dotnet add src/Acme.BookStore.WasmPrerendered.HttpApi.Host package Microsoft.AspNetCore.Components.WebAssembly.Server
```

this in turn lets us append new line in `WasmPrerenderedHttpApiHostModule.cs`

```diff
app.UseCorrelationId();
+ app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
```

18. Next error I must admin I'm not sure why is happening
> 'Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic.App' could not be found in the assembly 'Volo.Abp.AspNetCore.Components.Web.BasicTheme'. This is likely a result of trimming (tree shaking).)

I could swear it's there, until someone figures what is happening we can copy this `App.razor` to our `Blazor` project, most people will probably override it anyway.
```razor
@using Microsoft.Extensions.Options
@using Volo.Abp.AspNetCore.Components.Web.Theming.Routing
@using Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic
@inject IOptions<AbpRouterOptions> RouterOptions

<CascadingAuthenticationState>
    <Router AppAssembly="RouterOptions.Value.AppAssembly"
            AdditionalAssemblies="RouterOptions.Value.AdditionalAssemblies">
        <Found Context="routeData">
            <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
                <NotAuthorized>
                    @if (!context.User.Identity.IsAuthenticated)
                    {
                        <RedirectToLogin />
                    }
                    else
                    {
                        <p>You are not authorized to access this resource.</p>
                    }
                </NotAuthorized>
            </AuthorizeRouteView>
        </Found>
        <NotFound>
            <LayoutView Layout="@typeof(MainLayout)">
                <p>Sorry, there's nothing at this address.</p>
            </LayoutView>
        </NotFound>
    </Router>
</CascadingAuthenticationState>
```
In `HttpApi.Host/Pages/_Host.cshtml` replace the using to use our custom `App.razor`
```diff
- @using Volo.Abp.AspNetCore.Components.Web.BasicTheme.Themes.Basic
+ @using Acme.BookStore.WasmPrerendered.Blazor
```

19. Wasm is loading now! But hey there's still one last error to solve

In `Blazor/WasmPrerenderedBlazorModule.cs` we can safely remove
```diff
- builder.RootComponents.Add<App>("#ApplicationContainer");
```

### ✅ In this state prerendering should be pretty stable with no errors. You can even authenticate! Try looking at static page source (ctrl + u).

> Sadly this isn't end of the road, toolbars don't preprerender correctly, you can't manage your account and when you switch language, prerendered and client language will be different.

## TODO: To be continued ...

Do you find this article useful? Would you like to see more like it? Coffee helps me stay awake at night to write for you. ❤ 

<a href="https://ko-fi.com/xeevis"><img src="https://user-images.githubusercontent.com/5835044/188901166-972fef50-2aea-4720-86b4-01eb71ad9079.png" width="256"/></a>
