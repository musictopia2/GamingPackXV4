namespace Fluxx.Blazor.Views;
public partial class ActionEverybodyGetsOneView : SimpleActionView<ActionEverybodyGetsOneViewModel>
{
    private ICustomCommand GiveCommand => DataContext!.GiveCardsCommand!;
}