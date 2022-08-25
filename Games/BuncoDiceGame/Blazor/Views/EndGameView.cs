namespace BuncoDiceGame.Blazor.Views;
public class EndGameView : BasicCustomButtonView<EndGameViewModel>
{
    protected override string DisplayName => "End Game";
    protected override ICustomCommand Command => DataContext!.EndGameCommand!;
}