using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Blazor.WasmTests;
using Blazor.WasmTests.Models;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Blazor.WasmTests;

public class Program
{
    private static IJSRuntime js;

    private static async Task Main (string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        var host = builder.Build();
        js = host.Services.GetRequiredService<IJSRuntime>();
        await host.RunAsync();
    }

    [JSInvokable]
    public static async Task<string> BuildMessage (string name)
    {
        var time = await GetTimeViaJS();
        return $"Hello {name}! Current time is {time}.";
    }

    public static async Task<DateTime> GetTimeViaJS ()
    {
        return await js.InvokeAsync<DateTime>("getTime");
    }

    [JSInvokable]
    public static Task<string> FixCondition(string code)
    {
        ConditionReturn modifier = new ConditionReturn();
        return Task.FromResult(modifier.Process(code));
    }
}