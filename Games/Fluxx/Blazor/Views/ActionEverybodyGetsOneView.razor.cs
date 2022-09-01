namespace Fluxx.Blazor.Views;
public partial class ActionEverybodyGetsOneView : SimpleActionView
{
    [CascadingParameter]
    public ActionEverybodyGetsOneViewModel? DataContext { get; set; }
    private ICustomCommand GiveCommand => DataContext!.GiveCardsCommand!;
}