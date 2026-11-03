#nullable enable
namespace FiveCrowns.Resources;
public static class SvgPngImageResource
{
    private readonly static string _deckofcardback = "data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0iVVRGLTgiPz4NCjxzdmcgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIiB3aWR0aD0iMjA4IiBoZWlnaHQ9IjMwMyI+DQo8Y2xpcFBhdGggaWQ9InIiPjxyZWN0IHg9Ii41IiB5PSIuNSIgd2lkdGg9IjIwNyIgaGVpZ2h0PSIzMDIiIHJ4PSI4Ii8+PC9jbGlwUGF0aD4NCjxnIGNsaXAtcGF0aD0idXJsKCNyKSI+DQo8IS0tPHBhdGggZmlsbD0iI0ZGRiIgZD0ibTAsMGgyMDh2MzAzSDAiLz4tLT4NCjxwYXRoIHN0cm9rZT0iI0QxMTIwOSIgc3Ryb2tlLXdpZHRoPSI0MzAiIHN0cm9rZS1kYXNoYXJyYXk9IjMuNjciIGQ9Im0wLDI5NCAzMDYtMzAzIi8+DQo8L2c+DQoNCjwvc3ZnPg==";
    public static string Deckofcardback => "deckofcardback.svg";
    public static void Register()
    {
        CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileContentRegistry.RegisterFile("deckofcardback.svg", _deckofcardback);
    }
}