// See https://aka.ms/new-console-template for more information


using System.Diagnostics;
using Huffman;
using Huffman.Core.Services;
using Huffman.Core.Services.Deserialization;
using Huffman.Core.Services.Generation;
using Huffman.Core.Services.Serialization;
using Huffman.Infra;
using Huffman.Infra.Services;
using Huffman.Infra.Services.Deserialization;
using Huffman.Infra.Services.Generation;
using Huffman.Infra.Services.Serialization;
using Huffman.Services.Serde;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args).ConfigureServices(services =>
{
    services.AddSingleton<IDataSerializationService, DataSerializationService>();
    services.AddSingleton<ITreeSerializationService, TreeSerializationService>();
    services.AddSingleton<IDataDeserializationService, DataDeserializationService>();
    services.AddSingleton<ITreeDeserializationService, TreeDeserializationService>();
    services.AddSingleton<IHuffmanSerializationService, HuffmanSerializationService>();
    services.AddSingleton<IHuffmanDeserializationService, HuffmanDeserializationService>();
    services.AddSingleton<IHuffmanService, HuffmanService>();
    services.AddTransient<ITreeGenerationService, TreeGenerationService>();
    services.AddTransient<App>();
}).ConfigureAppConfiguration((hostingContext, configuration) =>
{
    configuration.Sources.Clear();

    var env = hostingContext.HostingEnvironment;

    configuration
        .AddJsonFile(@"C:\Users\c\git\Huffman\Huffman\appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
}).Build();

static void StartApp(IServiceProvider hostProvider)
{
    using var serviceScope = hostProvider.CreateScope();
    var provider = serviceScope.ServiceProvider;
    var app = provider.GetRequiredService<App>();

    // app.RunWithStringAndDebugInfo("MLJLKWFUIHPONVCVPOOAODXJYDGHWFBAPCWUIOPAPKROJNYSPLCYAIMRTSSCRTDMRAQNLPBNIBEYQVTSQCKVTDDRODRGRLJNTJGL");
    // app.RunWithStringAndDebugInfo("Hello");
    // await app.RunHamlet();
    // app.RunProfiling();
    // app.ProfileDeserialization();
    app.ProfileSerialization();
    Environment.Exit(0);
}

StartApp(host.Services);
await host.RunAsync();