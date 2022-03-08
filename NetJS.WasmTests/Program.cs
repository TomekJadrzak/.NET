using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DotNetJS;
using Microsoft.JSInterop;
using MonacoRoslynCompletionProvider.Api;
using NetJS.WasmTests.Models;

namespace NetJS.WasmTests;

public partial class Program
{
    // Entry point is invoked by the JavaScript runtime on boot.
    public static void Main ()
    {
        // Invoking 'dotnet.HelloWorld.GetHostName()' JavaScript function.
        var hostName = GetHostName();
        // Writing to JavaScript host console.
        Console.WriteLine($"Hello {hostName}, DotNet here!");
    }

    [JSFunction] // The interoperability code is auto-generated.
    public static partial string GetHostName ();

    [JSInvokable] // The method is invoked from JavaScript.
    public static string GetName () => "DotNet";

    [JSInvokable]
    public static ProcessInfo PassProcessInfo(ProcessInfo info)
    {
        Console.WriteLine($"{info.Id} {info.UnitSymbol} {info.Active} {info.ActiveFrom}");

        return info;
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