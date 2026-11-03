namespace Froggies.Blazor.Views;
public partial class FroggiesMainView
{
    private BasicList<LabelGridModel> Labels { get; set; } = new();
    protected override void OnInitialized()
    {
        Labels.Clear();
        Labels.AddLabel("Moves Left", nameof(FroggiesMainViewModel.MovesLeft))
            .AddLabel("How Many Frogs Currently", nameof(FroggiesMainViewModel.NumberOfFrogs))
            .AddLabel("How Many Frogs To Start", nameof(FroggiesMainViewModel.StartingFrogs));
        base.OnInitialized();
    }
}