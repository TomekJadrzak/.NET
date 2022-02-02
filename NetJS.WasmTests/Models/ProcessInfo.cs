using System;

namespace NetJS.WasmTests.Models;
public class ProcessInfo
{
    public int Id { get; set; }
    public string UnitSymbol { get; set; }
    public bool Active { get; set; }
    public DateTime ActiveFrom { get; set; }
}
