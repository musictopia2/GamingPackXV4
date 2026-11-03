#nullable enable
namespace YaBlewIt.Resources;
public static class SvgPngImageResource
{
    private readonly static string _dottedline = "data:image/svg+xml;base64,77u/PHN2ZyB3aWR0aD0iNTAwIiBoZWlnaHQ9IjIwIiB2aWV3Qm94PSIwIDAgNTAwIDIwIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciID4NCgk8bGluZSB4MT0iMCIgeTE9IjAiIHgyPSI1MDAiIHkyPSIwIiBzdHJva2U9ImJsYWNrIiBzdHJva2Utd2lkdGg9IjIwIg0KICAgICAgICAgIHN0cm9rZS1kYXNoYXJyYXk9IjgiIC8+DQoJDQo8L3N2Zz4=";
    public static string DottedLine => "DottedLine.svg";
    public static void Register()
    {
        CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileContentRegistry.RegisterFile("DottedLine.svg", _dottedline);
    }
}