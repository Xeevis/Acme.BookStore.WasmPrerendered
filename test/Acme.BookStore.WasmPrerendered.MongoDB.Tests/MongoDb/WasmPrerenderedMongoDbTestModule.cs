﻿using System;
using Volo.Abp.Data;
using Volo.Abp.Modularity;

namespace Acme.BookStore.WasmPrerendered.MongoDB;

[DependsOn(
    typeof(WasmPrerenderedTestBaseModule),
    typeof(WasmPrerenderedMongoDbModule)
    )]
public class WasmPrerenderedMongoDbTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var stringArray = WasmPrerenderedMongoDbFixture.ConnectionString.Split('?');
        var connectionString = stringArray[0].EnsureEndsWith('/') +
                                   "Db_" +
                               Guid.NewGuid().ToString("N") + "/?" + stringArray[1];

        Configure<AbpDbConnectionOptions>(options =>
        {
            options.ConnectionStrings.Default = connectionString;
        });
    }
}
