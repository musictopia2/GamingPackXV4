namespace BasicMultiplayerGames.Blazor.Views;
public partial class BasicMultiplayerGamesMainView
{
    [CascadingParameter]
    public TestOptions? TestData { get; set; }
    private readonly BasicList<LabelGridModel> _labels = new();
    protected override void OnInitialized()
    {
        _labels.Clear();
        _labels.AddLabel("Turn", nameof(BasicMultiplayerGamesVMData.NormalTurn))
            .AddLabel("Status", nameof(BasicMultiplayerGamesVMData.Status));
        base.OnInitialized();
    }
}