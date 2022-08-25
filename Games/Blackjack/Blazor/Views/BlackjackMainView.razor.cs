namespace Blackjack.Blazor.Views;
public partial class BlackjackMainView
{
    private BasicList<LabelGridModel> WinLabels { get; set; } = new();
    private BasicList<LabelGridModel> PointLabels { get; set; } = new();
    protected override void OnInitialized()
    {
        WinLabels.Clear();
        WinLabels.AddLabel("Wins", nameof(BlackjackMainViewModel.Wins))
            .AddLabel("Losses", nameof(BlackjackMainViewModel.Losses))
            .AddLabel("Draws", nameof(BlackjackMainViewModel.Draws));
        PointLabels.Clear();
        PointLabels.AddLabel("Human Points", nameof(BlackjackMainViewModel.HumanPoints))
            .AddLabel("Computer Points", nameof(BlackjackMainViewModel.ComputerPoints));
        //if you have to add command change, do so as well.
        base.OnInitialized();
    }
    private ICustomCommand StayCommand => DataContext!.StayCommand!;
    private ICustomCommand HitCommand => DataContext!.HitCommand!;
    private ICustomCommand AceCommand => DataContext!.AceCommand!;
}