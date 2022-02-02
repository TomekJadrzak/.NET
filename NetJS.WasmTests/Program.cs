using System;
using System.Diagnostics;
using DotNetJS;
using Microsoft.JSInterop;
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
}