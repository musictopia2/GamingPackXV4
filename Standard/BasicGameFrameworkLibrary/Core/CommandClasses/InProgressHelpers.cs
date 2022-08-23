namespace BasicGameFrameworkLibrary.Core.CommandClasses;

public static class InProgressHelpers
{
    //decided the inprogresshelpers belong to the command class because it helps the command class the most.
    public static bool MoveInProgress { get; set; }
    public static bool Reconnecting { get; set; }
}