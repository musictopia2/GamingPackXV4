#nullable enable
namespace FillOrBust.Resources;
public static class SvgPngImageResource
{
    private readonly static string _revenge = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGUAAABPCAYAAADySwufAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAG7SURBVHhe7dGNaoQwEADhe/+XvlKoIDJxNX9u7HwwUM6ct918JEmSJEmSJEl6ge/3+/eXUvi9kC0l4aUks78QLyYJLyUZupAtPYAu4pgmo0s4ponoAkppElp+KU1Ai4/SYLT0KA1EC7+aBqBF302d0ZLvpo5owbWpE1pubeqAFtuaGtFSW1MDWmivVIEW2Tt1QsuN+rdmLWC/7KvNMPO3LtsvYeSA9DtRo8z8rSo04L5e6N1RPdH796VCA5ZqQe+LakXvLJUODRl1F70jqga950rp0JB3uoK+F3UVffdOKdGgtZXQ2agzdL62lGjQHu3R86gjOtOjlGjQ1o7oTBShc62lRcPWRuhcVAmdbSktGramEjobdYbO15QaDVxTCZ2NOkPna0qNBu5V7fs39KxXqdHA/6H0aOi3lx4N/eaWQIO/uSXQ4G9uCTT4m1sGDf/WllEanj5fodLsSzkb/vgse3tnz9K7Ovj+XKYiV8+lUjPw9o8+1V0131necWm9Uye03CgNRkuP0mC09CgNRkuP0mC09CgNRkuP0mC09ChNQIsvpUlo+aU0EV3AMT2ALmJLD/MyJEmSVOnz+QFVW7pDD1mj5QAAAABJRU5ErkJggg==";
    public static string Revenge => "revenge.png";
    public static void Register()
    {
        CommonBasicLibraries.AdvancedGeneralFunctionsAndProcesses.FileFunctions.FileContentRegistry.RegisterFile("revenge.png", _revenge);
    }
}