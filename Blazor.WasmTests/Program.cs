using System;
using System.Threading.Tasks;
using System.Diagnostics;
using Blazor.WasmTests;
using Blazor.WasmTests.Models;
using Microsoft.JSInterop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MonacoRoslynCompletionProvider.Api;

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
        return $"Hello {name}! Current time is {time}. '{typeof(Console).Assembly.Location}'";
    }

    public static async Task<DateTime> GetTimeViaJS ()
    {
        return await js.InvokeAsync<DateTime>("getTime");
    }

    [JSInvokable]
    public static Task<string> FixCondition(
        string code)
    {
        ConditionReturn modifier = new ConditionReturn();
        return Task.FromResult(modifier.Process(code));
    }

    [JSInvokable]
    public static async Task LoadAssemblies(string basePath)
    {
        MonacoRoslynCompletionProvider.DefaultMetadataReferences.InitHttpClient(basePath);

        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("System.Console.dll");
        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("System.Runtime.dll");
        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("System.Collections.dll");
        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("mscorlib.dll");
        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("netstandard.dll");
        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("System.ComponentModel.Primitives.dll");
        await MonacoRoslynCompletionProvider.DefaultMetadataReferences.LoadFromPath("System.Linq.dll");
    }

    [JSInvokable]
    public static async Task<TabCompletionResult[]> GetTabCompletion(
        string code,
        int position)
    {
        try
        {
            return await MonacoRoslynCompletionProvider.CompletitionRequestHandler.Handle(
                new TabCompletionRequest()
                {
                    Code = code,
                    Position = position,
                });
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message + " " + ex.StackTrace);
        }
    }

    [JSInvokable]
    public static async Task<HoverInfoResult> GetHoverInfo(
        string code,
        int position)
    {
        return await MonacoRoslynCompletionProvider.CompletitionRequestHandler.Handle(
            new HoverInfoRequest()
            {
                Code = code,
                Position = position,
            });
    }

    [JSInvokable]
    public static async Task<CodeCheckResult[]> CheckCode(
        string code)
    {
        return await MonacoRoslynCompletionProvider.CompletitionRequestHandler.Handle(
            new CodeCheckRequest()
            {
                Code = code,
            });
    }
}