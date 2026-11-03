namespace BasicGameFrameworkLibrary.Blazor.Helpers;
public static class LayoutInfo
{
    public const int MaxedCompactHeight = 280;
    public const int MaxedDesktopHeight = 750;
    public const int MaxedTabletHeight = 550;

    public static EnumGameLayoutMode LayoutMode { get; set; }
        = EnumGameLayoutMode.Desktop;

    public static int GetHeight(double percent)
    {
        return LayoutMode switch
        {
            EnumGameLayoutMode.Compact => GetCompactHeight(percent),
            EnumGameLayoutMode.Tablet => GetTabletHeight(percent),
            EnumGameLayoutMode.Desktop => GetDesktopHeight(percent),
            _ => throw new InvalidOperationException(
                $"Unsupported layout mode: {LayoutMode}.")
        };
    }

    public static int GetCompactHeight(double percent)
        => CalculateHeight(MaxedCompactHeight, percent);

    public static int GetDesktopHeight(double percent)
        => CalculateHeight(MaxedDesktopHeight, percent);

    public static int GetTabletHeight(double percent)
        => CalculateHeight(MaxedTabletHeight, percent);

    private static int CalculateHeight(int maximum, double percent)
    {
        if (double.IsNaN(percent) ||
            double.IsInfinity(percent) ||
            percent < 0 ||
            percent > 100)
        {
            throw new ArgumentOutOfRangeException(
                nameof(percent),
                "Percent must be between 0 and 100.");
        }

        return (int)Math.Round(
            maximum * percent / 100.0,
            MidpointRounding.AwayFromZero);
    }
}