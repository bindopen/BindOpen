﻿using BindOpen.Data.Stores;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Samples.SampleA.Services;
using Samples.SampleA.Settings;
using System.Threading.Tasks;

namespace Samples.SampleA
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await new HostBuilder()
               .ConfigureServices(services =>
               {
                   services
                    .AddBindOpen<TestAppSettings>(
                        (options) => options
                            .SetRootFolder(q => q.HostSettings.Environment != "Development", @".\..\..\..")
                            .SetRootFolder(q => q.HostSettings.Environment == "Development", @".\")
                            .AddDataStore(s => s
                                .RegisterDatasources(m => m.AddFromConfiguration(options)))
                            .SetHostSettingsFile(false)
                            .SetHostSettings(p => p.WithAppConfigFileRequired(false))
                            .ThrowExceptionOnStartFailure()
                    )
                    //.AddBdoConnectedService<IBdoDbService, TestDbRepository>(
                    //    ServiceLifetime.Transient,
                    //    host => new TestDbRepository(host.GetModel<MyDbModel>(), host.CreateMSSqlServerConnector("mlmlm")))
                    .AddHostedService<TestService>();
               })
               .RunConsoleAsync().ConfigureAwait(false);
        }
    }
}